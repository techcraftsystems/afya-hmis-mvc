using System;
using AfyaHMIS.Models.Concepts;

namespace AfyaHMIS.Models.Rooms
{
    public class RoomType
    {
		public long Id { get; set; }
		public bool Void { get; set; }
        public string Name { get; set; }
        public Concept Concept { get; set; }

        public RoomType()
        {
            Id = 0;
            Void = false;
            Name = "";
            Concept = new Concept();
        }
    }
}
