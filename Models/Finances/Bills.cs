using System;
using AfyaHMIS.Models.Administrations;
using AfyaHMIS.Models.Registrations;
using AfyaHMIS.Service;

namespace AfyaHMIS.Models.Finances
{
    public class Bills
    {
        public static IFinanceService IFinanceService = new FinanceService();

        public long Id { get; set; }
        public string Date { get; set; }
        public Visit Visit { get; set; }
        public BillingFlag Flag { get; set; }
        public Department Department { get; set; }
        public double Amount { get; set; }
        public double Paid { get; set; }
        public double Waiver { get; set; }
        public double Balance { get; set; }
        public string WaiverReason { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime? WaivedOn { get; set; }
        public DateTime? ProcessedOn { get; set; }
        public DateTime? WaivedApprovedOn { get; set; }

        public Users CreatedBy { get; set; }
        public Users WaivedBy { get; set; }
        public Users ProcessedBy { get; set; }
        public Users WaivedApprovedBy { get; set; }
        public string Notes { get; set; }

        public Bills() {
            Id = 0;
            Date = DateTime.Now.ToString("dd/MM/yyyy");
            Visit = new Visit();
            Flag = new BillingFlag();
            Department = new Department();
            Amount = 0;
            Paid = 0;
            Waiver = 0;
            Balance = 0;
            WaiverReason = "";

            CreatedOn = DateTime.Now;

            CreatedBy = new Users();
            WaivedBy = new Users();
            ProcessedBy = new Users();
            WaivedApprovedBy = new Users();
            Notes = "";
        }

        public bool IsProcessed() {
            return IFinanceService.GetBillProcessingStatus(this);
        }

        public Bills Save() {
            return IFinanceService.SaveBill(this);
        }

        public Bills UpdateWaiver() {
            return IFinanceService.UpdateBillWaiver(this);
        }

        public Bills UpdateProcess() {
            return IFinanceService.UpdateBillProcess(this);
        }

        public Bills SetAutoFlag() {
            return IFinanceService.UpdateBillsAutoFlag(this);
        }
    }
}
