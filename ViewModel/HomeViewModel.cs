using System;
using System.Collections.Generic;
using AfyaHMIS.Models;

namespace AfyaHMIS.ViewModel
{
    public class HomeIndexViewModel
    {
        public List<Users> Users { get; set; }
        public HomeIndexViewModel()
        {
            Users = new List<Users>();
        }
    }
}
