using System;
using AfyaHMIS.Service;

namespace AfyaHMIS.Models.Finances
{
    public class InvoiceDetails
    {
        private IFinanceService IFinanceService = new FinanceService();

        public long Id { get; set; }
        public bool Checked { get; set; }
        public Invoice Invoice { get; set; }
        public BillsItem Item { get; set; }
        public DateTime CreatedOn { get; set; }
        public Users CreatedBy { get; set; }
        public string Notes { get; set; }

        public InvoiceDetails() {
            Id = 0;
            Checked = false;
            Item = new BillsItem();
            Invoice = new Invoice();
            CreatedBy = new Users();
            CreatedOn = DateTime.Now;
            Notes = "";
        }

        public InvoiceDetails Save() {
            return IFinanceService.SaveInvoiceDetails(this);
        }

        public void Remove()
        {
            IFinanceService.RemoveInvoiceDetail(this);
        }
    }
}
