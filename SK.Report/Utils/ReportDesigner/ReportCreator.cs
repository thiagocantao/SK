using System.Collections.Generic;
using System.Drawing;
using DevExpress.DashboardCommon;
using DevExpress.Drawing.Printing;
using DevExpress.Drawing;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using SK.Report.Models;

namespace SK.Report.Utils.ReportDesigner
{
    public partial class ReportCreator : XtraReport
    {
        private TopMarginBand topMarginBand1;
        private DetailBand detailBand1;
        private BottomMarginBand bottomMarginBand1;

        //private DevExpress.XtraReports.UI.DetailBand Detail;
        //private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        //private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;

        private System.ComponentModel.IContainer components = null;
        class Employee
        {
            public string Name { get; set; }
        }
        public ReportCreator()
        {
            //Name = "SimpleStaticReport";
            //DisplayName = "Simple Static Report";
            //PaperKind = DXPaperKind.Letter;
            //Margins = new DXMargins(100, 100, 100, 100);
            //DetailBand detailBand = new DetailBand()
            //{
            //    HeightF = 25
            //};
            //Bands.Add(detailBand);

            //XRLabel helloWordLabel = new XRLabel()
            //{
            //    Text = "Hello, World!",
            //    Font = new DXFont("Tahoma", 20f, DXFontStyle.Bold),
            //    BoundsF = new RectangleF(0, 0, 250, 50),
            //};
            //detailBand.Controls.Add(helloWordLabel);
            //InitializeComponent();
            DataSource = CreateDataSource();
            StyleSheet.Add(new XRControlStyle() { Name = "Title", Font = new Font("Tahoma", 20f, FontStyle.Bold) });
            StyleSheet.Add(new XRControlStyle() { Name = "Normal", Font = new Font("Tahoma", 10f), Padding = new PaddingInfo(2, 2, 0, 0) });
            var reportHeaderBand = CreateReportHeader("List of employees");
            var detailBand = CreateDetail("[Name]");
            Bands.AddRange(new Band[] { reportHeaderBand, detailBand });
        }
        static List<Employee> CreateDataSource()
        {
            return new List<Employee>() {
                new Employee() { Name = "Nancy Davolio" },
                new Employee() { Name = "Andrew Fuller" },
                new Employee() { Name = "Janet Leverling" },
                new Employee() { Name = "Margaret Peacock" },
                new Employee() { Name = "Steven Buchanan" },
                new Employee() { Name = "Michael Suyama" },
                new Employee() { Name = "Robert King" },
                new Employee() { Name = "Laura Callahan" },
                new Employee() { Name = "Anne Dodsworth" }
            };
        }
        static ReportHeaderBand CreateReportHeader(string title)
        {
            ReportHeaderBand reportHeaderBand = new ReportHeaderBand()
            {
                HeightF = 50
            };
            XRLabel titleLabel = new XRLabel()
            {
                Text = title,
                BoundsF = new RectangleF(0, 0, 300, 30),
                StyleName = "Title"
            };
            reportHeaderBand.Controls.Add(titleLabel);
            return reportHeaderBand;
        }
        static DetailBand CreateDetail(string expression)
        {
            DetailBand detailBand = new DetailBand()
            {
                HeightF = 25
            };
            XRLabel detailLabel = new XRLabel()
            {
                ExpressionBindings = { new ExpressionBinding("Text", expression) },
                BoundsF = new RectangleF(0, 0, 300, 20),
                StyleName = "Normal"
            };
            
            detailBand.Controls.Add(detailLabel);
            return detailBand;
        }

        private void InitializeComponent()
        {
            this.topMarginBand1 = new DevExpress.XtraReports.UI.TopMarginBand();
            this.detailBand1 = new DevExpress.XtraReports.UI.DetailBand();
            this.bottomMarginBand1 = new DevExpress.XtraReports.UI.BottomMarginBand();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // topMarginBand1
            // 
            this.topMarginBand1.Name = "topMarginBand1";
            // 
            // detailBand1
            // 
            this.detailBand1.Name = "detailBand1";
            // 
            // bottomMarginBand1
            // 
            this.bottomMarginBand1.Name = "bottomMarginBand1";
            // 
            // ReportCreator
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.topMarginBand1,
            this.detailBand1,
            this.bottomMarginBand1});
            this.Version = "23.2";
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

            //this.SaveLayoutToXml()

        }
    }
}
