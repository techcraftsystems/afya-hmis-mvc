using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using AfyaHMIS.Extensions;
using AfyaHMIS.Models;
using AfyaHMIS.Models.Patients;
using AfyaHMIS.Models.Persons;
using AfyaHMIS.Models.Registrations;
using AfyaHMIS.Models.Rooms;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AfyaHMIS.Service
{
    public interface IPatientService
    {
        public List<Patient> SearchPatients(string names = "", string identifier = "", string phone = "", string age = "", string gender = "", string visit = "");
        public List<SelectListItem> GetPatientIdentificationTypes();

        public Patient GetPatient(string uuid);
        public string GetPatientUuid(Patient patient);

        public Person SavePerson(Person person);
        public PersonAddress SavePersonAddress(PersonAddress address);
        public Patient SavePatient(Patient patient);
        public Patient UpdatePatientStatus(Patient patient);
        public PatientIdentifier SavePatientIdentifier(PatientIdentifier identifier);
        public Encounter CreateEncounter(Encounter Encounter);
        public Visit SaveVisit(Visit visit);
        public Referral SaveReferral(Referral referral);
        public ReferralDoctors SaveReferralDoctors(ReferralDoctors referral);

        public Queues SaveQueue(Queues queue);
        public Queues StartEncounter(Queues queue);
        public Queues CompleteEncounter(Queues queue);
    }

    public class PatientService : IPatientService
    {
        private readonly ICoreService ICoreService = new CoreService();

        public Patient GetPatient(string uuid)
        {
            Patient patient = null;

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT pt_idnt, pt_uuid, pt_identifier, pt_notes, pt_added_by, pt_added_on, pt_visit, pst_idnt, pst_status, pi_idnt, pi_identifier, pi_notes, pit_idnt, pit_type, ps_idnt, ps_name, ps_gender, ps_dob, ps_estimate, ps_added_on, ps_added_by, ps_notes, pa_idnt, pa_default, pa_telephone, pa_email, pa_location, pa_added_on, pa_added_by, pa_notes FROM vPatient WHERE pt_uuid COLLATE SQL_Latin1_General_CP1_CS_AS LIKE '" + uuid + "'");
            if (dr.Read())
            {
                patient = new Patient
                {
                    Id = Convert.ToInt64(dr[0]),
                    Uuid = dr[1].ToString(),
                    Identifier = dr[2].ToString(),
                    Notes = dr[3].ToString(),
                    AddedBy = new Users { Id = Convert.ToInt64(dr[4]) },
                    AddedOn = Convert.ToDateTime(dr[5]),
                    LastVisit = Convert.ToDateTime(dr[6]).ToString("dd/MM/yyyy"),
                    Status = new PatientStatus
                    {
                        Id = Convert.ToInt64(dr[7]),
                        Name = dr[8].ToString()
                    },
                    PI = new PatientIdentifier
                    {
                        Id = Convert.ToInt64(dr[9]),
                        Identifier = dr[10].ToString(),
                        Notes = dr[11].ToString(),
                        Type = new PatientIdentifierType
                        {
                            Id = Convert.ToInt64(dr[12]),
                            Name = dr[13].ToString()
                        }
                    },
                    Person = new Person
                    {
                        Id = Convert.ToInt64(dr[14]),
                        Name = dr[15].ToString(),
                        Gender = dr[16].ToString(),
                        DateOfBirth = Convert.ToDateTime(dr[17]),
                        Estimate = Convert.ToBoolean(dr[18]),
                        AddedOn = Convert.ToDateTime(dr[19]),
                        AddedBy = new Users { Id = Convert.ToInt64(dr[20]) },
                        Notes = dr[21].ToString(),
                        Address = new PersonAddress
                        {
                            Id = Convert.ToInt64(dr[22]),
                            Default = Convert.ToBoolean(dr[23]),
                            Telephone = dr[24].ToString(),
                            Email = dr[25].ToString(),
                            Location = dr[26].ToString(),
                            AddedOn = Convert.ToDateTime(dr[27]),
                            AddedBy = new Users { Id = Convert.ToInt64(dr[28]) },
                            Notes = dr[29].ToString()
                        }
                    }
                };

                patient.GetAge();
            }

            return patient;
        }

        public List<Patient> SearchPatients(string names = "", string identifier = "", string phone = "", string age = "", string gender = "", string visit = "")
        {
            SqlServerConnection conn = new SqlServerConnection();
            List<Patient> search = new List<Patient>();
            string query = "WHERE pt_idnt>0";
            double ageInt;

            if (!string.IsNullOrEmpty(names))
                query += conn.GetQueryString(names, "ps_name", "", true, false);
            if (!string.IsNullOrEmpty(identifier))
                query += conn.GetQueryString(identifier, "pt_identifier+'-'+pi_identifier", "", true, false);
            if (!string.IsNullOrEmpty(phone))
                query += conn.GetQueryString(phone, "pa_telephone+'-'+pa_location+'-'+pa_email", "", true, false);
            if (!string.IsNullOrEmpty(age) && Double.TryParse(age, out ageInt) && Double.Parse(age) > 0)
                query += " AND ps_dob BETWEEN DATEADD(YEAR, 0-" + (ageInt + 1) + ", GETDATE()) AND DATEADD(YEAR, 0-" + (ageInt - 1) + ", GETDATE())";
            if (!string.IsNullOrEmpty(gender))
                query += " AND ps_gender='" + gender + "'";
            if (!string.IsNullOrEmpty(visit))
                query += " AND last_visit>=DATEADD(MONTH, 0-" + Int32.Parse(visit) + ", GETDATE())";

            SqlDataReader dr = conn.SqlServerConnect("SELECT pt_idnt, pt_uuid, pt_identifier, pt_notes, pt_added_by, pt_added_on, pt_visit, pst_idnt, pst_status, pi_idnt, pi_identifier, pi_notes, pit_idnt, pit_type, ps_idnt, ps_name, ps_gender, ps_dob, ps_estimate, ps_added_on, ps_added_by, ps_notes, pa_idnt, pa_default, pa_telephone, pa_email, pa_location, pa_added_on, pa_added_by, pa_notes FROM vPatient WHERE pt_idnt IN (SELECT DISTINCT TOP(100)pt_idnt FROM vPatientSearch " + query + ") ORDER BY ps_name, pt_identifier");
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    Patient patient = new Patient
                    {
                        Id = Convert.ToInt64(dr[0]),
                        Uuid = dr[1].ToString(),
                        Identifier = dr[2].ToString(),
                        Notes = dr[3].ToString(),
                        AddedBy = new Users { Id = Convert.ToInt64(dr[4]) },
                        AddedOn = Convert.ToDateTime(dr[5]),
                        LastDate = Convert.ToDateTime(dr[6]),
                        Status = new PatientStatus
                        {
                            Id = Convert.ToInt64(dr[7]),
                            Name = dr[8].ToString()
                        },
                        PI = new PatientIdentifier
                        {
                            Id = Convert.ToInt64(dr[9]),
                            Identifier = dr[10].ToString(),
                            Notes = dr[11].ToString(),
                            Type = new PatientIdentifierType
                            {
                                Id = Convert.ToInt64(dr[12]),
                                Name = dr[13].ToString()
                            }
                        },
                        Person = new Person
                        {
                            Id = Convert.ToInt64(dr[14]),
                            Name = dr[15].ToString(),
                            Gender = dr[16].ToString(),
                            DateOfBirth = Convert.ToDateTime(dr[17]),
                            Estimate = Convert.ToBoolean(dr[18]),
                            AddedOn = Convert.ToDateTime(dr[19]),
                            AddedBy = new Users { Id = Convert.ToInt64(dr[20]) },
                            Notes = dr[21].ToString(),
                            Address = new PersonAddress
                            {
                                Id = Convert.ToInt64(dr[22]),
                                Default = Convert.ToBoolean(dr[23]),
                                Telephone = dr[24].ToString(),
                                Email = dr[25].ToString(),
                                Location = dr[26].ToString(),
                                AddedOn = Convert.ToDateTime(dr[27]),
                                AddedBy = new Users { Id = Convert.ToInt64(dr[28]) },
                                Notes = dr[29].ToString()
                            }
                        }
                    };

                    patient.GetAge();
                    patient.LastVisit = GetLastSeenAsTimeSpan(patient.LastDate);
                    search.Add(patient);
                }
            }

            return search;
        }

        public string GetPatientUuid(Patient patient)
        {
            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT pt_uuid FROM Patient WHERE pt_idnt=" + patient.Id);
            if (dr.Read())
            {
                return dr[0].ToString();
            }

            return "";
        }

        public List<SelectListItem> GetPatientIdentificationTypes()
        {
            return ICoreService.GetIEnumerable("SELECT pit_idnt, pit_type FROM PatientIdentifierType ORDER BY pit_order, pit_idnt");
        }

        public string GetPatientIdentifier(Patient patient)
        {
            string prefix = patient.Person.Name.Replace(" ", "").Substring(0, 3).ToUpper() + patient.Person.Gender.Substring(0, 1).ToUpper();

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT ISNULL(MAX(SUBSTRING(pt_identifier, 5, 100)),0)+1 FROM Patient WHERE pt_identifier LIKE '" + prefix + "%'");
            if (dr.Read())
            {
                return prefix + dr[0].ToString().PadLeft(7, '0');
            }

            return prefix + "0000001";
        }

        private string GetLastSeenAsTimeSpan(DateTime date)
        {
            TimeSpan span = DateTime.Now - date;
            if (span.TotalMinutes <= 60)
                return Convert.ToInt64(span.TotalMinutes) + (span.TotalMinutes.Equals(1) ? " min ago" : " mins ago");
            if (span.TotalHours <= 24)
                return Convert.ToInt64(span.TotalHours) + (span.TotalHours.Equals(1) ? " hr ago" : " hrs ago");
            if (span.TotalHours <= 48)
                return "Yesterday";
            if (span.TotalDays < 31)
                return Convert.ToInt64(span.TotalDays) + (span.TotalDays.Equals(1) ? " day ago" : " days ago");

            int mnth = (DateTime.Now.Month - date.Month) + 12 * (DateTime.Now.Year - date.Year);
            if (mnth <= 12)
                return mnth + (mnth.Equals(1) ? " mnth ago" : " mnths ago");

            int year = (new DateTime(1, 1, 1) + span).Year - 1;
            if (year <= 5)
                return year + (year.Equals(1) ? " yr ago" : " yrs ago");

            return date.ToString("dd/MM/yyyy");
        }

        /* Section
         * For
         * Data
         * Writers
         */

        public Person SavePerson(Person person)
        {
            SqlServerConnection conn = new SqlServerConnection();
            person.Id = conn.SqlServerUpdate("DECLARE @idnt INT=" + person.Id + ", @name NVARCHAR(250)='" + person.Name.ToUpper() + "', @gender NVARCHAR(1)='" + person.Gender + "', @dob DATE='" + person.DateOfBirth + "', @actor INT=" + person.AddedBy.Id + ", @estimate BIT='" + person.Estimate + "', @notes NVARCHAR(MAX)='" + person.Notes + "'; IF NOT EXISTS (SELECT ps_idnt FROM Person WHERE ps_idnt=@idnt) BEGIN INSERT INTO Person(ps_name, ps_gender, ps_dob, ps_added_by, ps_estimate, ps_notes) output INSERTED.ps_idnt VALUES (@name, @gender, @dob, @actor, @estimate, @notes) END ELSE BEGIN UPDATE Person SET ps_name=@name, ps_gender=@gender, ps_dob=@dob, ps_estimate=@estimate, ps_notes=@notes output INSERTED.ps_idnt WHERE ps_idnt=@idnt END");

            return person;
        }

        public PersonAddress SavePersonAddress(PersonAddress address)
        {
            SqlServerConnection conn = new SqlServerConnection();
            address.Id = conn.SqlServerUpdate("DECLARE @idnt INT=" + address.Id + ", @default BIT='" + address.Default + "', @person INT=" + address.Person.Id + ", @telephone NVARCHAR(250)='" + address.Telephone + "', @email NVARCHAR(250)='" + address.Email + "', @location NVARCHAR(250)='" + address.Location + "', @user INT=" + address.AddedBy.Id + ", @notes NVARCHAR(250)='" + address.Notes + "'; IF NOT EXISTS (SELECT pa_idnt FROM PersonAddress WHERE pa_idnt=@idnt) BEGIN INSERT INTO PersonAddress(pa_default, pa_person, pa_telephone, pa_email, pa_location, pa_added_by, pa_notes) output INSERTED.pa_idnt VALUES (@default, @person, @telephone, @email, @location, @user, @notes) END ELSE BEGIN UPDATE PersonAddress SET pa_default=@default, pa_person=@person, pa_telephone=@telephone, pa_email=@email, pa_location=@location, pa_notes=@notes output INSERTED.pa_idnt WHERE pa_idnt=@idnt END");

            return address;
        }

        public Patient SavePatient(Patient patient)
        {
            SqlServerConnection conn = new SqlServerConnection();
            patient.Id = conn.SqlServerUpdate("DECLARE @idnt INT=" + patient.Id + ", @identifier NVARCHAR(50)='" + GetPatientIdentifier(patient) + "', @person INT=" + patient.Person.Id + ", @status INT=" + patient.Status.Id + ", @user INT=" + patient.AddedBy.Id + ", @notes NVARCHAR(MAX)='" + patient.Notes + "'; IF NOT EXISTS (SELECT pt_idnt FROM Patient WHERE pt_idnt=@idnt) BEGIN INSERT INTO Patient(pt_identifier, pt_person, pt_status, pt_added_by, pt_notes) output INSERTED.pt_idnt VALUES (@identifier, @person, @status, @user, @notes) END ELSE BEGIN UPDATE Patient SET pt_status=@status, pt_notes=@notes output INSERTED.pt_idnt WHERE pt_idnt=@idnt END");

            return patient;
        }

        public Patient UpdatePatientStatus(Patient patient)
        {
            SqlServerConnection conn = new SqlServerConnection();
            conn.SqlServerUpdate("DECLARE @idnt INT=" + patient.Id + ", @status INT=" + patient.Status.Id + "; UPDATE Patient SET pt_status=@status WHERE pt_idnt=@idnt");

            return patient;
        }

        public PatientIdentifier SavePatientIdentifier(PatientIdentifier pi)
        {
            SqlServerConnection conn = new SqlServerConnection();
            pi.Id = conn.SqlServerUpdate("DECLARE @idnt INT=" + pi.Id + ", @default BIT='" + pi.Default + "', @patient INT=" + pi.Patient.Id + ", @type INT=" + pi.Type.Id + ", @identifier NVARCHAR(250)='" + pi.Identifier + "', @user INT=" + pi.AddedBy.Id + ", @notes NVARCHAR(MAX)='" + pi.Notes + "'; IF NOT EXISTS (SELECT pi_idnt FROM PatientIdentifier WHERE pi_idnt=@idnt) BEGIN INSERT INTO PatientIdentifier(pi_default, pi_patient, pi_type, pi_identifier, pi_added_by, pi_notes) output INSERTED.pi_idnt VALUES (@default, @patient, @type, @identifier, @user, @notes) END ELSE BEGIN UPDATE PatientIdentifier SET pi_default=@default, pi_type=@type, pi_identifier=@identifier, pi_notes=@notes output INSERTED.pi_idnt WHERE pi_idnt=@idnt END");

            return pi;
        }

        public Encounter CreateEncounter(Encounter encounter)
        {
            SqlServerConnection conn = new SqlServerConnection();
            encounter.Id = conn.SqlServerUpdate("DECLARE @visit INT=" + encounter.Visit.Id + ", @patient INT=" + encounter.Patient.Id + ", @type INT=" + encounter.Type.Id + ", @user INT=" + encounter.CreatedBy.Id + ", @notes NVARCHAR(250)='" + encounter.Notes + "'; INSERT INTO Encounter (enc_visit, enc_patient, enc_type, enc_created_by, enc_notes) output INSERTED.enc_idnt VALUES (@visit, @patient, @type, @user, @notes)");

            return encounter;
        }

        public Visit SaveVisit(Visit visit)
        {
            SqlServerConnection conn = new SqlServerConnection();
            visit.Id = conn.SqlServerUpdate("DECLARE @idnt INT=" + visit.Id + ", @patient INT=" + visit.Patient.Id + ", @type INT=" + visit.Type.Id + ", @code INT=" + visit.ClientCode.Id + ", @scheme NVARCHAR(250)='" + visit.SchemeNumber + "', @barcode NVARCHAR(250)='" + visit.Barcode + "', @medico INT=" + visit.MedicoLegal.Id + ", @user INT=" + visit.CreatedBy.Id + ", @notes NVARCHAR(MAX)='" + visit.Notes + "'; IF NOT EXISTS (SELECT vst_idnt FROM Visit WHERE vst_idnt=@idnt) BEGIN INSERT INTO Visit(vst_patient, vst_type, vst_client_code, vst_barcode, vst_scheme_no, vst_medico_legal, vst_created_by, vst_notes) output INSERTED.vst_idnt VALUES (@patient, @type, @code, @barcode, @scheme, @medico, @user, @notes) END ELSE BEGIN UPDATE Visit SET vst_type=@type, vst_client_code=@code, vst_barcode=@barcode, vst_scheme_no=@scheme, vst_medico_legal=@medico, vst_notes=@notes output INSERTED.vst_idnt WHERE vst_idnt=@idnt END");

            return visit;
        }

        public Referral SaveReferral(Referral referral)
        {
            SqlServerConnection conn = new SqlServerConnection();
            referral.Id = conn.SqlServerUpdate("DECLARE @idnt INT=" + referral.Id + ", @visit INT=" + referral.Visit.Id + ", @type INT=" + referral.Type.Id + ", @facility NVARCHAR(250)='" + referral.Facility + "', @user INT=" + referral.CreatedBy.Id + ", @notes NVARCHAR(MAX)='" + referral.Notes + "'; IF NOT EXISTS (SELECT rf_idnt FROM Referral WHERE rf_idnt=@idnt) BEGIN INSERT INTO Referral(rf_visit, rf_type, rf_facility, rf_created_by, rf_notes) output INSERTED.rf_idnt VALUES (@visit, @type, @facility, @user, @notes) END ELSE BEGIN UPDATE Referral SET rf_type=@type, rf_facility=@facility, rf_notes=@notes output INSERTED.rf_idnt WHERE rf_idnt=@idnt END");

            return referral;
        }

        public Queues SaveQueue(Queues queue) {
            SqlServerConnection conn = new SqlServerConnection();
            queue.Id = conn.SqlServerUpdate("DECLARE @idnt INT=" + queue.Id + ", @visit INT=" + queue.Visit.Id + ", @priority INT=" + queue.Priority.Id + ", @bill INT=" + queue.Item.Id + ", @room INT=" + queue.Room.Id + ", @user INT=" + queue.CreatedBy.Id + ", @notes NVARCHAR(MAX)='" + queue.Notes + "'; IF NOT EXISTS (SELECT qs_idnt FROM Queues WHERE qs_idnt=@idnt) BEGIN INSERT INTO Queues (qs_visit, qs_priority, qs_bill, qs_room, qs_created_by, qs_notes) output INSERTED.qs_idnt VALUES (@visit, @priority, @bill, @room, @user, @notes) END ELSE BEGIN UPDATE Queues SET qs_priority=@priority, qs_room=@room, qs_notes=@notes output INSERTED.qs_idnt WHERE qs_idnt=@idnt END");

            return queue;
        }

        public Queues StartEncounter(Queues queue) {
            SqlServerConnection conn = new SqlServerConnection();
            conn.SqlServerUpdate("DECLARE @idnt INT=" + queue.Id + ", @user INT=" + queue.SeenBy.Id + "; UPDATE Queues SET qs_started_on=GETDATE(), qs_seen_by=@user WHERE qs_idnt=@idnt");

            return queue;
        }

        public Queues CompleteEncounter(Queues queue) {
            SqlServerConnection conn = new SqlServerConnection();
            conn.SqlServerUpdate("DECLARE @idnt INT=" + queue.Id + "; UPDATE Queues SET qs_ended_on=GETDATE() WHERE qs_idnt=@idnt");

            return queue;
        }

        public ReferralDoctors SaveReferralDoctors(ReferralDoctors refdoc)
        {
            SqlServerConnection conn = new SqlServerConnection();
            refdoc.Id = conn.SqlServerUpdate("DECLARE @idnt INT=" + refdoc.Id + ", @referral INT=" + refdoc.Referral.Id + ", @doctor INT=" + refdoc.Doctor.Id + ", @notes NVARCHAR(MAX)='" + refdoc.Notes + "'; IF NOT EXISTS (SELECT rd_idnt FROM ReferralDoctor WHERE rd_idnt=@idnt) BEGIN INSERT INTO ReferralDoctor (rd_referral, rd_doctor, rd_notes) output INSERTED.rd_idnt VALUES (@referral, @doctor, @notes) END ELSE BEGIN UPDATE ReferralDoctor SET rd_doctor=@doctor, rd_notes=@notes output INSERTED.rd_idnt WHERE rd_idnt=@idnt END");

            return refdoc;
        }
    }
}