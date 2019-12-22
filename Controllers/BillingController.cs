using System;
using System.Globalization;
using AfyaHMIS.Models.Finances;
using AfyaHMIS.Service;
using AfyaHMIS.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AfyaHMIS.Controllers
{
    [Authorize]
    public class BillingController : Controller
    {
        private readonly IFinanceService IFinanceService;

        public BillingController(IFinanceService finance)
        {
            IFinanceService = finance;
        }

        [Route("/billing/cashier/")]
        public IActionResult Cashier(BillingCashierViewModel model) {
            model.Bills = IFinanceService.GetBillingCashierQueue(DateTime.Now, DateTime.Now, new BillsFlag());
            return View(model);
        }

        public JsonResult GetBillingCashierQueue(string start, string stop, string flag = "")
        {
            BillsFlag bf = null;
            if (!string.IsNullOrEmpty(flag) && long.TryParse(flag, out long dblFlag)) {
                bf = new BillsFlag {
                    Id = dblFlag
                };
            }

            return Json(IFinanceService.GetBillingCashierQueue(DateTime.ParseExact(start, "dd/MM/yyyy", CultureInfo.InvariantCulture), DateTime.ParseExact(stop, "dd/MM/yyyy", CultureInfo.InvariantCulture), bf));
        }
    }
}
