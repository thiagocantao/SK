using DevExpress.DashboardCommon;
using DevExpress.DashboardWeb;
using Microsoft.EntityFrameworkCore;
using SK.Report.Data;
using SK.Report.Services;
using System.Web;
using System.Xml.Linq;

namespace SK.Report.Utils.DashboardsConfiguration
{
    public class DashboardStorage : IDashboardStorage
    {
        private const string NewDashboardContent = @$"
            <Dashboard>
              <Title Text=""Novo Dashboard"" />
              <DataSources>
                <SqlDataSource Name=""Fonte de dados"" ComponentName=""sqlDataSource1"" DataProcessingMode=""Client"">
                  <Connection Name=""{CustomDataSourceWizardConnectionStringsProvider.ConnectionName}"" FromAppConfig=""true"">
                  </Connection>
                  <ConnectionOptions CloseConnection=""true"" />
                </SqlDataSource>
              </DataSources>
            </Dashboard>";

        private readonly ReportContext _context;
        private readonly SessionDataStorageService _sessionDataStorageService;

        public DashboardStorage(ReportContext context, SessionDataStorageService sessionDataStorageService)
        {
            _context = context;
            _sessionDataStorageService = sessionDataStorageService;
        }

        public IEnumerable<DashboardInfo> GetAvailableDashboardsInfo() => new List<DashboardInfo>();

        public XDocument LoadDashboard(string dashboardID)
        {
            var doc = GetDashboardContent(dashboardID);
            var d = new Dashboard();
            d.LoadFromXDocument(doc);

            return d.SaveToXDocument();
        }

        private XDocument GetDashboardContent(string dashboardID)
        {
            var report = _context.Reports
                            .AsNoTracking()
                            .FirstOrDefault(x => x.Id == dashboardID);

            if (string.IsNullOrWhiteSpace(report?.Content)) return XDocument.Parse(NewDashboardContent);

            return XDocument.Parse(ReplaceDashboardXmlWithQueryStringParameters(report));
        }

        private string ReplaceDashboardXmlWithQueryStringParameters(Models.Report report)
        {
            var xml = report.Content ?? string.Empty;
            const string editModeYes = "yes";
            var sessao = _sessionDataStorageService.SessionData;
            var editMode = editModeYes.Equals(sessao?.EditMode, StringComparison.InvariantCultureIgnoreCase);
            return editMode ?
                xml :
                xml
                    .Replace("<Parameter Name=\"pa_user\" Type=\"System.String\"></Parameter>", $"<Parameter Name=\"pa_user\" Type=\"System.String\">{sessao?.UserID}</Parameter>")
                    .Replace("<Parameter Name=\"pa_object\" Type=\"System.String\"></Parameter>", $"<Parameter Name=\"pa_object\" Type=\"System.String\">{sessao?.ObjectID}</Parameter>")
                    .Replace("<Parameter Name=\"pa_object_type\" Type=\"System.String\"></Parameter>", $"<Parameter Name=\"pa_object_type\" Type=\"System.String\">{sessao?.ObjectType}</Parameter>")
                    .Replace("<Parameter Name=\"pa_workspace\" Type=\"System.String\"></Parameter>", $"<Parameter Name=\"pa_workspace\" Type=\"System.String\">{sessao?.WorkspaceID}</Parameter>");
        }

        public void SaveDashboard(string dashboardID, XDocument dashboard)
        {
            var report = _context.Reports.FirstOrDefault(x => x.Id == dashboardID);

            if (report != null)
            {
                report.Content = dashboard.ToString();
                report.IsValid = true;
                _context.SaveChanges();
            }
        }
    }
}
