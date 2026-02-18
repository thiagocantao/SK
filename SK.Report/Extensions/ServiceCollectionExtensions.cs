using DevExpress.AspNetCore.Reporting;
using DevExpress.AspNetCore.Reporting.ReportDesigner.Native;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWeb;
using DevExpress.DataAccess.Sql;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Web.Extensions;
using DevExpress.XtraReports.Web.ReportDesigner;
using DevExpress.XtraReports.Web.ReportDesigner.Native;
using Microsoft.Extensions.DependencyInjection;
using SK.Report.Utils.DashboardsConfiguration;
using SK.Report.Utils.ReportsConfiguration;

namespace SK.Report.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDashboardConfigurator(this IServiceCollection services)
        {
            services.AddScoped<DashboardStorage>();
            services.AddScoped<ConnectionInterceptor>();
            services.AddScoped<CustomDataSourceWizardConnectionStringsProvider>();
            return services.AddScoped((serviceProvider) =>
            {
                var configurator = new DashboardConfigurator();

                var provider = serviceProvider.GetRequiredService<CustomDataSourceWizardConnectionStringsProvider>();
                configurator.SetConnectionStringsProvider(provider);

                var storage = serviceProvider.GetRequiredService<DashboardStorage>();
                configurator.SetDashboardStorage(storage);

                var dataSourceStorage = new DataSourceInMemoryStorage();

                var pgSqlDataSource = new DashboardSqlDataSource("Fonte de dados",
                    CustomDataSourceWizardConnectionStringsProvider.ConnectionName);
                pgSqlDataSource.DataProcessingMode = DataProcessingMode.Client;
                dataSourceStorage.RegisterDataSource("pgSqlDataSource", pgSqlDataSource.SaveToXml());

                configurator.SetDataSourceStorage(dataSourceStorage);

                var connectionInterceptor = serviceProvider.GetRequiredService<ConnectionInterceptor>();
                configurator.SetDBConnectionInterceptor(connectionInterceptor);

                return configurator;
            });
        }

        public static IServiceCollection AddReportDesignerConfigurator(this IServiceCollection services)
        {
            services.AddScoped<ReportStorageWebExtension, ReportDesignerStorage>();
            services.AddScoped<IDBConnectionInterceptor, ConnectionInterceptor>();

            return services.ConfigureReportingServices((builder) => {
                builder.ConfigureReportDesigner(designer => {
                    designer.RegisterDataSourceWizardConnectionStringsProvider<CustomDataSourceWizardConnectionStringsProvider>();
                });
            });
        }
    }
}
