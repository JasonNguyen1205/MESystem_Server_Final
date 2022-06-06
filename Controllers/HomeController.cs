using Microsoft.AspNetCore.Mvc;

namespace MESystem.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public ActionResult CldrData()
        {
            return new DevExtreme.AspNet.Mvc.CldrDataScriptBuilder()
                .SetCldrPath("~/wwwroot/cldr-data")
                .SetInitialLocale("de")
                .UseLocales("de", "es")
                .Build();
        }
    }
}
