using System;
using AfyaHMIS.Service;

namespace AfyaHMIS.Models.Finances
{
    public class InvoicePayment
    {
        private IFinanceService IFinanceService = new FinanceService();

        public long Id { get; set; }
        public Invoice Invoice { get; set; }
        public Users CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public double Amount { get; set; }
        public string Notes { get; set; }


        public InvoicePayment() {
            Id = 0;
            Invoice = new Invoice();
            CreatedBy = new Users();
            CreatedOn = DateTime.Now;
            Amount = 0;
            Notes = "";
        }

        public InvoicePayment Save() {
            return IFinanceService.SaveInvoicePayment(this);
        }
    }
}
