using System;
namespace AfyaHMIS.Models.Patients
{
    public class PatientIdentifierType
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public PatientIdentifierType()
        {
            Id = 0;
            Name = "";
        }
    }
}
