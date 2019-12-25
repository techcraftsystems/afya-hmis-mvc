using System;
using AfyaHMIS.Service;

namespace AfyaHMIS.Models.Finances
{
    public class BillsItem
    {
        private static IFinanceService IFinanceService = new FinanceService();

        public long Id { get; set; }
        public Bills Bill { get; set; }
        public BillableService Service { get; set; }
        public double Quantity { get; set; }
        public double Price { get; set; }
        public DateTime CreatedOn { get; set; }
        public Users CreatedBy { get; set; }
        public string Description { get; set; }
        public bool Voided { get; set; }
        public Users VoidedBy { get; set; }
        public DateTime? VoidedOn { get; set; }
        public string VoidedReason { get; set; }

        public BillsItem() {
            Id = 0;
            Bill = new Bills();
            Service = new BillableService();
            Quantity = 0;
            Price = 0;
            Description = "";
            CreatedOn = DateTime.Now;
            CreatedBy = new Users();
            Voided = false;
            VoidedReason = "";
            VoidedOn = DateTime.Now;
            VoidedBy = new Users();
        }

        public BillsItem Void() {
            return IFinanceService.VoidBillsItem(this);
        }

        public BillsItem Save() {
            return IFinanceService.SaveBillsItem(this);
        }
    }
}
