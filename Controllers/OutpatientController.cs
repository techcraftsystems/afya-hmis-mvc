using System;
using System.Globalization;
using AfyaHMIS.Models;
using AfyaHMIS.Models.Rooms;
using AfyaHMIS.Service;
using AfyaHMIS.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AfyaHMIS.Controllers
{
    [Authorize]
    public class OutpatientController : Controller
    {
        private readonly ICoreService ICoreService;
        private readonly IOutpatientService IOutpatientService;

        public OutpatientController(ICoreService core, IOutpatientService outpatient) {
            ICoreService = core;
            IOutpatientService = outpatient;
        }

        [Route("/outpatient/triage.queue")]
        public IActionResult TriageQueue(OutpatientTriageQueueViewModel model) {
            model.Types = ICoreService.GetRoomsIEnumerable(new RoomType { Id = Constants.ROOM_TRIAGE });        

            string room = HttpContext.Request.Cookies["triage.room"];
            if (string.IsNullOrEmpty(room) && model.Types.Count > 0) {
                room = model.Types[0].Value;
                CookieOptions opts = new CookieOptions {
                    Expires = DateTime.Now.AddMonths(3),
                    Secure = true
                };
                HttpContext.Response.Cookies.Append("triage.room", room, opts);
            }
            else if (string.IsNullOrEmpty(room)) {
                room = "0";
            }

            model.Room = ICoreService.GetRoom(Convert.ToInt64(room));
            model.Queue = IOutpatientService.GetQueue(model.Room, DateTime.Now);

            return View(model);
        }

        public JsonResult GetTriageQueue(int room, string date) {
            return Json(IOutpatientService.GetQueue(new Room { Id = room }, DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture)));
        }

        public IActionResult SetCookie(string value, string queue) {
            string room = HttpContext.Request.Cookies[queue];
            if (!string.IsNullOrEmpty(room).Equals(value)) {
                CookieOptions opts = new CookieOptions {
                    Expires = DateTime.Now.AddMonths(3),
                    Secure = true
                };
                HttpContext.Response.Cookies.Append(queue, value, opts);
            }

            return Ok("success");
        }

        [Route("/outpatient/triage")]
        public IActionResult TriageView(OutpatientTriageViewVModel model) {
            return View(model);
        }
    }
}
