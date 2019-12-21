using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AfyaHMIS.Service
{
    public interface IDoctorService
    {
        public List<SelectListItem> GetDoctorsIEnumerable();
    }
    public class DoctorService : IDoctorService
    {
        private readonly ICoreService ICoreService = new CoreService();

        public List<SelectListItem> GetDoctorsIEnumerable()
        {
            return ICoreService.GetIEnumerable("SELECT dr_idnt, RIGHT('000'+CAST(dr_idnt AS VARCHAR(3)),3)+':'+ps_name FROM Doctor INNER JOIN Person ON dr_person=ps_idnt WHERE dr_void=0");
        }
    }
}
