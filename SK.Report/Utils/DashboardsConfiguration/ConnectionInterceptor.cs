using DevExpress.DataAccess.Sql;
using SK.Report.Models;
using SK.Report.Services;
using System.Data;

namespace SK.Report.Utils.DashboardsConfiguration
{
    public class ConnectionInterceptor : IDBConnectionInterceptor
    {
        private readonly SessionDataStorageService _sessionDataStorageService;
        private readonly Sessao? _sessao;

        public ConnectionInterceptor(SessionDataStorageService sessionDataStorageService)
        {
            _sessionDataStorageService = sessionDataStorageService;
            _sessao = _sessionDataStorageService.SessionData;
        }
        public void ConnectionOpened(string sqlDataConnectionName, IDbConnection connection)
        {
            using var command = connection.CreateCommand();
            command.CommandText = $"set application_name = '{_sessao?.WorkspaceID}';";
            command.ExecuteNonQuery();
        }

        public void ConnectionOpening(string sqlDataConnectionName, IDbConnection connection) { }
    }
}
