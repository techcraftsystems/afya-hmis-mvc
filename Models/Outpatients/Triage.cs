using System;
using AfyaHMIS.Models.Registrations;
using AfyaHMIS.Models.Rooms;
using AfyaHMIS.Service;

namespace AfyaHMIS.Models.Outpatients
{
    public class Triage
    {
        public long Id { get; set; }
        public string Date { get; set; }
        public Queues Queue { get; set; }
        public Visit Visit { get; set; }

        public Vitals Temparature { get; set; }
        public Vitals BpSystolic { get; set; }
        public Vitals BpDiastolic { get; set; }
        public Vitals RespiratoryRate { get; set; }
        public Vitals PulseRate { get; set; }
        public Vitals OxygenSaturation { get; set; }

        public Vitals Weight { get; set; }
        public Vitals Height { get; set; }
        public Vitals MUAC { get; set; }
        public Vitals BMI { get; set; }
        public Vitals Chest { get; set; }
        public Vitals Abdominal { get; set; }

        public Vitals Mobility { get; set; }
        public Vitals AVPU { get; set; }
        public Vitals Trauma { get; set; }

        public Vitals Situation { get; set; }
        public Vitals Background { get; set; }
        public Vitals Assessment { get; set; }
        public Vitals Recommendation { get; set; }

        public Vitals PainScale { get; set; }

        public string Notes { get; set; }

        public Users CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        
        public Triage() {
            Id = 0;
            Queue = new Queues();
            Visit = new Visit();

            Temparature = new Vitals();
            BpSystolic = new Vitals();
            BpDiastolic = new Vitals();
            RespiratoryRate = new Vitals();
            PulseRate = new Vitals();
            OxygenSaturation = new Vitals();

            Weight = new Vitals();
            Height = new Vitals();
            MUAC = new Vitals();
            BMI = new Vitals();
            Chest = new Vitals();
            Abdominal = new Vitals();

            Mobility = new Vitals();
            AVPU = new Vitals();
            Trauma = new Vitals();

            Situation = new Vitals();
            Background = new Vitals();
            Assessment = new Vitals();
            Recommendation = new Vitals();

            PainScale = new Vitals();

            CreatedBy = new Users();
            CreatedOn = DateTime.Now;
        }

        public Triage Save() {
            return new OutpatientService().SaveTriage(this);
        }
    }
}
