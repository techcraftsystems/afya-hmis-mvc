using System;
using System.Collections.Generic;
using AfyaHMIS.Models.Outpatients;
using AfyaHMIS.Models.Patients;
using AfyaHMIS.Models.Rooms;

namespace AfyaHMIS.Service
{
    public interface IOutpatientService {
        public Queues GetQueue(long id);
        public List<Queues> GetQueue(Room room, DateTime? date, string conditions = "");

        public Triage GetTriage(long id);
        public List<Triage> GetTriage(Patient patient, Queues queue, string additional = "", string condition = "");
        public List<Triage> GetTriageList(Patient p);

        public Triage SaveTriage(Triage triage);
    }
}
