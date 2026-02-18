using Newtonsoft.Json;
using RestSharp;
using SK.Report.Dto;
using SK.Report.Extensions;

namespace SK.Report.Services
{
    public class ApiIntegrationService
    {
        const string ApiPath = "/api/1.1/wf/show-skreport";

        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public ApiIntegrationService(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }

        public async Task<ReportDataResponse?> GetReportInfo(ReportDataRequest reportDataRequest)
        {
            using var client = new RestClient(GetApiUri());
            var request = new RestRequest();
            request.AddJsonBody(JsonConvert.SerializeObject(reportDataRequest));
            try
            {
                return await client.PostAsync<ReportDataResponse>(request);
            }
            catch
            {
                return new ReportDataResponse
                {
                    Status = "error",
                    Response = new ReportDataResponse.ResponseData
                    {
                        StatusID = 500,
                        StatusTitle = "Não foi possível validar o token"
                    }
                };
            }
        }

        private Uri GetApiUri()
        {
            var baseUri = new Uri(_configuration.GetValue<string>("SmartKanvasSettings:Url"));
            var testVersion = _environment.IsDevelopment() ? GetUriTestEnvironmentComplement() : string.Empty;
            var skApiUri = baseUri.Append(testVersion, ApiPath);
            return skApiUri;
        }

        public string GetUriTestEnvironmentComplement()
        {
            return _configuration.GetValue<string>("SmartKanvasSettings:UriTestEnvironmentComplement");
        }
    }
}
