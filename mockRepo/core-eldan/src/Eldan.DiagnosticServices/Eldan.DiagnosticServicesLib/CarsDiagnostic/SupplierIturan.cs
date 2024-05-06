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
    internal class SupplierIturan : Supplier
    {
        internal override EnmSupplierName SupplierName => EnmSupplierName.Ituran;

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
            return new List<CommonData> { new IturanDiagnosticData { IturanCarNumber = 123, IturanCarModelName = "Toyota" } };
        }

        internal override List<CarData> TransformData(List<CommonData> ituranDiagnostics)
        {

            // TO DO: Transfrom List<IturanDiagnosticData> => List<CarData>
            List<CarData> CarsData = new List<CarData>();
            foreach (IturanDiagnosticData ituranDiagnostic in ituranDiagnostics)
            {
                CarData carData = new CarData { CarNumber = ituranDiagnostic.IturanCarNumber, CarModelName = ituranDiagnostic.IturanCarModelName };
                carData.Copy(ituranDiagnostic);
                CarsData.Add(carData);
            }

            return CarsData;
        }
    }

    public class IturanDiagnosticData : CommonData
    {
        public int IturanCarNumber { get; set; }
        public string IturanCarModelName { get; set; }
    }
}
