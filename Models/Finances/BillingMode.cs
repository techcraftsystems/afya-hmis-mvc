using System;
namespace AfyaHMIS.Models.Finances
{
    public class BillingMode
    {
        public long Id { get; set; }
        public bool Void { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }

        public BillingMode() {
            Id = 0;
            Void = false;
            Name = "";
            Title = "";
        }
    }
}
