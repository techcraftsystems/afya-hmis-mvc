using AfyaHMIS.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AfyaHMIS.Controllers
{
    [Authorize]
    public class BillingController : Controller
    {
        [Route("/billing/cashier/")]
        public IActionResult Cashier(BillingCashierViewModel model)
        {
            return View(model);
        }
    }
}
