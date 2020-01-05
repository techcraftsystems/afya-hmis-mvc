using System;
using System.Collections.Generic;
using AfyaHMIS.Models.Outpatients;
using AfyaHMIS.Models.Patients;
using AfyaHMIS.Models.Rooms;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AfyaHMIS.ViewModel
{
    public class OutpatientQueueViewModel {
        public string Message { get; set; }
        public Room Room { get; set; }
        public string Date { get; set; }
        public List<Queues> Queue { get; set; }
        public List<SelectListItem> Types { get; set; }

        public OutpatientQueueViewModel() {
            Message = "";
            Room = new Room();
            Date = DateTime.Now.ToString("dd/MM/yyyy");
            Queue = new List<Queues>();
        }
    }

    public class OutpatientTriageViewVModel
    {
        public Patient Patient { get; set; }
        public Queues Queue { get; set; }
        public Queues SendTo { get; set; }
        public Triage Triage { get; set; }

        public IEnumerable<SelectListItem> MobilityOpts { get; set; }
        public IEnumerable<SelectListItem> AvpuOpts { get; set; }
        public IEnumerable<SelectListItem> TraumaOpts { get; set; }

        public IEnumerable<SelectListItem> Rooms { get; set; }
        public IEnumerable<SelectListItem> Types { get; set; }
        public IEnumerable<SelectListItem> Priority { get; set; }

        public OutpatientTriageViewVModel() {
            Patient = new Patient();
            Queue = new Queues();
            SendTo = new Queues();
            Triage = new Triage();

            MobilityOpts = new List<SelectListItem>();
            AvpuOpts = new List<SelectListItem>();
            TraumaOpts = new List<SelectListItem>();

            Rooms = new List<SelectListItem>();
            Types = new List<SelectListItem>();
            Priority = new List<SelectListItem>();
        }
    }

    public class OutpatientDoctorViewVModel
    {
        public OutpatientDoctorViewVModel() {
        }
    }
}
