using Eldan.LoggerBase;
using Eldan.TypeExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Eldan.DiagnosticServicesLib.CarsDiagnostic
{
    internal class SupplierEdi : Supplier
    {
        internal override EnmSupplierName SupplierName => EnmSupplierName.Edi;

        internal override bool PullingSupported => true;

        internal override bool PushingSupported => true;

        internal override void CalibrateSupplierCar(int carNumber, int KM)
        {
            base.CalibrateSupplierCar(carNumber, KM);
            // TO DO: post supplier to calibrate car by KM;
        }

        internal override List<CommonData> RequestSupplierData(List<int> carsNumber)
        {
            // TO DO: Request supplier data by cars number
            Logger.Write("SupplierEdi.RequestSupplierData - Attempt to deserilized Edi supplier data into List<EdiDiagnosticData>");
            List<CommonData> supplierData;
            try
            {
                // here we deserilized Edi supplier data into List<EdiDiagnosticData>
                List<EdiDiagnosticData> ediDiagnostics = new List<EdiDiagnosticData> { new EdiDiagnosticData { EdiCarNumber = 123, EdiCarModelName = "Toyota" } };
                supplierData = ediDiagnostics.Cast<CommonData>().ToList();
            }
            catch (Exception ex)
            {
                Logger.Write("SupplierEdi.RequestSupplierData - Failed to deserilized Edi supplier data into List<EdiDiagnosticData> since:", ex);
                supplierData = new List<CommonData> { new EdiDiagnosticData { IsSuccess = false, ErrorMessage = ex.Message, ErrorType = CommonData.EnmErrorType.HTTPError } };
            }

            return supplierData;

        }

        internal override List<CarData> TransformData(List<CommonData> ediDiagnostics)
        {
            
            // TO DO: Transfrom List<EdiDiagnosticData> => List<CarData>
            List<CarData> CarsData = new List<CarData>();
            foreach (EdiDiagnosticData ediDiagnostic in ediDiagnostics)
            {
                CarData carData = new CarData { CarNumber = ediDiagnostic.EdiCarNumber, CarModelName = ediDiagnostic.EdiCarModelName };
                carData.Copy(ediDiagnostic);
                CarsData.Add(carData);
            }

            return CarsData;
        }
    }

    public class EdiDiagnosticData : CommonData
    {
        public int EdiCarNumber { get; set; }
        public string EdiCarModelName { get; set; }
    }
}
