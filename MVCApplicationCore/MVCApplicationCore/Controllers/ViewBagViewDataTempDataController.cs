using Microsoft.AspNetCore.Mvc;
using MVCApplicationCore.Data;

namespace MVCApplicationCore.Controllers
{
    [Route("transferdata")]
    public class ViewBagViewDataTempDataController : Controller
    {
        private AppDbContext _context;

        public ViewBagViewDataTempDataController(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }

        [HttpGet("viewbagexample")]
        public IActionResult Index()
        {
            ViewBag.Kavit = "Hello, Kavit from viewbag!";
            return View();
        }

        [HttpGet("viewdata")]
        public IActionResult ViewDataExample()
        {
            ViewData["Message"] = "Hello Kavit, from ViewData";
            return View();
        }

        [HttpGet("viewdatatypecasting")]
        public IActionResult ViewDataExample1()
        {
            var categories = _context.Categories.ToList();
            ViewData["categories"] = categories;
            return View();
        }

        [HttpGet("tempdata")]
        public IActionResult TempDataExample()
        {
            TempData["Message"] = "Hello Kavit, from TempData!";
            return RedirectToAction("NewAction");
        }

        [HttpGet("NewAction")]
        public IActionResult NewAction()
        {
            return View();
        }
    }
}
