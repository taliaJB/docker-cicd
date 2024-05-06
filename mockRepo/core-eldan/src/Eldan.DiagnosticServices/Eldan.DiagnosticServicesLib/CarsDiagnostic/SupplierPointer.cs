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
    internal class SupplierPointer : Supplier
    {
        internal override EnmSupplierName SupplierName => EnmSupplierName.Pointer;

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
            return new List<CommonData> { new PointerDiagnosticData { PointerCarNumber = 123, PointerCarModelName = "Toyota" } };
        }

        internal override List<CarData> TransformData(List<CommonData> pointerDiagnostics)
        {

            // TO DO: Transfrom List<PointerDiagnosticData> => List<CarData>
            List<CarData> CarsData = new List<CarData>();
            foreach (PointerDiagnosticData pointerDiagnostic in pointerDiagnostics)
            {
                CarData carData = new CarData { CarNumber = pointerDiagnostic.PointerCarNumber, CarModelName = pointerDiagnostic.PointerCarModelName };
                carData.Copy(pointerDiagnostic);
                CarsData.Add(carData);
            }

            return CarsData;
        }
    }

    public class PointerDiagnosticData : CommonData
    {
        public int PointerCarNumber { get; set; }
        public string PointerCarModelName { get; set; }
    }
}
