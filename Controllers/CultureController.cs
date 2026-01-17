using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace AiLearningLab.Controllers;

[Route("[controller]/[action]")]
public class CultureController : Controller
{
    public IActionResult Set(string culture, string returnUrl = "/")
    {
        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
        );

        // Return a page that forces hard refresh via JavaScript
        return Content($@"
<!DOCTYPE html>
<html>
<head><meta charset=""utf-8""></head>
<body>
<script>window.location.replace('{returnUrl}');</script>
</body>
</html>", "text/html");
    }
}
