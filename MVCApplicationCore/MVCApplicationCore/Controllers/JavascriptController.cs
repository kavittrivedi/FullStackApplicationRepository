using Microsoft.AspNetCore.Mvc;

namespace MVCApplicationCore.Controllers
{
    public class JavascriptController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
