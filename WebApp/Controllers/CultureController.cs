using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    public class CultureController : Controller
    {
        [HttpGet]
        public IActionResult Set(string culture, string returnUrl = null)
        {
            if (string.IsNullOrEmpty(culture))
            {
                return LocalRedirect(returnUrl ?? "/");
            }

            var supported = new[] { "ru", "kk" };
            if (!supported.Contains(culture))
            {
                return BadRequest("Неподдерживаемая культура");
            }

            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,  
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture, culture)),
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1),
                    HttpOnly = true,
                    Secure = Request.IsHttps,        
                    SameSite = SameSiteMode.Lax,
                    Path = "/"
                });

            return LocalRedirect(returnUrl ?? "/");
        }
    }
}