using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TestApp.CarsDiagnosticReference;

namespace TestApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnUpdateSuppliersData_Click(object sender, EventArgs e)
        {
            //CarsDiagnosticReference.CarData carData = new CarsDiagnosticReference.CarData { CarModelName = "mazda", CarNumber = 7 };
            using (CarsDiagnosticReference.CarsDiagnosticServiceClient proxy = new CarsDiagnosticReference.CarsDiagnosticServiceClient())
            {
                proxy.UpdateSuppliersData();
            }
        }

        private void btnUpdateEdiData_Click(object sender, EventArgs e)
        {
            CarsDiagnosticReference.EdiDiagnosticData ediDiagnosticData = new CarsDiagnosticReference.EdiDiagnosticData { EdiCarModelName = "mazda", EdiCarNumber = 7 };
            using (CarsDiagnosticReference.CarsDiagnosticServiceClient proxy = new CarsDiagnosticReference.CarsDiagnosticServiceClient())
            {
                proxy.UpdateEdiData(ediDiagnosticData, "111");
            }
        }

        private void btnUpdatePointerData_Click(object sender, EventArgs e)
        {
            CarsDiagnosticReference.PointerDiagnosticData pointerDiagnosticData = new CarsDiagnosticReference.PointerDiagnosticData { PointerCarModelName = "mazda", PointerCarNumber = 7 };
            using (CarsDiagnosticReference.CarsDiagnosticServiceClient proxy = new CarsDiagnosticReference.CarsDiagnosticServiceClient())
            {
                proxy.UpdatePointerData(pointerDiagnosticData, "222");
            }
        }

        private void btnUpdateSupplierData_Click(object sender, EventArgs e)
        {
            using (CarsDiagnosticReference.CarsDiagnosticServiceClient proxy = new CarsDiagnosticReference.CarsDiagnosticServiceClient())
            {
                proxy.UpdateSupplierData(new List<int> { 111, 222});
            }
        }

        private void btnGetCarData_Click(object sender, EventArgs e)
        {
            using (CarsDiagnosticReference.CarsDiagnosticServiceClient proxy = new CarsDiagnosticReference.CarsDiagnosticServiceClient())
            {
                var res = proxy.GetCarData(123);
            }
        }

        private void btnUpdateIturanData_Click(object sender, EventArgs e)
        {
            CarsDiagnosticReference.IturanDiagnosticData ituranDiagnosticData = new CarsDiagnosticReference.IturanDiagnosticData { IturanCarModelName = "mazda", IturanCarNumber = 7 };
            using (CarsDiagnosticReference.CarsDiagnosticServiceClient proxy = new CarsDiagnosticReference.CarsDiagnosticServiceClient())
            {
                proxy.UpdateIturanData(ituranDiagnosticData, "222");
            }
        }

        private void btnUpdateInetData_Click(object sender, EventArgs e)
        {
            CarsDiagnosticReference.InetDiagnosticData inetDiagnosticData = new CarsDiagnosticReference.InetDiagnosticData { InetCarModelName = "mazda", InetCarNumber = 7 };
            using (CarsDiagnosticReference.CarsDiagnosticServiceClient proxy = new CarsDiagnosticReference.CarsDiagnosticServiceClient())
            {
                proxy.UpdateInetData(inetDiagnosticData, "222");
            }
        }

        private void btnCalibrateSupplierCar_Click(object sender, EventArgs e)
        {
            using (CarsDiagnosticReference.CarsDiagnosticServiceClient proxy = new CarsDiagnosticReference.CarsDiagnosticServiceClient())
            {
                proxy.CalibrateSupplierCar(123, 60000);
            }
        }
    }
}
