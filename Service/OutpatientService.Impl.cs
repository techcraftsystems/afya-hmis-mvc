using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using AfyaHMIS.Extensions;
using AfyaHMIS.Models;
using AfyaHMIS.Models.Concepts;
using AfyaHMIS.Models.Finances;
using AfyaHMIS.Models.Patients;
using AfyaHMIS.Models.Persons;
using AfyaHMIS.Models.Registrations;
using AfyaHMIS.Models.Rooms;

namespace AfyaHMIS.Service
{
    public class OutpatientService : IOutpatientService
    {
        public List<Queues> GetQueue(Room room, DateTime date) {
            List<Queues> queue = new List<Queues>();

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT ROW_NUMBER() OVER (ORDER BY qp_order, qs_created_on) AS bl_idnt, qs_idnt, qs_created_on, qs_started_on, qs_ended_on, qs_created_by, qs_notes, qp_idnt, qp_priority, rm_idnt, rm_room, rm_concept, rm_service, rt_idnt, rt_type, rt_concept, qs_seen_by, usr_name, vst_idnt, pt_idnt, pt_uuid, pt_identifier, ps_idnt, ps_name, CASE ps_gender WHEN 'm' THEN 'MALE' WHEN 'f' THEN 'FEMALE' ELSE 'OTHERS' END ps_gender, ps_dob FROM vQueues INNER JOIN QueuesPriority ON qs_priority=qp_idnt INNER JOIN Rooms ON qs_room=rm_idnt INNER JOIN RoomType ON rm_type=rt_idnt INNER JOIN Visit ON qs_visit=vst_idnt INNER JOIN Patient ON vst_patient=pt_idnt INNER JOIN Person ON ps_idnt=pt_person LEFT OUTER JOIN Users ON usr_idnt=qs_seen_by WHERE qs_flag=1 and qs_ended_on IS NULL AND qs_room=" + room.Id + " AND CAST(qs_created_on AS DATE)='" + date + "' ORDER BY qp_order, qs_created_on");
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
    }
}
