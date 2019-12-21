using System;
namespace AfyaHMIS.Models
{
    public class Modules
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public string Urls { get; set; }
        public string Class { get; set; }
        public bool Void { get; set; }
        public int Order { get; set; }

        public Modules()
        {
            Id = 0;
            Name = "";
            Description = "";
            Icon = "";
            Urls = "";
            Class = "";
            Void = false;
            Order = 0;
        }
    }
}
