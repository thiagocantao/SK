using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SK.Report.Dto;
using SK.Report.Services;
using SK.Report.Utils;
using System.Text.Json;

namespace SK.Report.Pages;

public class Index : PageModel
{
    private readonly ApiIntegrationService _apiIntegrationService;
    private readonly ILogger<Index> _logger;

    [FromQuery(Name = "EditMode")]
    public string? EditMode { get; set; }

    [FromQuery(Name = "ObjectID")]
    public string ObjectID { get; set; } = null!;

    [FromQuery(Name = "ObjectType")]
    public string ObjectType { get; set; } = null!;

    public Index(ApiIntegrationService apiIntegrationService, ILogger<Index> logger)
    {
        _apiIntegrationService = apiIntegrationService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGet(string token, string reportId)
    {
        var reportDataRequest = new ReportDataRequest(
            token,
            SmartKanvasCredentials.User,
            SmartKanvasCredentials.Password,
            ObjectID,
            ObjectType);
        var reportInfo = await _apiIntegrationService.GetReportInfo(reportDataRequest);

        if (reportInfo is not null && reportInfo.Response is not null && reportInfo.StatusOk)
        {
            var responseData = reportInfo.Response;
            var routeValues = new
            {
                userID = responseData.UserID,
                workspaceID = responseData.WorkspaceID,
                reportID = reportId,
                editMode = EditMode ?? "no",
                language = responseData.Language,
                statusTitle = responseData.StatusTitle,
                objectID = responseData.ObjectID,
                objectType = responseData.ObjectType
            };
            _logger.LogTrace($"LOG - Index: {JsonSerializer.Serialize(routeValues)}");
            return base.RedirectToPage("DashboardDesigner", routeValues);
        }

        return RedirectToPage("TokenError", new { errorMessage = $"Mensgem: {reportInfo?.Response?.StatusTitle} - Token: {token}" });
    }
}
