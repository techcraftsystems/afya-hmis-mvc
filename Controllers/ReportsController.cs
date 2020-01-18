using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AfyaHMIS.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        [Route("/reports")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
