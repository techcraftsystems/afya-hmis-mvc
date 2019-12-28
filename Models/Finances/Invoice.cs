using System;
using AfyaHMIS.Models.Patients;
using AfyaHMIS.Service;

namespace AfyaHMIS.Models.Finances
{
    public class Invoice
    {
        private IFinanceService IFinanceService = new FinanceService();

        public long Id { get; set; }
        public string Date { get; set; }
        public Patient Patient { get; set; }
        public BillingFlag Flag { get; set; }
        public double Amount { get; set; }
        public double Paid { get; set; }
        public double Balance { get; set; }
        public Users CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Notes { get; set; }

        public Invoice()
        {
            Id = 0;
            Patient = new Patient();
            Flag = new BillingFlag();
            Amount = 0;
            Paid = 0;
            Balance = 0;
            CreatedBy = new Users();
            CreatedOn = DateTime.Now;
            Notes = "";
        }

        public Invoice Save() {
            return IFinanceService.SaveInvoice(this);
        }
    }
}
