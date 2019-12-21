using System;
using System.Collections.Generic;
using AfyaHMIS.Models;

namespace AfyaHMIS.ViewModel
{
    public class HomeIndexViewModel
    {
        public List<Modules> Modules { get; set; }

        public HomeIndexViewModel()
        {
            Modules = new List<Modules>();
        }
    }
}
