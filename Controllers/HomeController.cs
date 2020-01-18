using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AfyaHMIS.Models;
using Microsoft.AspNetCore.Authorization;
using AfyaHMIS.ViewModel;
using AfyaHMIS.Service;

namespace AfyaHMIS.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICoreService ICoreService;

        public HomeController(ICoreService icservice, ILogger<HomeController> logger) {
            _logger = logger;
            ICoreService = icservice;
        }

        public IActionResult Index(HomeIndexViewModel model)
        {
            model.Modules = ICoreService.GetApplicationModules();

            return View(model);
        }

        public IActionResult Privacy() {
            return View();
        }

        [Route("/notifications")]
        public IActionResult Notifications() {
            return View();
        }

        [Route("/appointments")]
        public IActionResult Appointments()
        {
            return View();
        }

        [Route("/settings")]
        public IActionResult Settings()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
