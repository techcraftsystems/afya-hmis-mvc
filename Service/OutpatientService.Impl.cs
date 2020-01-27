using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using AfyaHMIS.Extensions;
using AfyaHMIS.Models;
using AfyaHMIS.Models.Concepts;
using AfyaHMIS.Models.Finances;
using AfyaHMIS.Models.Outpatients;
using AfyaHMIS.Models.Patients;
using AfyaHMIS.Models.Persons;
using AfyaHMIS.Models.Registrations;
using AfyaHMIS.Models.Rooms;

namespace AfyaHMIS.Service
{
    public class OutpatientService : IOutpatientService
    {
        public Queues GetQueue(long id) {
            var queues = GetQueue(null, null, " qs_idnt=" + id);
            return queues.Count.Equals(0) ? null : queues[0];
        }

        public List<Queues> GetQueue(Room room, DateTime? date, string conditions = "") {
            List<Queues> queue = new List<Queues>();
            string query = "";

            if (room != null)
                query = "WHERE qs_flag=1 and qs_ended_on IS NULL AND qs_room=" + room.Id + " AND CAST(qs_created_on AS DATE)='" + date + "'";
            if (!string.IsNullOrEmpty(conditions))
                query += (string.IsNullOrEmpty(query) ? "WHERE " : " AND ") + conditions;

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT ROW_NUMBER() OVER (ORDER BY qp_order, qs_created_on) AS bl_idnt, qs_idnt, qs_created_on, qs_started_on, qs_ended_on, qs_created_by, qs_notes, qp_idnt, qp_priority, rm_idnt, rm_room, rm_concept, rm_service, rt_idnt, rt_type, rt_concept, qs_seen_by, usr_name, vst_idnt, pt_idnt, pt_uuid, pt_identifier, ps_idnt, ps_name, CASE ps_gender WHEN 'm' THEN 'MALE' WHEN 'f' THEN 'FEMALE' ELSE 'OTHERS' END ps_gender, ps_dob FROM vQueues INNER JOIN QueuesPriority ON qs_priority=qp_idnt INNER JOIN Rooms ON qs_room=rm_idnt INNER JOIN RoomType ON rm_type=rt_idnt INNER JOIN Visit ON qs_visit=vst_idnt INNER JOIN Patient ON vst_patient=pt_idnt INNER JOIN Person ON ps_idnt=pt_person LEFT OUTER JOIN Users ON usr_idnt=qs_seen_by " + query + " ORDER BY qp_order, qs_created_on");
            if (dr.HasRows) {
                while (dr.Read()) {
                    Queues q = new Queues {
                        Ix = Convert.ToInt64(dr[0]),
                        Id = Convert.ToInt64(dr[1]),
                        CreatedOn = Convert.ToDateTime(dr[2]),
                        StartOn = dr[3].ToString() == "" ? (DateTime?)null : Convert.ToDateTime(dr[3]),
                        EndedOn = dr[4].ToString() == "" ? (DateTime?)null : Convert.ToDateTime(dr[4]),
                        CreatedBy = new Users { Id = Convert.ToInt64(dr[5]) },
                        Notes = dr[6].ToString(),
                        Priority = new QueuesPriority {
                            Id = Convert.ToInt64(dr[7]),
                            Name = dr[8].ToString()
                        },
                        Room = new Room {
                            Id = Convert.ToInt64(dr[9]),
                            Name = dr[10].ToString(),
                            Concept = new Concept { Id = Convert.ToInt64(dr[11]) },
                            Service = new BillableService { Id = Convert.ToInt64(dr[12]) },
                            Type = new RoomType {
                                Id = Convert.ToInt64(dr[13]),
                                Name = dr[14].ToString(),
                                Concept = new Concept { Id = Convert.ToInt64(dr[15]) }
                            }
                        },
                        SeenBy = new Users {
                            Id = Convert.ToInt64(dr[16]),
                            Name = dr[17].ToString(),
                        },
                        Visit = new Visit {
                            Id = Convert.ToInt64(dr[18]),
                            Patient = new Patient {
                                Id = Convert.ToInt64(dr[19]),
                                Uuid = dr[20].ToString(),
                                Identifier = dr[21].ToString(),
                                Person = new Person {
                                    Id = Convert.ToInt64(dr[22]),
                                    Name = dr[23].ToString(),
                                    Gender = dr[24].ToString(),
                                    DateOfBirth = Convert.ToDateTime(dr[25])
                                }
                            }
                        }
                    };

                    q.Date = q.CreatedOn.ToString("dd/MM/yyyy");
                    q.Start = q.StartOn.HasValue ? ((DateTime)q.StartOn).ToString("dd/MM/yyyy") : "N/A";
                    q.Ended = q.EndedOn.HasValue ? ((DateTime)q.EndedOn).ToString("dd/MM/yyyy") : "N/A";

                    q.Visit.Patient.GetAge();

                    queue.Add(q);
                }
            }

            return queue;
        }

