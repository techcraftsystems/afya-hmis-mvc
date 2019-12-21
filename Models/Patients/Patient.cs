using System;
using AfyaHMIS.Models.Persons;
using AfyaHMIS.Service;

namespace AfyaHMIS.Models.Patients
{
    public class Patient
    {
        private static readonly IPatientService IService = new PatientService();

        public long Id { get; set; }
        public string Uuid { get; set; }
        public string Identifier { get; set; }
        public string Age { get; set; }
        public Person Person { get; set; }
        public PatientStatus Status { get; set; }
        public PatientIdentifier PI { get; set; }
        public Users AddedBy { get; set; }
        public DateTime AddedOn { get; set; }
        public string Notes { get; set; }
        public DateTime LastDate { get; set; } 
        public string LastVisit { get; set; }

        public Patient() {
            Id = 0;
            Uuid = "";
            Age = "";
            Identifier = "";
            PI = new PatientIdentifier();
            Person = new Person();
            Status = new PatientStatus();
            AddedBy = new Users();
            AddedOn = DateTime.Now;
            Notes = "";
            LastVisit = "";
            LastDate = DateTime.Now;
        }

        public string GetAge() {
            Age = Person.GetAge();
            return Age;
        }

        public int GetAgeInYears() {
            return Person.GetAgeInYears();
        }

        public string GetUuid() {
            if (string.IsNullOrEmpty(Uuid))
                Uuid = new PatientService().GetPatientUuid(this);
            return Uuid;
        }

        public Patient Save() {
            Person.Save();
            IService.SavePatient(this);

            PI.Patient = this;
            PI.Save();
            
            return this;
        }

        public Patient UpdateStatus() {
            return IService.UpdatePatientStatus(this);
        }
    }
}
