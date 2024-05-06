using Eldan.LoggerBase;
using Eldan.ServiceMonitor;
using Eldan.TypeExtensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eldan.DiagnosticServicesLib.CarsDiagnostic
{
    public enum EnmSupplierName
    {
        Edi = 1,
        Pointer = 2,
        Ituran = 3,
        Inet = 4
    }

    abstract internal class Supplier : CarsDiagnosticDAL
    {
        ServiceLoggerBase _logger;

        internal ServiceLoggerBase Logger 
        { 
            set => _logger = value;
            get => _logger;
        }

        internal void UpdateSupplierData(CommonData supplierDiagnosticData)
        {
            if (!PushingSupported)
            {
                _logger.Write($"SuppliersEngine.UpdateSupplierData - Supplier {SupplierName} dose not support pushing data");
                return;
            }

            List<CarData> carsData = TransformData(new List<CommonData> { supplierDiagnosticData });
            if (carsData.Count == 0)
                throw new Exception($"SuppliersEngine.UpdateSupplierData - no cars to update for {SupplierName} supplier");

            // Update supplier diagnostic data
            UpdateCarsDiagnostic(carsData.First());
        }

        internal void GetAndUpdateSupplierData()
        {
            if (!PullingSupported)
            {
                _logger.Write($"SuppliersEngine.GetAndUpdateSupplierData - Supplier {SupplierName} dose not support pulling data");
                return;
            }

            // Get supplier diagnostic data
            List<CarData> carsData = GetSupplierData(RequestSupplierData, TransformData);

            // Update supplier diagnostic data
            UpdateCarsDiagnostic(carsData, false);
        }

        internal virtual void CalibrateSupplierCar(int carNumber, int KM)
        {
            _logger.Write($"SuppliersEngine.CalibrateSupplierCar - Attempt to calibrate car number: {carNumber} with KM: {KM} for supplier {SupplierName}");

        }

        private void UpdateCarsDiagnostic(CarData carData)
        {
            UpdateCarsDiagnostic(new List<CarData> { carData }, true);
        }

        private void UpdateCarsDiagnostic(List<CarData> carsData, bool throwException)
        {
            _logger.Write($"SuppliersEngine:UpdateCarDiagnostic - Attempt to update car diagnostic data in DB for {SupplierName} supplier, " +
                          $"carData:='{carsData.ToJSON(true)}', throwException:='{throwException}'");
            try
            {
                UpdateCarsDiagnosticData(carsData);
            }
            catch (Exception ex)
            {
                if (throwException)
                    throw ex;
                DocumentFault($"SuppliersEngine:UpdateCarsDiagnostic - faild to update car diagnostic data in DB for {SupplierName} supplier since:", ex);
            }
        }

        private List<CarData> GetSupplierData(Func<List<int>, List<CommonData>> requestSupplierData, Func<List<CommonData>, List<CarData>> transformData)
        {
            _logger.Write($"SuppliersEngine:GetSupplierData - Attmpt to get {SupplierName} supplier diagnostic data");
            try
            {
                List<int> carsNumber = GetSupplierCarsNumber(SupplierName);
                _logger.Write($"SuppliersEngine:GetSupplierData - {carsNumber.Count} cars number founded for {SupplierName} supplier");

                List<CommonData> suplireDiagnostics = requestSupplierData(carsNumber);
                return transformData(suplireDiagnostics);
            }
            catch (Exception ex)
            {
                DocumentFault($"SuppliersEngine:GetSupplierData - faild to update {SupplierName} supplier data since:", ex);
                return new List<CarData>();
            }
        }

        private List<CarData> GetSupplierData<T>(Func<List<int>, List<T>> requestSupplierData, Func<List<T>, List<CarData>> transformData, int carNumber)
        {
            List<T> suplireDiagnostics = requestSupplierData(new List<int> { carNumber });
            return transformData(suplireDiagnostics);
        }

        private void DocumentFault(string message, Exception ex)
        {
            _logger.Write(message, ex);

            new MonitorDispatcher
            {
                MailAddress = ConfigurationManager.AppSettings["CarsDiagnosticFaultsRecipients"],
                LogFilePathKey = _logger.LoggerFilePathKey,
                ServiceName = CarsDiagnosticBL.SERVICE_NAME,
                LoggerSessionID = _logger.LoggerSessionID
            }.SendFault(ex);
        }

        internal abstract EnmSupplierName SupplierName { get; }

        internal abstract List<CommonData> RequestSupplierData(List<int> carsNumber);

        internal abstract List<CarData> TransformData(List<CommonData> diagnostics);

        internal abstract bool PullingSupported { get; }
        internal abstract bool PushingSupported { get; }

    }
}
