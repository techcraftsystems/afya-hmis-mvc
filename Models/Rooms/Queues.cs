using System;
using AfyaHMIS.Models.Finances;
using AfyaHMIS.Models.Registrations;
using AfyaHMIS.Service;

namespace AfyaHMIS.Models.Rooms
{
    public class Queues
    {
        public long Id { get; set; }
        public QueuesPriority Priority { get; set; }
        public Visit Visit { get; set; }
        public BillsItem Item { get; set; }
        public Room Room { get; set; }
        public DateTime CreatedOn { get; set; }
        public Users CreatedBy { get; set; }
        public string Notes { get; set; }

        public Queues()
        {
            Id = 0;
            Priority = new QueuesPriority();
            Item = new BillsItem();
            Visit = new Visit();
            Room = new Room();
            CreatedBy = new Users();
            CreatedOn = DateTime.Now;
            Notes = "";
        }

        public Queues Save()
        {
            return new PatientService().SaveQueue(this);
        }
    }
}
