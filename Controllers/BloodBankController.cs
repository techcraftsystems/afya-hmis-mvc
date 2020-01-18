using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AfyaHMIS.Controllers
{
    [Authorize]
    public class BloodBankController : Controller
    {
        [Route("/blood-bank/")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
