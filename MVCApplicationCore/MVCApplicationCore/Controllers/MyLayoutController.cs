using Microsoft.AspNetCore.Mvc;

namespace MVCApplicationCore.Controllers
{
    public class MyLayoutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