        public Triage GetTriage(long id) {
            var triage = GetTriage(null, null, "", "WHERE tg_idnt=" + id);
            return triage.Count.Equals(0) ? null : triage[0];
        }

        public List<Triage> GetTriage(Patient patient, Queues queue, string additional = "", string condition = "")
        {
            List<Triage> triages = new List<Triage>();
            string query = "";

            if (patient != null)
                query += "WHERE vst_patient=" + patient.Id;
            if (queue != null)
                query += (string.IsNullOrEmpty(query) ? "WHERE " : " AND ") + "tg_queue=" + queue.Id;
            if (!string.IsNullOrEmpty(additional))
                query += (string.IsNullOrEmpty(query) ? "WHERE " : " AND ") + additional;
            if (!string.IsNullOrEmpty(condition))
                query = condition;

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT tg_idnt, tg_temp, tg_systolic_bp, tg_diastolic_bp, tg_respiratory_rate, tg_pulse_rate, tg_oxygen_saturation, tg_weight, th_height, tg_bmi, tg_muac, tg_chest, tg_abdominal, tg_pain, tg_situation, tg_background, tg_assessment, tg_recommendation, tg_notes, tg_mobility, mb.ct_name, tg_trauma, tr.ct_name, tg_avpu, av.ct_name, usr_idnt, usr_uuid, usr_name, qs_idnt, qs_room, vst_idnt, vst_patient, vst_type FROM Triage INNER JOIN Queues ON tg_queue=qs_idnt INNER JOIN Visit ON tg_visit=vst_idnt INNER JOIN Users ON tg_created_by=usr_idnt LEFT OUTER JOIN Concept mb ON mb.ct_idnt=tg_mobility LEFT OUTER JOIN Concept tr ON tr.ct_idnt=tg_trauma LEFT OUTER JOIN Concept av ON av.ct_idnt=tg_trauma " + query + " ORDER BY tg_created_on DESC");
            if (dr.HasRows) {
                while (dr.Read()) {
                    triages.Add(new Triage {
                        Id = Convert.ToInt64(dr[0]),
                        Temparature = new Vitals { Value = dr[1].ToString().Trim() },
                        BpSystolic = new Vitals { Value = dr[2].ToString().Trim() },
                        BpDiastolic = new Vitals { Value = dr[3].ToString().Trim() },
                        RespiratoryRate = new Vitals { Value = dr[4].ToString().Trim() },
                        PulseRate = new Vitals { Value = dr[5].ToString().Trim() },
                        OxygenSaturation = new Vitals { Value = dr[6].ToString().Trim() },

                        Weight = new Vitals { Value = dr[7].ToString().Trim() },
                        Height = new Vitals { Value = dr[8].ToString().Trim() },
                        BMI = new Vitals { Value = dr[9].ToString().Trim() },
                        MUAC = new Vitals { Value = dr[10].ToString().Trim() },
                        Chest = new Vitals { Value = dr[11].ToString().Trim() },
                        Abdominal = new Vitals { Value = dr[12].ToString().Trim() },
                        PainScale = new Vitals { Value = dr[13].ToString().Trim() },

                        Situation = new Vitals { Value = dr[14].ToString().Trim() },
                        Background = new Vitals { Value = dr[15].ToString().Trim() },
                        Assessment = new Vitals { Value = dr[16].ToString().Trim() },
                        Recommendation = new Vitals { Value = dr[17].ToString().Trim() },

                        Notes = dr[18].ToString().Trim(),

                        Mobility = new Vitals { Units = dr[19].ToString().Trim(), Value = dr[20].ToString().Trim() },
                        Trauma = new Vitals { Units = dr[21].ToString().Trim(), Value = dr[22].ToString().Trim() },
                        AVPU = new Vitals { Units = dr[23].ToString().Trim(), Value = dr[24].ToString().Trim() },

                        CreatedBy = new Users {
                            Id = Convert.ToInt64(dr[25]),
                            Uuid = dr[26].ToString(),
                            Name = dr[27].ToString()
                        },
                        Queue = new Queues {
                            Id = Convert.ToInt64(dr[28]),
                            Room = new Room { Id = Convert.ToInt64(dr[29]) },
                            Visit = new Visit {
                                Id = Convert.ToInt64(dr[30]),
                                Patient = new Patient { Id = Convert.ToInt64(dr[31]) },
                                Type = new VisitType { Id = Convert.ToInt64(dr[32]) }
                            },  
                        }
                    });
                }
            } return triages;
        }

