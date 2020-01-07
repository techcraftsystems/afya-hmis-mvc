using System;
using System.Globalization;
using System.Security.Claims;
using AfyaHMIS.Extensions;
using AfyaHMIS.Models;
using AfyaHMIS.Models.Concepts;
using AfyaHMIS.Models.Outpatients;
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
        private readonly IPatientService IPatientService;
        private readonly IOutpatientService IOutpatientService;
        private readonly IConceptService IConceptService;

        [BindProperty]
        public OutpatientTriageViewVModel TriageModel { get; set; }

        public OutpatientController(ICoreService core, IOutpatientService outpatient, IPatientService patient, IConceptService concept) {
            ICoreService = core;
            IPatientService = patient;
            IConceptService = concept;
            IOutpatientService = outpatient;
        }

        [Route("/outpatient/triage.queue")]
        public IActionResult TriageQueue(OutpatientQueueViewModel model, string date = "",string error = "") {
            model.Types = ICoreService.GetRoomsIEnumerable(new RoomType { Id = Constants.ROOM_TRIAGE });

            DateTime queueDate = DateTime.Now;
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

            if (error == "1011")
                model.Message = "Invalid patient. Queue request didn't match the passed patient";
            if (!string.IsNullOrEmpty(date))
                queueDate = DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            model.Date = queueDate.ToString("dd/MM/yyyy");
            model.Room = ICoreService.GetRoom(Convert.ToInt64(room));
            model.Queue = IOutpatientService.GetQueue(model.Room, queueDate);

            return View(model);
        }

        [Route("/outpatient/triage")]
        public IActionResult Triage(int qid, string pt, OutpatientTriageViewVModel model) {
            model.Patient = IPatientService.GetPatient(pt);
            model.Queue = IOutpatientService.GetQueue(qid);

            if (!model.Patient.Id.Equals(model.Queue.Visit.Patient.Id)) {
                return LocalRedirect("/outpatient/triage.queue?error=1011");
            }

            model.MobilityOpts = IConceptService.GetConceptAnswersIEnumerable(new Concept { Id = Constants.MOBILITY });
            model.AvpuOpts = IConceptService.GetConceptAnswersIEnumerable(new Concept { Id = Constants.AVPU });
            model.TraumaOpts = IConceptService.GetConceptAnswersIEnumerable(new Concept { Id = Constants.TRAUMA });

            model.Rooms = ICoreService.GetRoomsIEnumerable("1");
            model.Types = ICoreService.GetRoomsIEnumerable(new RoomType { Id = Constants.ROOM_OPD });
            model.Priority = ICoreService.GetQueuePriorityIEnumerable();

            return View(model);
        }

        [Route("/outpatient/doctor.queue")]
        public IActionResult DoctorQueue(OutpatientQueueViewModel model)
        {
            model.Types = ICoreService.GetRoomsIEnumerable(new RoomType { Id = Constants.ROOM_OPD });

            string room = HttpContext.Request.Cookies["doctor.room"];
            if (string.IsNullOrEmpty(room) && model.Types.Count > 0) {
                room = model.Types[0].Value;
                CookieOptions opts = new CookieOptions {
                    Expires = DateTime.Now.AddMonths(3),
                    Secure = true
                };
                HttpContext.Response.Cookies.Append("doctor.room", room, opts);
            }
            else if (string.IsNullOrEmpty(room)) {
                room = "0";
            }

            model.Room = ICoreService.GetRoom(Convert.ToInt64(room));
            model.Queue = IOutpatientService.GetQueue(model.Room, DateTime.Now);

            return View(model);
        }

        [Route("/outpatient/doctor")]
        public IActionResult Doctor(int qid, string pt, OutpatientDoctorViewVModel model) {
            model.Patient = IPatientService.GetPatient(pt);
            model.Queue = IOutpatientService.GetQueue(qid);
 
            if (!model.Patient.Id.Equals(model.Queue.Visit.Patient.Id))            {
                return LocalRedirect("/outpatient/triage.queue?error=1011");
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult SaveTriage() {
            Users user = new Users { Id = long.Parse(HttpContext.User.FindFirst(ClaimTypes.Actor).Value) };
            Queues queue = IOutpatientService.GetQueue(TriageModel.Queue.Id);

            Triage triage = TriageModel.Triage;
            triage.Visit = queue.Visit;
            triage.Queue = queue;

            //Test::Mobility/Trauma/Avpu && convert to int
            if (string.IsNullOrEmpty(triage.Mobility.Value))
                triage.Mobility.Value = "0";
            if (string.IsNullOrEmpty(triage.Trauma.Value))
                triage.Trauma.Value = "0";
            if (string.IsNullOrEmpty(triage.AVPU.Value))
                triage.AVPU.Value = "0";

            //Sanitize TextAreas
            triage.Situation.Value = triage.Situation.Value.ToValidString();
            triage.Background.Value = triage.Background.Value.ToValidString();
            triage.Assessment.Value = triage.Assessment.Value.ToValidString();
            triage.Recommendation.Value = triage.Recommendation.Value.ToValidString();
            triage.Notes = triage.Notes.ToValidString();

            //Save Triage
            triage.CreatedBy = user;
            triage.Save();

            if (!queue.StartOn.HasValue) {
                queue.SeenBy = user;
                queue.StartEncounter();
            }
            queue.CompleteEncounter();

            var dest = TriageModel.SendTo;
            dest.Visit = queue.Visit;
            dest.CreatedBy = user;
            dest.Save();

            return LocalRedirect("/outpatient/triage.queue" + (queue.CreatedOn.Date.Equals(DateTime.Now.Date) ? "" : "?date=" + TriageModel.Queue.Date));
        }

        public JsonResult GetOutpatientQueue(int room, string date) {
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

            return Ok(Json("success"));
        }

        public IActionResult StartTriage(long qid) {
            Queues queue = new Queues {
                Id = qid,
                SeenBy = new Users { Id = long.Parse(HttpContext.User.FindFirst(ClaimTypes.Actor).Value) }
            };
            queue.StartEncounter();

            return Ok(Json("success"));
        }
    }
}
