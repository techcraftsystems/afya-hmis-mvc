using System;
namespace AfyaHMIS.Models.Rooms
{
    public class RoomType
    {
		public long Id { get; set; }
		public string Name { get; set; }

		public RoomType()
        {
            Id = 0;
            Name = "";
        }
    }
}
