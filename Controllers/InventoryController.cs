using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace AfyaHMIS.Controllers
{
    [Authorize]
    public class InventoryController : Controller
    {
        [Route("/inventory")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("/inventory/drugs")]
        public IActionResult Drugs()
        {
            return View();
        }

        [Route("/inventory/non-pharma")]
        public IActionResult NonPharma()
        {
            return View();
        }
    }
}
