using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using RazorPageEntraId.WebApiEntraId;
using System.Threading.Tasks;

namespace RazorPageEntraId.Pages;

[AuthorizeForScopes(Scopes = ["api://b2a09168-54e2-4bc4-af92-a710a64ef1fa/access_as_user"])]
public class CallApiModel : PageModel
{
    private readonly WebApiEntraIdService _apiService;

    [BindProperty]
    public byte[] Photo { get; set; } = [];

    public CallApiModel(WebApiEntraIdService apiService)
    {
        _apiService = apiService;
    }

    public async Task OnGetAsync()
    {
        var photo = await _apiService.GetWebApiEntraIdDataAsync();
        if (!string.IsNullOrEmpty(photo))
        {
            Photo = Base64UrlEncoder.DecodeBytes(photo);
        }
    }
}