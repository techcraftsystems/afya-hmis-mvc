using System;
using AfyaHMIS.Models.Patients;
using AfyaHMIS.Service;

namespace AfyaHMIS.Models.Registrations
{
    public class Encounter
    {
        public long Id { get; set; }
        public Visit Visit { get; set; }
        public Patient Patient { get; set; }
        public EncounterType Type { get; set; }
        public DateTime CreatedOn { get; set; }
        public Users CreatedBy { get; set; }
        public string Notes { get; set; }

        public Encounter()
        {
            Id = 0;
            Visit = new Visit();
            Patient = new Patient();
            Type = new EncounterType();
            CreatedOn = DateTime.Now;
            CreatedBy = new Users();
            Notes = "";
        }

        public Encounter Create(IPatientService IPatientService) {
            return IPatientService.CreateEncounter(this);
        }
    }
}
