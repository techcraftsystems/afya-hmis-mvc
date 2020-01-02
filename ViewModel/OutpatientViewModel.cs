using System;
using System.Collections.Generic;
using AfyaHMIS.Models.Rooms;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AfyaHMIS.ViewModel
{
    public class OutpatientQueueViewModel {
        public Room Room { get; set; }
        public string Date { get; set; }
        public List<Queues> Queue { get; set; }
        public List<SelectListItem> Types { get; set; }

        public OutpatientQueueViewModel() {
            Room = new Room();
            Date = DateTime.Now.ToString("dd/MM/yyyy");
            Queue = new List<Queues>();
        }
    }

    public class OutpatientTriageViewVModel
    {
        public OutpatientTriageViewVModel() {
        }
    }

    public class OutpatientDoctorViewVModel
    {
        public OutpatientDoctorViewVModel() {
        }
    }
}
