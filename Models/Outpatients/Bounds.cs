using System;
namespace AfyaHMIS.Models.Outpatients
{
    public class Bounds
    {
        public long Id { get; set; }
        public double Low { get; set; }
        public double High { get; set; }

        public Bounds() {
            Id = 0;
            Low = 0;
            High = 0;
        }
    }
}
