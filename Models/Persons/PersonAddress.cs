using System;
using AfyaHMIS.Service;

namespace AfyaHMIS.Models.Persons
{
    public class PersonAddress
    {
        private static readonly IPatientService IService = new PatientService();

        public long Id { get; set; }
        public bool Default { get; set; }
        public Person Person { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public string Location { get; set; }
        public DateTime AddedOn { get; set; }
        public Users AddedBy { get; set; }
        public string Notes { get; set; }

        public PersonAddress() {
            Id = 0;
            Default = true;
            Telephone = "";
            Email = "";
            Location = "";
            AddedOn = DateTime.Now;
            AddedBy = new Users();
            Notes = "";
        }

        public PersonAddress Save() {
            return IService.SavePersonAddress(this);
        }
    }
}
