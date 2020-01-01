using System;
using AfyaHMIS.Service;

namespace AfyaHMIS.Models.Finances
{
    public class InvoicePaymentDetails
    {
        private IFinanceService IFinanceService = new FinanceService();

        public long Id { get; set; }
        public InvoicePayment Payment { get; set; }
        public BillingMode Mode { get; set; }
        public double Amount { get; set; }
        public string Reference { get; set; }
        public string Account { get; set; }
        public string Notes { get; set; }

        public InvoicePaymentDetails() {
            Id = 0;
            Payment = new InvoicePayment();
            Mode = new BillingMode();
            Amount = 0;
            Reference = "";
            Account = "";
            Notes = "";
        }

        public InvoicePaymentDetails Save() {
            return IFinanceService.SaveInvoicePaymentDetails(this);
        }
    }
}