        public List<Triage> GetTriageList(Patient p) {
            List<Triage> triages = new List<Triage>();

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("DECLARE @p INT=" + p.Id + "; SELECT tg_idnt, tg_created_on, tg_queue, tg_visit FROM Triage INNER JOIN Visit ON vst_idnt=tg_visit WHERE vst_patient=@p ORDER BY tg_created_on DESC");
            if (dr.HasRows) {
                while (dr.Read()) {
                    triages.Add(new Triage {
                        Id = Convert.ToInt64(dr[0]),
                        CreatedOn = Convert.ToDateTime(dr[1]),
                        Queue = new Queues { Id = Convert.ToInt64(dr[2]) },
                        Visit = new Visit { Id = Convert.ToInt64(dr[3]) }
                    });
                }
            } return triages;
        }

        public Triage SaveTriage(Triage triage) {
            SqlServerConnection conn = new SqlServerConnection();
            triage.Id = conn.SqlServerUpdate("DECLARE @idnt INT=" + triage.Id + ", @queue INT=" + triage.Queue.Id + ", @visit INT=" + triage.Visit.Id + ", @temp NVARCHAR(10)='" + triage.Temparature.Value + "', @systolic NVARCHAR(10)='" + triage.BpSystolic.Value + "', @diastolic NVARCHAR(10)='" + triage.BpDiastolic.Value + "', @resp NVARCHAR(10)='" + triage.RespiratoryRate.Value + "', @pulse NVARCHAR(10)='" + triage.PulseRate.Value + "', @oxygen NVARCHAR(10)='" + triage.OxygenSaturation.Value + "', @weight NVARCHAR(10)='" + triage.Weight.Value + "', @height NVARCHAR(10)='" + triage.Height.Value + "', @muac NVARCHAR(10)='" + triage.MUAC.Value + "', @chest NVARCHAR(10)='" + triage.Chest.Value + "', @abdominal NVARCHAR(10)='" + triage.Abdominal.Value + "', @mobility NVARCHAR(10)='" + triage.Mobility.Value + "', @trauma NVARCHAR(10)='" + triage.Trauma.Value + "', @avpu NVARCHAR(10)='" + triage.AVPU.Value + "', @pain NVARCHAR(10)='" + triage.PainScale.Value + "', @situation NVARCHAR(MAX)='" + triage.Situation.Value + "', @background NVARCHAR(MAX)='" + triage.Background.Value + "', @assessment NVARCHAR(MAX)='" + triage.Assessment.Value + "', @recommendation NVARCHAR(MAX)='" + triage.Recommendation.Value + "', @notes NVARCHAR(MAX)='" + triage.Notes + "', @user INT=" + triage.CreatedBy.Id + "; " +
                "IF NOT EXISTS (SELECT tg_idnt FROM Triage WHERE tg_idnt=@idnt) " +
                "BEGIN INSERT INTO Triage (tg_queue, tg_visit, tg_temp, tg_systolic_bp, tg_diastolic_bp, tg_respiratory_rate, tg_pulse_rate, tg_oxygen_saturation, tg_weight, th_height, tg_muac, tg_chest, tg_abdominal, tg_mobility, tg_trauma, tg_avpu, tg_pain, tg_situation, tg_background, tg_assessment, tg_recommendation, tg_created_by, tg_notes) output INSERTED.tg_idnt VALUES (@queue, @visit, @temp, @systolic, @diastolic, @resp, @pulse, @oxygen, @weight, @height, @muac, @chest, @abdominal, @mobility, @trauma, @avpu, @pain, @situation, @background, @assessment, @recommendation, @user, @notes) END " +
                "ELSE " +
                "BEGIN UPDATE Triage SET tg_temp=@temp, tg_systolic_bp=@systolic, tg_diastolic_bp=@diastolic, tg_respiratory_rate=@resp, tg_pulse_rate=@pulse, tg_oxygen_saturation=@oxygen, tg_weight=@weight, th_height=@height, tg_muac=@muac, tg_chest=@chest, tg_abdominal=@abdominal, tg_mobility=@mobility, tg_trauma=@trauma, tg_avpu=@avpu, tg_pain=@pain, tg_situation=@situation, tg_background=@background, tg_assessment=@assessment, tg_recommendation=@recommendation, tg_notes=@notes output INSERTED.tg_idnt WHERE tg_idnt=@idnt END");

            return triage;
        }
    }
}
