using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Web;
using SK.Report.Extensions;
using SK.Report.Services;

namespace SK.Report.Utils.DashboardsConfiguration
{
    public class CustomDataSourceWizardConnectionStringsProvider : IDataSourceWizardConnectionStringsProvider
    {
        public const string ConnectionName = "pgSqlConnection";
        private const string DefaultLanguage = "pt_br";
        private readonly SessionDataStorageService _sessionDataStorageService;
        private readonly DatabaseSettingsService _databaseSettingsService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CustomDataSourceWizardConnectionStringsProvider(
            SessionDataStorageService sessionDataStorageService,
            DatabaseSettingsService databaseSettingsService,
            IHttpContextAccessor httpContextAccessor)
        {
            _sessionDataStorageService = sessionDataStorageService;
            _databaseSettingsService = databaseSettingsService;
            _httpContextAccessor = httpContextAccessor;
        }

        public Dictionary<string, string> GetConnectionDescriptions()
        {
            var connections = new Dictionary<string, string>
            {
                { ConnectionName, "Conexão de dados" }
            };
            return connections;
        }

        public DataConnectionParametersBase GetDataConnectionParameters(string name)
        {
            if (name == ConnectionName)
            {
                var language = GetLanguage();
                var sessao = _sessionDataStorageService.SessionData;
                return new CustomStringConnectionParameters(
                    $"XpoProvider=Postgres;" +
                    $"Server={_databaseSettingsService.GetServerName()};" +
                    $"User ID={_databaseSettingsService.GetUserByLanguage(language)};" +
                    $"Password={_databaseSettingsService.GetPasswordByLanguage(language)};" +
                    $"Database={_databaseSettingsService.GetDatabaseName()};" +
                    $"ApplicationName={sessao?.WorkspaceID}" +
                    $"Encoding=UNICODE");
            }
            throw new Exception("The connection string is undefined.");
        }

        private string? GetLanguage()
        {
            var userLanguage = _sessionDataStorageService.SessionData?.Language ?? _httpContextAccessor?.HttpContext?.Request.GetDefaultUserLanguage();

            var supportedLanguages = new string[] { "pt-BR", "pt-PT", "en-US", "es-ES" };

            userLanguage = supportedLanguages.Any(x => x.Equals(userLanguage, StringComparison.InvariantCultureIgnoreCase))
                ? (userLanguage?.ToLower().Replace("-", "_"))
                : DefaultLanguage;

            return userLanguage;
        }
    }
}
