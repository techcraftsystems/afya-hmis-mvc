
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
namespace AfyaHMIS.Extensions
{
    public class MailSendExtensions
    {
        public List<MailAddress> SendTo { get; set; }
        public List<MailAddress> CopyTo { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }

        public MailSendExtensions()
        {
            SendTo = new List<MailAddress>();
            CopyTo = new List<MailAddress>();
            Message = "";
        }

        public string Send()
        {
            MailMessage message = new MailMessage();
            SmtpClient Client = new SmtpClient("mail.techcraftsystems.net", 587);
            foreach (MailAddress address in SendTo)
            {
                message.To.Add(address);
            }

            foreach (MailAddress address in CopyTo)
            {
                message.CC.Add(address);
            }

            message.From = new MailAddress("helpdesk@medifortehospital.com", "Mediforte Hospital");
            message.Subject = Subject;
            message.Body = Message;
            message.IsBodyHtml = false;
            message.Priority = MailPriority.Normal;

            Client.Credentials = new NetworkCredential("ubunifu.techcraftsystems", new CrytoUtilsExtensions().Decrypt("SF6BQTDvA5yyHNPV92P4iA=="));

            try
            {
                Client.Send(message);
                return "success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }
    }
}