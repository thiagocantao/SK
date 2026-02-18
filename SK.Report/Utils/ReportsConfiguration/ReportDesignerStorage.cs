using DevExpress.DataAccess.Sql;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Expressions;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.Extensions;
using Microsoft.EntityFrameworkCore;
using Npgsql.Replication.PgOutput.Messages;
using SK.Report.Data;
using SK.Report.Models;
using SK.Report.Services;
using System.Text;
using System.Xml.Linq;
using Wangkanai.Extensions;

namespace SK.Report.Utils.ReportsConfiguration
{
    public class ReportDesignerStorage : ReportStorageWebExtension
    {
        protected readonly ReportContext _DbContext;
        protected readonly SessionDataStorageService _SessionDataStorageService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ReportDesignerStorage(
            ReportContext dbContext, 
            SessionDataStorageService sessionDataStorageService,
            IHttpContextAccessor httpContextAccessor
            )
        {
            this._DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this._SessionDataStorageService = sessionDataStorageService ?? throw new ArgumentNullException(nameof(sessionDataStorageService));
            this._httpContextAccessor = httpContextAccessor;
        }

        public override bool CanSetData(string url)
        {
            return true;
        }

        public override byte[] GetData(string url)
        {
            var sessao = this._SessionDataStorageService.SessionData;
            if(sessao == null) { throw new ArgumentNullException(nameof(sessao) + "is null"); }
            var reportId = url;
            var reportData = _DbContext.Reports.FirstOrDefault(x => x.Id == reportId);

            if (reportData == null || string.IsNullOrEmpty(reportData.Content))
            {
                using var ms = new MemoryStream();
                XtraReport report = new XtraReport();
                report.SourceUrl = reportId;
                report.Name = reportData.Title;

                report.Parameters.Add(new Parameter(){Name = "pa_user",Type = typeof(string),Value = sessao.UserID,Visible = false});
                report.Parameters.Add(new Parameter() { Name = "pa_object", Type = typeof(string), Value = sessao.ObjectID, Visible = false });
                report.Parameters.Add(new Parameter() { Name = "pa_object_type", Type = typeof(string), Value = sessao.ObjectType, Visible = false });
                report.Parameters.Add(new Parameter() { Name = "pa_workspace", Type = typeof(string), Value = sessao.WorkspaceID, Visible = false });

                report.SaveLayoutToXml(ms);
                return ms.ToArray();
            }
            return Encoding.UTF8.GetBytes(reportData.Content);
        }

        public override Dictionary<string, string> GetUrls()
        {
            return _DbContext.Reports
                   .ToList()
                   .Where(p => p.ReportType == "ReportDesigner")
                   .Select(x => x.Title)
                   .ToDictionary<string, string>(x => x);
            //return base.GetUrls();
        }

        public override void SetData(XtraReport report, string reportName)
        {
            var reportId = reportName;
            using var stream = new MemoryStream(); 
            report.SaveLayoutToXml(stream);
            
            var reportData = _DbContext.Reports.FirstOrDefault(x => x.Id == reportId);

            using var reader = new StreamReader(stream);
            reader.BaseStream.Position = 0;

            var content = reader.ReadToEnd();

            if (reportData == null)
            {
                _DbContext.Reports.Add(new Models.Report { 
                    Id= this._SessionDataStorageService.SessionData.ObjectID ?? "",
                    Title = reportName, 
                    App = "SmartKanvas",
                    ReportType = "Report",
                    IsValid = true,
                    Content = content 
                });
            }
            else
            {
                reportData.Content = content;
            }
            _DbContext.SaveChanges();

            //base.SetData(report, url);
        }

        public override bool IsValidUrl(string url)
        {
            return true;
        }

        public override string SetNewData(XtraReport report, string defaultUrl)
        {
            SetData(report, defaultUrl);
            return defaultUrl;
            //return base.SetNewData(report, defaultUrl);
        }

    }
}
