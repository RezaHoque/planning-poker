using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PlanningPoker.Pages
{
    public class CommercialSupportModel : PageModel
    {
        public void OnGet()
        {
            ViewData["Title"] = "Commercial Support";
        }
    }
}
