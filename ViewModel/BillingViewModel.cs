using System;
using System.Collections.Generic;
using AfyaHMIS.Models.Finances;
using AfyaHMIS.Models.Patients;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AfyaHMIS.ViewModel
{
    public class BillingCashierViewModel
    {
        public string Message { get; set; }
        public string Date { get; set; }
        public List<Bills> Bills { get; set; }

        public BillingCashierViewModel()
        {
            Message = "";
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
        public string Amount { get; set; }
        public string Message { get; set; }
        public Invoice Invoice { get; set; }
        public InvoicePayment Payment { get; set; }
        public List<InvoiceDetails> Details { get; set; }
        public List<InvoicePaymentDetails> Tendered { get; set; }
        public List<InvoicePaymentDetails> Payments { get; set; }
        public List<SelectListItem> Modes { get; set; }

        public BillingInvoiceViewModel() {
            Amount = "Zero";
            Message = "";
            Invoice = new Invoice();
            Payment = new InvoicePayment();
            Details = new List<InvoiceDetails>();
            Tendered = new List<InvoicePaymentDetails>();
            Payments = new List<InvoicePaymentDetails>();

            //Initialize Items
            for (int i = 0; i < 20; i++) {
                Payments.Add(new InvoicePaymentDetails());
            }

            Modes = new List<SelectListItem>();
        }
    }
}
