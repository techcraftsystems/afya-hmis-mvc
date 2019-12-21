using System;
namespace AfyaHMIS.Models.Finances
{
    public class ClientCode
    {
		public long Id { get; set; }
		public string Code { get; set; }
		public string Name { get; set; }

        public ClientCode() {
            Id = 0;
            Code = "";
            Name = "";
        }
    }
}
