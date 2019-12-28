using System;
using System.Collections.Generic;
using AfyaHMIS.Models.Finances;
using AfyaHMIS.Models.Patients;

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

    public class BillingBillViewModel
    {
        public string Bills { get; set; }
        public DateTime Date { get; set; }
        public Patient Patient { get; set; }
        public Invoice Invoice { get; set; }

        public List<Bills> Completed { get; set; }
        public List<Bills> Pending { get; set; }
        public List<Invoice> Invoices { get; set; }
        public List<InvoiceDetails> Items { get; set; }
        public List<BillsDepartment> Departments { get; set; }

        public BillingBillViewModel()
        {
            Patient = new Patient();
            Invoice = new Invoice();

            Completed = new List<Bills>();
            Pending = new List<Bills>();
            Items = new List<InvoiceDetails>();
            Invoices = new List<Invoice>();
            Date = DateTime.Now;
            Bills = "";
            Departments = new List<BillsDepartment>();
        }
    }

    public class BillingInvoiceViewModel {
        public Invoice Invoice { get; set; }
        public List<InvoiceDetails> Details { get; set; }

        public BillingInvoiceViewModel() {
            Invoice = new Invoice();
            Details = new List<InvoiceDetails>();
        }
    }
}
