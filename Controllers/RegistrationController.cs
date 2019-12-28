using System;
using System.Globalization;
using System.Security.Claims;
using AfyaHMIS.Models;
using AfyaHMIS.Models.Administrations;
using AfyaHMIS.Models.Concepts;
using AfyaHMIS.Models.Doctors;
using AfyaHMIS.Models.Finances;
using AfyaHMIS.Models.Patients;
using AfyaHMIS.Models.Registrations;
using AfyaHMIS.Models.Rooms;
using AfyaHMIS.Service;
using AfyaHMIS.ViewModel;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AfyaHMIS.Controllers
{
    [Authorize]
    public class RegistrationController : Controller
    {
        private readonly ICoreService ICoreService;
        private readonly IDoctorService IDoctorService;
        private readonly IConceptService IConceptService;
        private readonly IPatientService IPatientService;
        private readonly IFinanceService IFinanceService;

        [BindProperty]
        public RegistrationNewEditViewModel InputModel { get; set; }

        [BindProperty]
        public RegistrationVisitViewModel VisitModel { get; set; }

        public RegistrationController(IPatientService ipatient, ICoreService icore, IFinanceService ifinance, IConceptService iconcept, IDoctorService idoctor)
        {
            IPatientService = ipatient;
            ICoreService = icore;
            IFinanceService = ifinance;
            IConceptService = iconcept;
            IDoctorService = idoctor;
        }

        [Route("/registration/search")]
        public IActionResult Search(RegistrationRegisterViewModel model)
        {
            return View(model);
        }

        [Route("/registration/new")]
        public IActionResult New(RegistrationNewEditViewModel model, string name = "", string id = "", string phone = "", string age = "", string gender = "") {
            if (!string.IsNullOrEmpty(name))
                model.Patient.Person.Name = name;
            if (!string.IsNullOrEmpty(id))
                model.Patient.PI.Identifier = id;
            if (!string.IsNullOrEmpty(gender))
                model.Patient.Person.Name = gender;
            if (!string.IsNullOrEmpty(phone))
                model.Patient.Person.Address.Telephone = phone;
            if (!string.IsNullOrEmpty(age) && Double.TryParse(age, out double dblAge) && double.Parse(age) > 0)
                model.DateOfBirth = DateTime.Now.AddYears(0 - Int32.Parse(age)).ToString("dd/MM/yyyy");

            model.IdType = IPatientService.GetPatientIdentificationTypes();

            return View(model);
        }

        [Route("/registration/edit")]
        public IActionResult Edit(string p, RegistrationNewEditViewModel model) {
            if (string.IsNullOrEmpty(p))
                return LocalRedirect("/registration/search");

            model.Patient = IPatientService.GetPatient(p);
            model.IdType = IPatientService.GetPatientIdentificationTypes();
            model.DateOfBirth = model.Patient.Person.DateOfBirth.ToString("dd/MM/yyyy");

            return View(model);
        }

        [Route("/registration/visit/")]
        public IActionResult Visit(string p, RegistrationVisitViewModel model) {
            if (string.IsNullOrEmpty(p))
                return LocalRedirect("/registration/search");

            model.Visit.Patient = IPatientService.GetPatient(p);
            model.Codes = ICoreService.GetClientCodesIEnumerable();
            model.Rooms = ICoreService.GetRoomsIEnumerable();
            model.Types = ICoreService.GetRoomsIEnumerable(new RoomType { Id = Constants.ROOM_TRIAGE });
            model.Referrals = IConceptService.GetConceptAnswersIEnumerable(new Concept { Id = Constants.REFERRAL_TYPE });
            model.MedicoLegal = IConceptService.GetConceptAnswersIEnumerable(new Concept { Id = Constants.MEDICO_LEGAL });
            model.Doctors = IDoctorService.GetDoctorsIEnumerable();

            return View(model);
        }

        [Route("/patients/{uuid}")]
        public IActionResult PatientsView(string uuid, RegistrationPateintsViewModel model)
        {
            model.Patient = IPatientService.GetPatient(uuid);

            return View(model);
        }

        //Json Results
        public JsonResult SearchPatients(string names = "", string identifier = "", string phone = "", string age = "", string gender = "", string visit = "") {
            return Json(IPatientService.SearchPatients(names, identifier, phone, age, gender, visit));
        }

        public JsonResult GetRoomsIEnumerable(int type) {
            return Json(ICoreService.GetRoomsIEnumerable(new RoomType { Id = type }));
        }

        public ClientCodeRates GetClientCodeBillableRate(int room, int code) {
            return IFinanceService.GetRoomsBillableRate(new Room { Id = room}, new ClientCode { Id = code });
        }

        [HttpPost]
        //[Authorize(Roles = "Administrator, Super User, Regional Admin, Agency Admin, Facility Admin")]
        public IActionResult RegisterNewPatient() {
            DateTime dob = DateTime.Now;
            bool isNewPatient = true && InputModel.Patient.Id.Equals(0);

            try {
                dob = DateTime.ParseExact(InputModel.DateOfBirth, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            catch (Exception) {
                System.Diagnostics.Debug.WriteLine("...");
            }

            Users user = new Users { Id = long.Parse(HttpContext.User.FindFirst(ClaimTypes.Actor).Value) };
            Patient patient = InputModel.Patient;
            patient.Person.DateOfBirth = dob;
            patient.Person.AddedBy = user;
            patient.Person.Address.AddedBy = user;
            patient.AddedBy = user;
            patient.PI.AddedBy = user;
            patient.Save();

            if (isNewPatient) {
                Encounter encounter = new Encounter {
                    Type = new EncounterType { Id = Constants.ENCOUNTER_REGISTRATION },
                    Patient = patient,
                    CreatedBy = user
                };
                encounter.Create(IPatientService);
            }

            if (isNewPatient)
                return LocalRedirect("/registration/visit?p=" + patient.GetUuid());
            else 
                return LocalRedirect("/patients/" + patient.GetUuid());
        }

        [HttpPost]
        public IActionResult RegisterVisit()
        {
            Users user = new Users { Id = Int64.Parse(HttpContext.User.FindFirst(ClaimTypes.Actor).Value) };
            Patient patient = VisitModel.Visit.Patient;
            patient.Status = new PatientStatus
            {
                Id = Constants.STATUS_ACTIVE
            };
            patient.UpdateStatus();

            Encounter encounter = new Encounter
            {
                Type = new EncounterType { Id = Constants.ENCOUNTER_VISIT },
                Patient = patient,
                CreatedBy = user
            };
            encounter.Create(IPatientService);

            Visit visit = VisitModel.Visit;
            visit.Type = new VisitType { Id = Constants.VISIT_FACILITY };
            visit.CreatedBy = user;
            visit.CreatedOn = DateTime.Now;
            visit.Save();

            Referral referral = VisitModel.Referral;
            if (!referral.Type.Id.Equals(0))
            {
                referral.Visit = visit;
                referral.CreatedBy = user;
                referral.Save();

                var seps = new string[] { "," };
                string[] doctors = VisitModel.DoctorString.Split(seps, StringSplitOptions.RemoveEmptyEntries);
                foreach (string doc in doctors) {
                    new ReferralDoctors
                    {
                        Referral = referral,
                        Doctor = new Doctor {
                            Id = Convert.ToInt64(doc)
                        }
                    }.Save();
                }
            }

            Bills bill = VisitModel.Bill;
            bill.Visit = visit;
            bill.Department = new Department { Id = Constants.DEPT_REGISTRATION };
            bill.CreatedBy = user;
            bill.Save();

            if (VisitModel.Waiver) {
                bill.Waiver = bill.Amount;
                bill.WaiverReason = VisitModel.WaiverReason;
                bill.WaivedBy = user;
                bill.UpdateWaiver();

                bill.Flag = new BillingFlag { Id = Constants.FLAG_CLEARED };
                bill.ProcessedBy = user;
                bill.UpdateProcess();
            }

            BillsItem item = VisitModel.Item;
            item.Bill = bill;
            item.CreatedBy = user;
            item.Save();

            //INVOICE/FLAG/CLEAR
            if (VisitModel.Waiver) {
                Invoice invoice = new Invoice {
                    Patient = patient,
                    Flag = new BillingFlag { Id = Constants.FLAG_CLEARED },
                    CreatedBy = user,
                    Notes = "Waivered Bill"
                };
                invoice.Save();

                InvoiceDetails details = new InvoiceDetails {
                    Invoice = invoice,
                    Item = item,
                    CreatedBy = user,
                    Notes = "Waivered Item"
                };
                details.Save();
            }

            Queues queue = VisitModel.Queue;
            queue.Visit = visit;
            queue.Item = item;
            queue.CreatedBy = user;
            queue.Save();

            return LocalRedirect("/registration/search");
        }

        [AllowAnonymous]
        public string GetDateFromString(string value) {
            if (string.IsNullOrWhiteSpace(value))
                return "";
            else
                value = value.ToLower().Trim();

            try             {
                return DateTime.ParseExact(value, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy");
            }
            catch (Exception) {
                System.Diagnostics.Debug.WriteLine("...");
            }

            if (IsNumber(value)) {
                return DateTime.Now.AddYears(0 - int.Parse(value)).ToString("dd/MM/yyyy");
            }

            if (value.Contains('y')) {
                value = value.Replace("y", string.Empty);

                if (IsNumber(value))
                    return DateTime.Now.AddYears(0 - int.Parse(value)).ToString("dd/MM/yyyy");
                else
                    return "";
            }
            else if (value.Contains('m')) {
                value = value.Replace("m", string.Empty);

                if (IsNumber(value))
                    return DateTime.Now.AddMonths(0 - int.Parse(value)).ToString("dd/MM/yyyy");
                else
                    return "";
            }
            else if (value.Contains('w')) {
                value = value.Replace("w", string.Empty);

                if (IsNumber(value))
                    return DateTime.Now.AddDays(0 - (int.Parse(value) * 7)).ToString("dd/MM/yyyy");
                else
                    return "";
            }
            else if (value.Contains('d')) {
                value = value.Replace("d", string.Empty);

                if (IsNumber(value))
                    return DateTime.Now.AddDays(0 - int.Parse(value)).ToString("dd/MM/yyyy");
                else
                    return "";
            }

            return "";
        }

        public bool IsNumber(string s) {
            bool value = true;
            foreach (char c in s) { 
                value = value && Char.IsDigit(c);
            }

            return value;
        }
    }
}
