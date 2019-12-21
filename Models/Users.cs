using System;
using System.Collections.Generic;
using System.Net.Mail;
using AfyaHMIS.Extensions;
using AfyaHMIS.Service;
using Microsoft.AspNetCore.Http;

namespace AfyaHMIS.Models
{
    public class Users
    {
        public long Id { get; set; }
        public string Uuid { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool Enabled { get; set; }
        public bool ToChange { get; set; }
        public long AdminRole { get; set; }
        public long AdminLevel { get; set; }
        public string AccessLevel { get; set; }
        public string Message { get; set; }
        public string LastSeen { get; set; }
        public string Notes { get; set; }
        public DateTime AddedOn { get; set; }

        public Roles Role { get; set; }
        public Users AddedBy { get; set; }

        public Users()
        {
            Id = 0;
            Uuid = "";
            Name = "";
            Email = "";
            Username = "";
            Password = "";
            Enabled = true;
            ToChange = false;
            AdminRole = 0;
            AdminLevel = 0;
            AccessLevel = "";
            Message = "";
            LastSeen = "N/A";
            AddedOn = DateTime.Now;

            Role = new Roles();
        }

        public Users(long idnt) : this()
        {
            Id = idnt;
        }

        public Users(long idnt, string name) : this()
        {
            Id = idnt;
            Name = name;
        }

        public List<UsersRoles> GetRoles()
        {
            return new UserService().GetRoles(this);
        }

        public Users Save(HttpContext context)
        {
            return new UserService(context).SaveUser(this);
        }

        public void ResetPassword()
        {
            this.Password = new CrytoUtilsExtensions().Encrypt("pass");
            this.UpdatePassword(1);

            if (!string.IsNullOrEmpty(Email))
            {
                MailSendExtensions mail = new MailSendExtensions();
                mail.SendTo.Add(new MailAddress(Email, Name));
                mail.Subject = "Call Center Password has been Reset";

                string message = "Dear " + Name + Environment.NewLine + Environment.NewLine;
                message += "The password for your Account on TAC call center system has been reset. Your login credentials are as below" + Environment.NewLine;
                message += "URL: http://callcenter.tachealthafrica.or.ke" + Environment.NewLine;
                message += "Username: " + Username + Environment.NewLine;
                message += "Password: pass" + Environment.NewLine + Environment.NewLine;
                message += "You will be prompted to change the password after the first login. Provide a password of your liking." + Environment.NewLine + Environment.NewLine;
                message += "Regards," + Environment.NewLine;
                message += "System Admin" + Environment.NewLine + Environment.NewLine;
                message += "P.S. This is a system generated Email. Do not respond to it.";

                mail.Message = message;
                mail.Send();
            }
        }

        public void EnableAccount(bool opts = true)
        {
            new UserService().EnableAccount(this, opts);
        }

        public void UpdatePassword(int changepw = 0)
        {
            new UserService().UpdateUserPassword(this, changepw);
        }

        public void UpdateLastAccess()
        {
            new UserService().UpdateLastAccess(this);
        }
    }

    public class Roles
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public Roles()
        {
            Id = 0;
            Name = "";
            Description = "";
        }

        public Roles(long idnt) : this()
        {
            Id = idnt;
        }

        public Roles(long idnt, string name) : this()
        {
            Id = idnt;
            Name = name;
        }
    }

    public class UsersRoles
    {
        public long Id { get; set; }
        public Users User { get; set; }
        public Roles Role { get; set; }

        public UsersRoles()
        {
            Id = 0;
            User = new Users();
            Role = new Roles();
        }
    }
}
