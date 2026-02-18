using DevExpress.DataAccess.Sql;
using DevExpress.XtraReports;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Wizards;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SK.Report.Utils.DashboardsConfiguration;
using System.Text.Json;
using Wangkanai.Detection.Models;
using Wangkanai.Detection.Services;

namespace SK.Report.Pages
{
    public class TestReportModel : PageModel
    {
        private readonly ILogger<DashboardDesignerModel> _logger;

        public string UserId { get; set; } = null!;
        public string WorkspaceID { get; set; } = null!;
        public string ReportID { get; set; } = null!;
        public bool EditMode { get; set; }
        public string Language { get; set; } = null!;
        public string StatusTitle { get; set; } = null!;
        public string ObjectId { get; set; } = null!;
        public string ObjectType { get; set; } = null!;
        public XtraReport report { get; set; } = null;

        public bool DesktopDevice { get; }

        public TestReportModel(IDetectionService detectionService, ILogger<DashboardDesignerModel> logger)
        {
            DesktopDevice = detectionService.Device.Type == Device.Desktop;
            _logger = logger;
        }

        public void OnGet(string userID,
            string workspaceID,
            string reportID,
            string language,
            string statusTitle,
            string objectID,
            string objectType,
            string editMode = "no"
            )
        {
            this.UserId = userID;
            this.WorkspaceID = workspaceID;
            this.ReportID = reportID;
            this.EditMode = editMode.Equals("yes", StringComparison.InvariantCultureIgnoreCase);
            this.Language = language;
            this.StatusTitle = statusTitle;
            this.ObjectId = objectID;
            this.ObjectType = objectType;

            this.report = new XtraReport();

            var traceData = new
            {
                userID,
                workspaceID,
                reportID,
                editMode,
                language,
                statusTitle,
                objectID,
                objectType
            };
            _logger.LogTrace($"LOG - DashboardDesigner: {JsonSerializer.Serialize(traceData)}");
            
        }
    }
}
