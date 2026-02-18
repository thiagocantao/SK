using System.Collections;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SK.Report.Data;
using SK.Report.Services;

namespace SK.Report.Pages
{
    public class InfoModel : PageModel
    {
        private const string Language = "pt_br";
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ReportContext _context;
        private readonly DatabaseSettingsService _databaseSettingsService;

        public string? Ip => _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

        public string? Ip2 => _httpContextAccessor.HttpContext?.Request.Headers["X-Forwarded-For"].ToString();

        public string? Id => _context.Reports.FirstOrDefault()?.Id;

        public DateTime DataInicial { get; }

        public DateTime DataHora => DateTime.UtcNow;

        public Dictionary<string, string?> Variaveis
        {
            get
            {
                return Environment
                    .GetEnvironmentVariables()
                    .Cast<DictionaryEntry>()
                    .ToDictionary(entry => (string)entry.Key, entry => entry.Value as string);
            }
        }

        public string DbSchema { get => $"{_databaseSettingsService.GetUserByLanguage(Language)}:{_databaseSettingsService.GetPasswordByLanguage(Language)}"; }

        public string? CS { get; }

        public InfoModel(
            IHttpContextAccessor httpContextAccessor,
            ReportContext context,
            DatabaseSettingsService databaseSettingsService)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _databaseSettingsService = databaseSettingsService;
            DataInicial = DateTime.UtcNow;
            CS = _context.Database.GetConnectionString();
        }

        public void OnGet()
        {
        }
    }
}
