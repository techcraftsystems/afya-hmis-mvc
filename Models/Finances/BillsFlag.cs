using System;
namespace AfyaHMIS.Models.Finances
{
    public class BillsFlag
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public BillsFlag()
        {
            Id = 0;
            Name = "";
        }
    }
}
