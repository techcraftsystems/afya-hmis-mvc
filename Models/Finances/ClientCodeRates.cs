using System;
namespace AfyaHMIS.Models.Finances
{
    public class ClientCodeRates
    {
        public ClientCode Code { get; set; }
        public BillableService Service { get; set; }
        public double Amount { get; set; }

        public ClientCodeRates()
        {
            Code = new ClientCode();
            Service = new BillableService();
            Amount = 0;
        }
    }
}
