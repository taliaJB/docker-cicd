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
    internal class SupplierInet : Supplier
    {
        internal override EnmSupplierName SupplierName => EnmSupplierName.Inet;

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
            return new List<CommonData> { new InetDiagnosticData { InetCarNumber = 123, InetCarModelName = "Toyota" } };
        }

        internal override List<CarData> TransformData(List<CommonData> inetDiagnostics)
        {

            // TO DO: Transfrom List<InetDiagnosticData> => List<CarData>
            List<CarData> CarsData = new List<CarData>();
            foreach (InetDiagnosticData inetDiagnostic in inetDiagnostics)
            {
                CarData carData = new CarData { CarNumber = inetDiagnostic.InetCarNumber, CarModelName = inetDiagnostic.InetCarModelName };
                carData.Copy(inetDiagnostic);
                CarsData.Add(carData);
            }

            return CarsData;
        }
    }

    public class InetDiagnosticData : CommonData
    {
        public int InetCarNumber { get; set; }
        public string InetCarModelName { get; set; }
    }
}
