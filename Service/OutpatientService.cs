using System;
using System.Collections.Generic;
using AfyaHMIS.Models.Outpatients;
using AfyaHMIS.Models.Rooms;

namespace AfyaHMIS.Service
{
    public interface IOutpatientService {
        public Queues GetQueue(long id);
        public List<Queues> GetQueue(Room room, DateTime? date, string conditions = "");

        public Triage SaveTriage(Triage triage);
    }
}
