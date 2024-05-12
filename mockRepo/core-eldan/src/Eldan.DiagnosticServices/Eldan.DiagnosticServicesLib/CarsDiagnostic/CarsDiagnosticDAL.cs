using Eldan.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eldan.DiagnosticServicesLib.CarsDiagnostic
{
    public class CarsDiagnosticDAL : clsDataAccess
    {
        protected List<int> GetSupplierCarsNumber(EnmSupplierName supplier)
        {
            // TO DO: needs to implement
            return new List<int>();
        }

        internal List<CarSupplier> GetCarsSupplier(List<int> carNumber) 
        {
            // TO DO: needs to implement
            return new List<CarSupplier> { new CarSupplier { CarNumber = 111, supplierName = EnmSupplierName.Edi },
                                           new CarSupplier { CarNumber = 222, supplierName = EnmSupplierName.Pointer }};
        }

        protected void UpdateCarsDiagnosticData(List<CarData> carsData)
        {
            // TO DO: needs to implement

        }

        public CarData GetCarData(int carNumber)
        {
            // TO DO: needs to implement
            return new CarData();
        }
    }
}
