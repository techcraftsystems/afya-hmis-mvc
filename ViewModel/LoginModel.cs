using System;
using AfyaHMIS.Models;

namespace AfyaHMIS.ViewModel
{
    public class LoginModel
    {
        public Users User { get; set; }
        public int ToChange { get; set; }
        public string Message { get; set; }
        public string Password { get; set; }
        public string ReturnUrl { get; set; }

        public LoginModel()
        {
            User = new Users();
            Message = "";
            Password = "";
            ReturnUrl = "/";
            ToChange = 0;
        }
    }
}
