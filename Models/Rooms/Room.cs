using System;
using AfyaHMIS.Models.Concepts;

namespace AfyaHMIS.Models.Rooms
{
    public class Room
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public RoomType Type { get; set; }
        public Concept Concept { get; set; }

        public Room()
        {
            Id = 0;
            Name = "";
            Type = new RoomType();
            Concept = new Concept();
        }
    }
}
