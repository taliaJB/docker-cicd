using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Eldan.LoggerBase;
using Eldan.TypeExtensions;
namespace Eldan.DiagnosticServicesLib.CarsDiagnostic
{
    public class CarsDiagnosticBL : CarsDiagnosticDAL, IServiceLoggerBL
    {
        public const string LOGGER_FILE_PATH_ONLINE_KEY = "loggerFilePathCarsDiagnosticOnline";
        public const string LOGGER_FILE_PATH_SCHEDUALE_KEY = "loggerFilePathCarsDiagnosticScheduale";
        public const string SERVICE_NAME = "CarsDiagnosticService";

        ServiceLoggerBase _logger;

        public ServiceLoggerBase ServiceLogger { set => _logger = value; }

        public void UpdateSuppliersData()
        {
            GetAndUpdateSupplierData(new SupplierEdi());
            GetAndUpdateSupplierData(new SupplierPointer());
            GetAndUpdateSupplierData(new SupplierIturan());
            GetAndUpdateSupplierData(new SupplierInet());
        }

        private void GetAndUpdateSupplierData(Supplier supplier)
        {
            supplier.Logger = _logger;
            Thread tr = new Thread(() => supplier.GetAndUpdateSupplierData());
            tr.Start();
        }

        public void UpdateSupplierData(List<int> carsNumber)
        {
            List<CarSupplier> carsSupplier = GetCarsSupplier(carsNumber);
            foreach (CarSupplier carSupplier in carsSupplier)
            {
                Supplier supplier = GetSupplier(carSupplier.supplierName);
                supplier.GetAndUpdateSupplierData();
            }
        }

        public void CalibrateSupplierCar(int carNumber, int KM)
        {
            List<CarSupplier> carsSupplier = GetCarsSupplier(new List<int> { carNumber });
            if (carsSupplier.Count == 0)
                throw new Exception($"CarsDiagnosticBL:CalibrateSupplierCar - no supplier founded for car number: {carNumber}");

            Supplier supplier = GetSupplier(carsSupplier.First().supplierName);
            supplier.CalibrateSupplierCar(carNumber, KM);
        }

        public void UpdateEdiData(EdiDiagnosticData ediDiagnosticData)
        {
            SupplierEdi supplierEdi = GetSupplier<SupplierEdi>();
            supplierEdi.UpdateSupplierData(ediDiagnosticData);
        }

        public void UpdatePointerData(PointerDiagnosticData pointerDiagnosticData)
        {
            SupplierPointer supplierPointer = GetSupplier<SupplierPointer>();
            supplierPointer.UpdateSupplierData(pointerDiagnosticData);
        }

        public void UpdateIturanData(IturanDiagnosticData pointerDiagnosticData)
        {
            SupplierIturan supplierIturan = GetSupplier<SupplierIturan>();
            supplierIturan.UpdateSupplierData(pointerDiagnosticData);
        }

        public void UpdateInetData(InetDiagnosticData pointerDiagnosticData)
        {
            SupplierInet supplierInet = GetSupplier<SupplierInet>();
            supplierInet.UpdateSupplierData(pointerDiagnosticData);
        }

        private Supplier GetSupplier(EnmSupplierName supplierName)
        {
            Supplier supplier;

            switch (supplierName)
            {
                case EnmSupplierName.Edi:
                    supplier = new SupplierEdi();
                    break;
                case EnmSupplierName.Pointer:
                    supplier = new SupplierPointer();
                    break;
                case EnmSupplierName.Ituran:
                    supplier = new SupplierIturan();
                    break;
                case EnmSupplierName.Inet:
                    supplier = new SupplierInet();
                    break;
                default:
                    throw new Exception($"CarsDiagnosticBL:GetSupplier - Supplier {supplierName} is not supported");
            }

            supplier.Logger = _logger;

            return supplier;
        }

        private T GetSupplier<T>() where T : Supplier, new()
        {
            T supplier = new T
            {
                Logger = _logger
            };
            return supplier;
        }
    }
}
