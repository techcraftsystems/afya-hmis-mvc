using System;
using AfyaHMIS.Service;

namespace AfyaHMIS.Models.Persons
{
    public class Person
    {
        private static readonly IPatientService IService = new PatientService();

        public long Id { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public bool Estimate { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime AddedOn { get; set; }
        public Users AddedBy { get; set; }
        public PersonAddress Address { get; set; }
        public string Notes { get; set; }

        public Person() {
            Id = 0;
            Name = "";
            Gender = "";
            Estimate = false;
            DateOfBirth = DateTime.Now;
            AddedOn = DateTime.Now;
            AddedBy = new Users();
            Address = new PersonAddress();
            Notes = "";
        }

        public string GetAge() {
            int age = DateTime.Now.Year - DateOfBirth.Year;
            if (DateOfBirth > DateTime.Now.AddYears(-age)) age--;

            int mnth = Convert.ToInt32(DateTime.Now.Subtract(DateOfBirth).Days / (365.25 / 12));
            int days = Convert.ToInt32((DateTime.Now - DateOfBirth).TotalDays);

            if (age > 2)
                return age + "yrs";
            if (mnth > 2)
                return mnth + "mnths";
            if (days == 1)
                return "1 day";
            return days + "days";
        }

        public int GetAgeInYears() {
            int age = DateTime.Now.Year - DateOfBirth.Year;
            if (DateOfBirth > DateTime.Now.AddYears(-age)) age--;

            return age;
        }

        public Person Save() {
            IService.SavePerson(this);

            Address.Person = this;
            Address.Save();
            return this;
        }
    }
}
