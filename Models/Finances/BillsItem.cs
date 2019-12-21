using System;
using AfyaHMIS.Service;

namespace AfyaHMIS.Models.Finances
{
    public class BillsItem
    {
        public long Id { get; set; }
        public Bills Bill { get; set; }
        public BillableService Service { get; set; }
        public double Amount { get; set; }
        public DateTime CreatedOn { get; set; }
        public Users CreatedBy { get; set; }
        public string Description { get; set; }

        public BillsItem()
        {
            Id = 0;
            Bill = new Bills();
            Service = new BillableService();
            Amount = 0;
            Description = "";
            CreatedOn = DateTime.Now;
            CreatedBy = new Users();
        }

        public BillsItem Save()
        {
            return new FinanceService().SaveBillsItem(this);
        }
    }
}
