using AfyaHMIS.Models.Concepts;

namespace AfyaHMIS.Models.Outpatients
{
    public class Vitals
    {
        public long Id { get; set; }
        public Concept Concept { get; set; }
        public VitalsCategory Category { get; set; }
        public string Name { get; set; }
        public string Units { get; set; }
        public string Value { get; set; }
        public Bounds Age { get; set; }
        public Bounds Range { get; set; }
        public string Notes { get; set; }

        public Vitals() {
            Id = 0;
            Concept = new Concept();
            Category = new VitalsCategory();
            Name = "";
            Units = "";
            Age = new Bounds();
            Range = new Bounds();
            Notes = "";
        }
    }
}
