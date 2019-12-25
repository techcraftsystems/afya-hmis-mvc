using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using AfyaHMIS.Models;
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
        private readonly IPatientService IPatientService;

        public BillingController(IFinanceService finance, IPatientService patient) {
            IFinanceService = finance;
            IPatientService = patient;
        }

        [Route("/billing/cashier/")]
        public IActionResult Cashier(BillingCashierViewModel model) {
            model.Bills = IFinanceService.GetBillingCashierQueue(DateTime.Now, DateTime.Now, new BillsFlag());
            return View(model);
        }

        [Route("/billing/bill")]
        public IActionResult Bill(string p, string date, BillingBillViewModel model)
        {
            model.Patient = IPatientService.GetPatient(p);
            if (model.Patient == null)
                return LocalRedirect("/billing/cashier?error=1043");

            try {
                model.Date = DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            catch (Exception) {
                return LocalRedirect("/billing/cashier?error=4509");
            }

            model.Completed = IFinanceService.GetBills(model.Patient, null, null, null, null, true);
            model.Pending = IFinanceService.GetBills(model.Patient, null, null, null, new BillsFlag());
            model.Departments = IFinanceService.GetBillsDepartments(model.Patient, new BillsFlag());
            return View(model);
        }

        [Route("/billing/bill/process")]
        public IActionResult BillProcess(string p, string date, string bill, BillingBillViewModel model)
        {
            model.Patient = IPatientService.GetPatient(p);
            if (model.Patient == null)
                return LocalRedirect("/billing/cashier?error=1043");

            try {
                model.Date = DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            catch (Exception) {
                return LocalRedirect("/billing/cashier?error=4509");
            }

            model.Bills = bill;

            return View(model);
        }

        public bool CancelBillItems(int idnt, string items, string notes) {
            Bills bill = new Bills { Id = idnt };
            Users user = new Users { Id = long.Parse(HttpContext.User.FindFirst(ClaimTypes.Actor).Value) };

            List<string> Items = items.Split(new string[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (var id in Items) {
                BillsItem item = new BillsItem {
                    Id = Convert.ToInt64(id),
                    Bill = bill,
                    VoidedBy = user,
                    VoidedReason = notes
                };

                item.Void();
            }

            bill.ProcessedBy = user;
            bill.SetAutoFlag();

            return true;
        }

        public bool GetBillProcessingStatus(int idnt) {
            return new Bills { Id = idnt }.IsProcessed();
        }

        public JsonResult GetBillingCashierQueue(string start, string stop, string flag = "") {
            BillsFlag bf = null;
            if (!string.IsNullOrEmpty(flag) && long.TryParse(flag, out long dblFlag)) {
                bf = new BillsFlag {
                    Id = dblFlag
                };
            }

            return Json(IFinanceService.GetBillingCashierQueue(DateTime.ParseExact(start, "dd/MM/yyyy", CultureInfo.InvariantCulture), DateTime.ParseExact(stop, "dd/MM/yyyy", CultureInfo.InvariantCulture), bf));
        }

        public JsonResult GetBillsItems(int idnt, bool process = false, bool viod = true) {
            return Json(IFinanceService.GetBillsItems(new Bills { Id = idnt }, viod, process));
        }
    }
}
