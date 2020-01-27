using System.Collections.Generic;
using AfyaHMIS.Models.Finances;
using AfyaHMIS.Models.Patients;
using AfyaHMIS.Models.Registrations;
using AfyaHMIS.Models.Rooms;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AfyaHMIS.ViewModel
{
    public class RegistrationRegisterViewModel
    {
        public RegistrationRegisterViewModel()
        {
        }
    }

    public class RegistrationNewEditViewModel
    {
        public string DateOfBirth { get; set; }
        public Patient Patient { get; set; }
        public IEnumerable<SelectListItem> Gender { get; set; }
        public IEnumerable<SelectListItem> IdType { get; set; }

        public RegistrationNewEditViewModel() {
            DateOfBirth = "";
            Patient = new Patient();
            Gender = new List<SelectListItem> {
                new SelectListItem {Value = "f", Text = "Female" },
                new SelectListItem {Value = "m", Text = "Male" },
                new SelectListItem {Value = "o", Text = "Others" },
            };
            IdType = new List<SelectListItem>();
        }
    }

    public class RegistrationVisitViewModel
    {
        public Visit Visit { get; set; }
        public Bills  Bill { get; set; }
        public BillsItem Item { get; set; }
        public Queues Queue { get; set; }
        public Referral Referral { get; set; }
        public string Doctor { get; set; }
        public string DoctorString { get; set; }
        public bool Waiver { get; set; }
        public string WaiverReason { get; set; }

        public IEnumerable<SelectListItem> Codes { get; set; }
        public IEnumerable<SelectListItem> Rooms { get; set; }
        public IEnumerable<SelectListItem> Types { get; set; }
        public IEnumerable<SelectListItem> Doctors { get; set; }
        public IEnumerable<SelectListItem> Referrals { get; set; }
        public IEnumerable<SelectListItem> MedicoLegal { get; set; }

        public RegistrationVisitViewModel() {
            Visit = new Visit();
            Bill = new Bills();
            Item = new BillsItem();
            Referral = new Referral();

            Codes = new List<SelectListItem>();
            Rooms = new List<SelectListItem>();
            Types = new List<SelectListItem>();
            Doctors = new List<SelectListItem>();

            Referrals = new List<SelectListItem>();
            MedicoLegal = new List<SelectListItem>();

            DoctorString = "";
            Doctor = "";
            Waiver = false;
            WaiverReason = "";
        }
    }

    public class RegistrationPateintsModel
    {

    }

    public class RegistrationPateintsViewModel
    {
        public Patient Patient { get; set; }

        public RegistrationPateintsViewModel() {
            Patient = new Patient();
        }
    }
}
