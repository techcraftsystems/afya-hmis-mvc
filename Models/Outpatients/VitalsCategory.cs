using System;
namespace AfyaHMIS.Models.Outpatients
{
    public class VitalsCategory
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public VitalsCategory() {
            Id = 0;
            Name = "";
        }
    }
}
