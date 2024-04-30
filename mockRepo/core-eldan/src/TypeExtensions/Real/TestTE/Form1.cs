using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Eldan.TypeExtensions;
using System.Xml.Linq;
using System.Diagnostics;
using TestTE.ObjectClone;

namespace TestTE
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string rr = "1234".Right(2);
            string ll = "1234".Left(2);

            DateTime dd = DateTime.MinValue.ParseEldanDate("19700812","152211");

            string ed = dd.ToEldanDate();
            string et = dd.ToEldanTime();

            

            XDocument doc = new XDocument(
              new XElement("Root",
                  new XElement("Child1",
                      new XElement("Child2",
                          new XElement("Child3", new XAttribute("ID", 1)),
                          new XElement("Child3", new XAttribute("ID", 2)))),
                  new XElement("Child2",
                      new XElement("Child3", new XAttribute("ID", 2)))));
            IEnumerable<XElement> res = doc.Root.DescendantsByPath("Child2", "Child3");

            var res2 = doc.DescendantsByPath("Child2", "Child3");

            var res3 = doc.Root.ElementsByPath("Child2", "Child3");
            var res4 = doc.ElementsByPath("Child2", "Child3");


            var res5 = doc.Descendants(null);
            var res6 = doc.DescendantsByPath(null);
        }

        private struct TestText
        {
            public string aa;
            public string bb;
            public string cc;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            TestText din = new TestText();
            din.aa = "123456";
            din.bb = "123";
            din.cc = "12345678";

            string res = din.ToJSON();

            string res2 = res.MaskJSONParam("bb", 4);

        }

        private void button3_Click(object sender, EventArgs e)
        {
            string JSON = "{\"name\":\"John\", \"age\":30, \"car\":null }";

            XDocument doc = XMLExtensions.ParseJSON(JSON, true);

            XDocument doc2 = new XDocument().ParseJSON(JSON, "xxx");
            

        }

        private void button4_Click(object sender, EventArgs e)
        {
            XDocument doc1 = Eldan.TypeExtensions.Excel.GetXMLDoc(@"C:\Main\Eldan.TypeExtensions\TestTE\TestFiles\report.xls");
        }

        private void btnIsFileExist_Click(object sender, EventArgs e)
        {
            string MyFile = @"c:\temp\abc.txt";

            bool res = MyFile.IsFileExist(8, 2000, WriteMsg);
            //bool res2 = MyFile.IsFileExist(3, 2000);
        }

        private void WriteMsg(string message)
        {
            Debug.WriteLine(DateTime.Now.ToLongTimeString() + " - " + message);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string Res = @"\\files01\Enterprise\הסכם מכר.pdf".GetFilePath();
            //string Res = @"xxx".GetFilePath();
        }

        public enum EnmT
        {
            A = 1,
            B = 2
        }

        private void btnGetElementValue_Click(object sender, EventArgs e)
        {
            XDocument Doc = XDocument.Parse("<root><child>null</child></root>");

            var Res1 = XDocument.Parse("<root><child>null</child></root>").Root.Element("child").GetElementValue<int?>();
            var Res11 = XDocument.Parse("<root><child>xx</child></root>").Root.Element("child").GetElementValue<int>();
            try
            {
                var Res112 = XDocument.Parse("<root><child>xx</child></root>").Root.Element("child").GetElementValue<int>(false);
            }
            catch (Exception ex)
            {
                var Res1121 = ex.Message;
            }
            var Res113 = XDocument.Parse("<root><child>xx</child></root>").Root.Element("child").GetElementValue(true, 99);

            var Res111 = XDocument.Parse("<root><child>xx</child></root>").Root.Element("child").GetElementValue(77);
            var Res2 = XDocument.Parse("<root><child>22</child></root>").Root.Element("child").GetElementValue<int?>();
            var Res3 = XDocument.Parse("<root><child>a</child></root>").Root.Element("child").GetElementValue<EnmT>();
            var Res4 = XDocument.Parse("<root><child>2</child></root>").Root.Element("child").GetElementValue<EnmT>();
            var Res5 = XDocument.Parse("<root><child>7</child></root>").Root.Element("child").GetElementValue<EnmT>();
            var Res6 = XDocument.Parse("<root><child>2.00</child></root>").Root.Element("child").GetElementValue<EnmT>();
            var Res7 = XDocument.Parse("<root><child>xxx</child></root>").Root.Element("child").GetElementValue<EnmT>();
            var Res71 = XDocument.Parse("<root><child>xxx</child></root>").Root.Element("child").GetElementValue(EnmT.B);

            var Res31 = XDocument.Parse("<root><child>a</child></root>").Root.Element("child").GetElementValue<EnmT?>();
            var Res41 = XDocument.Parse("<root><child>2</child></root>").Root.Element("child").GetElementValue<EnmT?>();
            var Res51 = XDocument.Parse("<root><child>7</child></root>").Root.Element("child").GetElementValue<EnmT?>();
            var Res61 = XDocument.Parse("<root><child>2.00</child></root>").Root.Element("child").GetElementValue<EnmT?>();
            var Res711 = XDocument.Parse("<root><child>xxx</child></root>").Root.Element("child").GetElementValue<EnmT?>();
            var Res712 = XDocument.Parse("<root><child>xxx</child></root>").Root.Element("child").GetElementValue<EnmT?>(EnmT.B);
            var Res713 = XDocument.Parse("<root><child>xxx</child></root>").Root.Element("child").GetElementValue<EnmT?>(true, EnmT.B);

            var Res81 = XDocument.Parse("<root><child>xxx</child></root>").Root.Element("NoElement").GetElementValue<string>();
            var Res82 = XDocument.Parse("<root><child>xxx</child></root>").Root.Element("NoElement").GetElementValue<string>(null);
            var Res821 = XDocument.Parse("<root><child>xxx</child></root>").Root.Element("NoElement").GetElementValue(" ");
            var Res83 = XDocument.Parse("<root><child></child></root>").Root.Element("child").GetElementValue<string>();
            var Res84 = XDocument.Parse("<root><child>xxx</child></root>").Root.Element("NoElement").GetElementValue<int>();
            var Res85 = XDocument.Parse("<root><child>xxx</child></root>").Root.Element("NoElement").GetElementValue(7);
            var Res86 = XDocument.Parse("<root><child>xxx</child></root>").Root.Element("NoElement").GetElementValue<EnmT>();
            var Res87 = XDocument.Parse("<root><child>xxx</child></root>").Root.Element("NoElement").GetElementValue<EnmT?>();
            var Res88 = XDocument.Parse("<root><child>xxx</child></root>").Root.Element("NoElement").GetElementValue<EnmT?>(EnmT.B);

            var Res89 = XDocument.Parse("<root><child>true</child></root>").Root.Element("child").GetElementValue<bool>();
            // => GetElementValue(bool returnDefaulInFailure := true, T defaultValue := false)
            var Res892 = XDocument.Parse("<root><child>x</child></root>").Root.Element("child").GetElementValue(false);
            try
            {
                // => GetElementValue(bool returnDefaulInFailure := false, T defaultValue := false <which is the default value of bool>)
                var Res891 = XDocument.Parse("<root><child>x</child></root>").Root.Element("child").GetElementValue<bool>(false);
            }
            catch (Exception ex)
            {
                var Res8911 = ex.Message;
            }
            var Res893 = XDocument.Parse("<root><child>x</child></root>").Root.Element("child").GetElementValue(true, false);

            try
            {
                var Res = Doc.Root.Element("child").GetElementValue<int?>(7);
            }
            catch (Exception ex)
            {

                throw;
            }


        }

        private void btnRunMethod_Click(object sender, EventArgs e)
        {
            MySource MS = new MySource();
            bool RV;
            object par = 9;

            List<object> pars = new List<object> { 8 };

            var Res = ObjectCloneExtensions.RunMethod<MySource>("GetString", out RV, pars.ToArray());
        }



        private void btnTryParseType_Click(object sender, EventArgs e)
        {
            bool resEnm = "b".TryParseType(out EnmT psEnm);
            bool resEnm1 = "bxx".TryParseType(out EnmT psEnm1);
            bool resEnm2 = "bxx".TryParseType(out EnmT? psEnm2);
            bool resEnm3 = "a".TryParseType(out EnmT? psEnm3);

            bool resD = "01.02.2020".TryParseType(out DateTime psD);
            bool resD1 = "01.02.2020xxx".TryParseType(out DateTime psD1);
            bool resD2 = "01.02.2020".TryParseType(out DateTime? psD2);
            bool resD3 = "01.02.2020xxxx".TryParseType(out DateTime? psD3);
            bool resD4 = "  ".TryParseType(out DateTime? psD4);
            string xD5 = null;
            bool resD5 = xD5.TryParseType(out DateTime? psD5);

            bool res1 = "1".TryParseType(out int? ps1);
            bool res2 = "1.000".TryParseType(out int? ps2);
            bool res3 = "1.02".TryParseType(out int? ps3);
            bool res31 = "x".TryParseType(out int? ps31);
            bool res311 = "".TryParseType(out int? ps311);
            string x312 = null;
            bool res312 = x312.TryParseType(out int? ps312);

            bool res4 = "1".TryParseType(out int ps4);
            bool res5 = "1.000".TryParseType(out int ps5);
            bool res6 = "1.02".TryParseType(out int ps6);
            bool res61 = "x".TryParseType(out int ps61);
            bool res616 = "".TryParseType(out int ps616);
            string x617 = null;
            bool res617 = x617.TryParseType(out int ps617);

            bool res7 = "1".TryParseType(out EnmT ps7);
            bool res8= "1.000".TryParseType(out EnmT ps8);
            bool res9 = "1.02".TryParseType(out EnmT ps9);
            bool res91 = "x".TryParseType(out EnmT ps91);
            string x915 = null;
            bool res915 = x915.TryParseType(out EnmT ps915);

            bool res711 = "1".TryParseType(out EnmT? ps711);
            bool res811 = "1.000".TryParseType(out EnmT? ps811);
            bool res911 = "1.02".TryParseType(out EnmT? ps911);
            bool res9111 = "x".TryParseType(out EnmT? ps9111);
            bool res9112 = "  ".TryParseType(out EnmT? ps9112);
            string x91n = null;
            bool res91n = x91n.TryParseType(out EnmT? ps91n);

            bool resS1 = "aa".TryParseType(out string psS1);
            bool resS2 = "".TryParseType(out string psS2);
            bool resS3 = "   ".TryParseType(out string psS3);
            string xS91n = null;
            bool resSs91n = xS91n.TryParseType(out string psS91n);
        }

        private void btnGetTypeValue_Click(object sender, EventArgs e)
        {
            var Res1 = "1".GetTypeValue<EnmT>(true);
            var Res2 = "1.0".GetTypeValue<EnmT>(true);
            var Res3 = "1.02".GetTypeValue<EnmT>(true);
            var Res4 = "x".GetTypeValue<EnmT>(true);

            var Res5 = "1".GetTypeValue<EnmT?>(true);
            var Res6 = "1.0".GetTypeValue<EnmT?>(true);
            var Res7 = "1.02".GetTypeValue<EnmT?>(true);
            var Res8 = "x".GetTypeValue<EnmT?>(true);

            try
            {
                var Res9 = "x".GetTypeValue<EnmT?>(false);
            }
            catch (Exception ex)
            {

                throw;
            }


        }

        private void btnToFileName_Click(object sender, EventArgs e)
        {
            var x = "a:b".ToFileName();
        }

        private void btnContainsByParts_Click(object sender, EventArgs e)
        {
            char[] xx = new char[] { 'x' };

            var res1 =  "תל אביב-יפו".ContainsByWords("תל אביב", new char[] { '-', ' ' });

            var res2 = "תל אביב".ContainsByWords("תל אביב", new char[] { '-', ' ' });

            var res3 = "תל אביב".ContainsByWords("תל אביב-יפו", new char[] { '-', ' ' });

            var res4 = "".ContainsByWords("", new char[] { '-', ' ' });

            var res5 = "תל אביב-יפו".ContainsByWords("יפו", new char[] { '-', ' ' });

            var res6 = "    תל     אביב-יפו".ContainsByWords("      יפו     ", new char[] { '-', ' ' });

            var res7 = "תל  אביב-יפו   ".ContainsByWords("    תל    אביב    ", new char[] { '-', ' ' });

            string res81 = null;
            var res8 = res81.ContainsByWords(null, new char[] { '-', ' ' });
        }

        public enum EnmTest
        {
            a = 1,
            b = 2
        }

        private void btnDefinedCast_Click(object sender, EventArgs e)
        {
            EnmTest? test = EnmTest.a;
            var x = test?.DefinedCast<EnmTest>();

            EnmTest? test2 = null;
            var x2 = test2?.DefinedCast<EnmTest>();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string res6 = "102003";
            TimeSpan x = res6.ToTimeSpan();
        }
    }
}

