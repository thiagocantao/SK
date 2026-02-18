using SK.Report.Data;
using SK.Report.Models;

namespace SK.Report.Middlewares
{
    public class SessionDataStorageMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionDataStorageMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, AppDbContext appDbContext, ILogger<SessionDataStorageMiddleware> logger)
        {
            if (context.Request.Path.HasValue && context.Request.Path.Value.Contains("DashboardDesigner"))
            {
                var ip = context.Connection.RemoteIpAddress?.ToString() ?? throw new Exception();
                var workspaceId = context.Request.Query["workspaceID"].First();
                var editMode = context.Request.Query["editMode"].FirstOrDefault() ?? "no";
                var userId = context.Request.Query["userID"].First();
                var objectId = context.Request.Query["objectID"].First();
                var objectType = context.Request.Query["objectType"].First();
                var language = context.Request.Query["language"].First();
                var sessao = new Sessao(ip, workspaceId, editMode, userId, objectId, objectType, language);

                if (appDbContext.Sessoes.Any(x => x.Ip == sessao.Ip))
                    appDbContext.Update(sessao);
                else
                    appDbContext.Sessoes.Add(sessao);

                await appDbContext.SaveChangesAsync();
            }

            else if (context.Request.Path.HasValue && context.Request.Path.Value.Contains("ReportDesigner"))
            {
                var ip = context.Connection.RemoteIpAddress?.ToString() ?? throw new Exception();
                var workspaceId = context.Request.Query["workspaceID"].First();
                var editMode = context.Request.Query["editMode"].FirstOrDefault() ?? "no";
                var userId = context.Request.Query["userID"].First();
                var objectId = context.Request.Query["objectID"].First();
                var objectType = context.Request.Query["objectType"].First();
                var language = context.Request.Query["language"].First();
                var sessao = new Sessao(ip, workspaceId, editMode, userId, objectId, objectType, language);

                if (appDbContext.Sessoes.Any(x => x.Ip == sessao.Ip))
                    appDbContext.Update(sessao);
                else
                    appDbContext.Sessoes.Add(sessao);

                await appDbContext.SaveChangesAsync();
            }

            await _next(context);
        }
    }

    public static class SessionDataStorageMiddlewareExtensions
    {
        public static IApplicationBuilder UseSessionDataStorage(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SessionDataStorageMiddleware>();
        }
    }
}
