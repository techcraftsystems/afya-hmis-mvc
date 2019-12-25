using System;
using AfyaHMIS.Models.Administrations;
using AfyaHMIS.Models.Patients;

namespace AfyaHMIS.Models.Finances
{
    public class BillsDepartment
    {
        public long Id { get; set; }
        public Patient Patient { get; set; }
        public Department Department { get; set; }
        public double Count { get; set; }
        public double Total { get; set; }

        public BillsDepartment() {
            Id = 0;
            Patient = new Patient();
            Department = new Department();
            Count = 0;
            Total = 0;
        }
    }
}
