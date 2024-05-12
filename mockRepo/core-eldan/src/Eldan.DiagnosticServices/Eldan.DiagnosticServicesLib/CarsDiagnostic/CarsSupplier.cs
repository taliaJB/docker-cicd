using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eldan.DiagnosticServicesLib.CarsDiagnostic
{
    internal class CarsSupplier : List<CarSupplier>
    {
        private CarsSupplier(IEnumerable<CarSupplier> carsSupplier) : base(carsSupplier)
        { }

        internal static CarsSupplier CreateInstace(List<int> carsNumber)
        {
            CarsDiagnosticDAL carsDiagnosticDAL = new CarsDiagnosticDAL();
            return new CarsSupplier(carsDiagnosticDAL.GetCarsSupplier(carsNumber));
        }

        internal List<int> GetCarsNumber(EnmSupplierName supplierName)
        {
            return this.Where(x => x.supplierName == supplierName).Select(x => x.CarNumber).ToList();
        }
    }

    internal class CarSupplier
    {
        public int CarNumber;
        public EnmSupplierName supplierName;
    }
}
