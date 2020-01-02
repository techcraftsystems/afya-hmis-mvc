using System;
using AfyaHMIS.Models.Concepts;
using AfyaHMIS.Models.Finances;

namespace AfyaHMIS.Models.Rooms
{
    public class Room
    {
        public long Id { get; set; }
        public bool Void { get; set; }
        public string Name { get; set; }
        public RoomType Type { get; set; }
        public Concept Concept { get; set; }
        public BillableService Service { get; set; }

        public Room()
        {
            Id = 0;
            Void = false;
            Name = "";
            Type = new RoomType();
            Concept = new Concept();
            Service = new BillableService();
        }
    }
}
