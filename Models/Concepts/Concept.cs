using System;
namespace AfyaHMIS.Models.Concepts
{
    public class Concept
    {
		public long Id { get; set; }
		public string Name   { get; set; }

        public Concept()
        {
            Id = 0;
            Name = "";
        }
    }
}
