using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IRRRL.Web.Pages;

[Authorize]
public class HomeModel : PageModel
{
    public void OnGet()
    {
    }
}

