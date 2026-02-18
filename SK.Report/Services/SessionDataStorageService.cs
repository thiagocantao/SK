using SK.Report.Data;
using SK.Report.Models;
using System;

namespace SK.Report.Services
{
    public class SessionDataStorageService
    {
        private Sessao? _sessao = null;

        private readonly AppDbContext _appDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionDataStorageService(AppDbContext appDbContext, IHttpContextAccessor httpContextAccessor)
        {
            _appDbContext = appDbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public Sessao? SessionData
        {
            get
            {
                if (_sessao == null)
                {
                    var ip = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
                    _sessao = _appDbContext.Sessoes.Find(ip);
                }
                return _sessao;
            }
        }
    }
}
