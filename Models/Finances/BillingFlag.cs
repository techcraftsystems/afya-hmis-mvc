using System;
namespace AfyaHMIS.Models.Finances
{
    public class BillingFlag
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public BillingFlag()
        {
            Id = 0;
            Name = "";
        }
    }
}
