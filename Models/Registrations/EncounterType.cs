using System;
namespace AfyaHMIS.Models.Registrations
{
    public class EncounterType
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public EncounterType() {
            Id = 0;
            Name = "";
        }
    }
}
