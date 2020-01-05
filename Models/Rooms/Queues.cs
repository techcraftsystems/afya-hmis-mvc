using System;
using AfyaHMIS.Models.Finances;
using AfyaHMIS.Models.Registrations;
using AfyaHMIS.Service;

namespace AfyaHMIS.Models.Rooms
{
    public class Queues
    {
        private IPatientService IPatientService = new PatientService();

        public long Id { get; set; }
        public long Ix { get; set; }
        public string Date { get; set; }
        public string Start { get; set; }
        public string Ended { get; set; }
        public QueuesPriority Priority { get; set; }
        public Visit Visit { get; set; }
        public BillsItem Item { get; set; }
        public Room Room { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? StartOn  { get; set; }
        public DateTime? EndedOn { get; set; }
        public Users CreatedBy { get; set; }
        public Users SeenBy { get; set; }
        public string Notes { get; set; }

        public Queues() {
            Id = 0;
            Date = "";
            Start = "";
            Ended = "";
            Priority = new QueuesPriority();
            Item = new BillsItem();
            Visit = new Visit();
            Room = new Room();
            CreatedBy = new Users();
            CreatedOn = DateTime.Now;
            Notes = "";
        }

        public Queues Save() {
            return IPatientService.SaveQueue(this);
        }

        public Queues StartEncounter() {
            return IPatientService.StartEncounter(this);
        }

        public Queues CompleteEncounter() {
            return IPatientService.CompleteEncounter(this);
        }
    }
}
