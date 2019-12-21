using System;
using AfyaHMIS.Models.Doctors;
using AfyaHMIS.Service;

namespace AfyaHMIS.Models.Registrations
{
    public class ReferralDoctors
    {
        public long Id { get; set; }
        public Referral Referral { get; set; }
        public Doctor Doctor { get; set; }
        public string Notes { get; set; }
 
        public ReferralDoctors()
        {
            Id = 0;
            Referral = new Referral();
            Doctor = new Doctor();
            Notes = "";
        }

        public ReferralDoctors Save()
        {
            return new PatientService().SaveReferralDoctors(this);
        }
    }
}
