using System;
namespace AfyaHMIS.Models.Administrations
{
    public class Department
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public Department()
        {
            Id = 0;
            Name = "";
        }
    }
}
