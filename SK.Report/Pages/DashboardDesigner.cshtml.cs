using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using Wangkanai.Detection.Models;
using Wangkanai.Detection.Services;

namespace SK.Report.Pages
{
    public class DashboardDesignerModel : PageModel
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

        public bool DesktopDevice { get; }

        public DashboardDesignerModel(IDetectionService detectionService, ILogger<DashboardDesignerModel> logger)
        {
            DesktopDevice = detectionService.Device.Type == Device.Desktop;
            _logger = logger;
        }

        public void OnGet(
            string userID,
            string workspaceID,
            string reportID,
            string editMode,
            string language,
            string statusTitle,
            string objectID,
            string objectType)
        {
            UserId = userID;
            WorkspaceID = workspaceID;
            ReportID = reportID;
            EditMode = editMode.Equals("yes", StringComparison.InvariantCultureIgnoreCase);
            Language = language;
            StatusTitle = statusTitle;
            ObjectId = objectID;
            ObjectType = objectType;

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
