using System;
using System.Collections.Generic;
using AfyaHMIS.Models.Finances;

namespace AfyaHMIS.ViewModel
{
    public class BillingCashierViewModel
    {
        public string Date { get; set; }
        public List<Bills> Bills { get; set; }

        public BillingCashierViewModel()
        {
            Date = DateTime.Now.ToString("dd/MM/yyyy");
            Bills = new List<Bills>();
        }
    }
}
