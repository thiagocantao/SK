namespace SK.Report.Services
{
    public class DatabaseSettingsService
    {
        private readonly IConfigurationSection _section;

        public DatabaseSettingsService(IConfiguration configuration)
        {
            _section = configuration.GetSection("SmartKanvasSettings:DatabaseSettings");
        }

        public string GetServerName() =>
            _section.GetValue<string>("Host");

        public string GetDatabaseName() =>
            _section.GetValue<string>("Database");

        public string? GetUserByLanguage(string? language) =>
            _section.GetValue<string>($"Schemas:{language}:User");

        public string? GetPasswordByLanguage(string? language) =>
            _section.GetValue<string>($"Schemas:{language}:Password");
    }
}
