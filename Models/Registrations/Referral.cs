using System;
using AfyaHMIS.Service;

namespace AfyaHMIS.Models.Registrations
{
    public class Referral
    {
        public long Id { get; set; }
        public Visit Visit { get; set; }
        public ReferralType Type { get; set; }
        public string Facility { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedOn { get; set; }
        public Users CreatedBy { get; set; }

        public Referral()
        {
            Id = 0;
            Visit = new Visit();
            Type = new ReferralType();
            Facility = "";
            Notes = "";
            CreatedOn = DateTime.Now;
            CreatedBy = new Users();
        }

        public Referral Save()
        {
            return new PatientService().SaveReferral(this);
        }
    }
}
