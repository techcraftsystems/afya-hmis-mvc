using System;
using AfyaHMIS.Models.Persons;

namespace AfyaHMIS.Models.Doctors
{
    public class Doctor
    {
		public long Id { get; set; }
        public bool Void { get; set; }
        public Person Person { get; set; }
        public string Notes { get; set; }

		public Doctor()
        {
            Id = 0;
            Void = false;
            Person = new Person();
            Notes = "";
        }
    }
}
