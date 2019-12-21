using System;
namespace AfyaHMIS.Models.Patients
{
    public class PatientStatus
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public PatientStatus()
        {
            Id = 0;
            Name = "";
        }
    }
}
