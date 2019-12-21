using System;
namespace AfyaHMIS.Models.Rooms
{
    public class QueuesPriority
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public QueuesPriority()
        {
            Id = 0;
            Name = "";
        }
    }
}
