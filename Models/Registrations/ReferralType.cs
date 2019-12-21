using System;
namespace AfyaHMIS.Models.Registrations
{
    public class ReferralType
    {
        public long Id { get; set; }
        public long ConceptId { get; set; }
        public string Name { get; set; }

        public ReferralType()
        {
            Id = 0;
            ConceptId = 0;
            Name = "";
        }
    }
}
