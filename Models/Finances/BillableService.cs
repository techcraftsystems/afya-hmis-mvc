using System;
using AfyaHMIS.Models.Concepts;

namespace AfyaHMIS.Models.Finances
{
    public class BillableService
    {
        public long Id { get; set; }
        public Concept Concept { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; }

        public BillableService()
        {
            Id = 0;
            Concept = new Concept();
            Code = "0000";
            Name = "";
            Amount = 0;
            Description = "";
        }
    }
}
