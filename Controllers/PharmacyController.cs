using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AfyaHMIS.Controllers
{
    [Authorize]
    public class PharmacyController : Controller
    {
        [Route("/pharmacy")]
        public IActionResult Index() {
            return View();
        }
    }
}
