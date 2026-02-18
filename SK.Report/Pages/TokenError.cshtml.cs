using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SK.Report.Pages
{
    public class TokenErrorModel : PageModel
    {
        [FromQuery(Name = "errorMessage")]
        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
        }
    }
}
