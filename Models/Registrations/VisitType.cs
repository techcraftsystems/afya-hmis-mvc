using System;
namespace AfyaHMIS.Models.Registrations
{
    public class VisitType
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public VisitType() {
            Id = 0;
            Name = "";
        }
    }
}
