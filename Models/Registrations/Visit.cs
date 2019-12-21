using System;
using AfyaHMIS.Models.Concepts;
using AfyaHMIS.Models.Finances;
using AfyaHMIS.Models.Patients;
using AfyaHMIS.Service;

namespace AfyaHMIS.Models.Registrations
{
    public class Visit
    {
        public long Id { get; set; }
        public Patient Patient { get; set; }
        public VisitType Type { get; set; }
        public ClientCode ClientCode { get; set; }
        public string SchemeNumber { get; set; }
        public string Barcode { get; set; }
        public Concept MedicoLegal { get; set; }
        public DateTime CreatedOn { get; set; }
        public Users CreatedBy { get; set; }
        public string Notes { get; set; }

        public Visit()
        {
            Id = 0;
            Patient = new Patient();
            Type = new VisitType();
            ClientCode = new ClientCode();
            SchemeNumber = "";
            Barcode = "";
            MedicoLegal = new Concept();
            CreatedOn = DateTime.Now;
            CreatedBy = new Users();
            Notes = "";
        }

        public Visit Save()
        {
            return new PatientService().SaveVisit(this);
        }
    }
}
