using System;
using System.Collections.Generic;
using AfyaHMIS.Models.Rooms;

namespace AfyaHMIS.Service
{
    public interface IOutpatientService {
        public List<Queues> GetQueue(Room room, DateTime date); 
    }
}
