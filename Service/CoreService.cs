using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using AfyaHMIS.Extensions;
using AfyaHMIS.Models;
using AfyaHMIS.Models.Concepts;
using AfyaHMIS.Models.Finances;
using AfyaHMIS.Models.Rooms;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AfyaHMIS.Service
{
    public interface ICoreService {
        public List<Modules> GetApplicationModules();

        public Room GetRoom(long idnt);
        public List<Room> GetRooms(RoomType type, Concept concept, BillableService service, bool includeVoid = false, string conditions = "", string filter = "");

        public List<SelectListItem> GetIEnumerable(string query);
        public List<SelectListItem> GetClientCodesIEnumerable();
        public List<SelectListItem> GetRoomsIEnumerable(string exclude = "");
        public List<SelectListItem> GetRoomsIEnumerable(RoomType Type);
        public List<SelectListItem> GetQueuePriorityIEnumerable(bool order = false);
    }

    public class CoreService : ICoreService
    {
        public List<SelectListItem> GetIEnumerable(string query) {
            List<SelectListItem> ienumarable = new List<SelectListItem>();
            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect(query);
            if (dr.HasRows) {
                while (dr.Read()) {
                    ienumarable.Add(new SelectListItem {
                        Value = dr[0].ToString(),
                        Text = dr[1].ToString()
                    });
                }
            }

            return ienumarable;
        }

        public Room GetRoom(long idnt) {
            List<Room> rooms = GetRooms(null, null, null, true, "rm_idnt=" + idnt);
            return rooms.Count.Equals(0) ? null : rooms[0];
        }

        public List<Room> GetRooms(RoomType type, Concept concept, BillableService service, bool includeVoid = false, string conditions = "", string filter = "") {
            List<Room> rooms = new List<Room>();
            SqlServerConnection conn = new SqlServerConnection();

            string query = "";

            if (type != null)
                query = "WHERE rm_type=" + type.Id;
            if (concept != null)
                query += (query == "" ? "WHERE " : " AND ") + "rm_concept=" + concept.Id;
            if (service != null)
                query += (query == "" ? "WHERE " : " AND ") + "rm_service=" + service.Id;
            if (!includeVoid)
                query += (query == "" ? "WHERE " : " AND ") + "rm_void=0";
            if (!string.IsNullOrEmpty(conditions))
                query += (query == "" ? "WHERE " : " AND ") + conditions;
            if (!string.IsNullOrEmpty(filter))
                query += conn.GetQueryString(filter, "rm_room+'-'+rt_type+'-'+bs_service+'-'+CAST(bs_amount AS NVARCHAR)", "", true, false);

            SqlDataReader dr = conn.SqlServerConnect("SELECT rm_idnt, rm_void, rm_room, rm_concept, rt_idnt, rt_void, rt_concept, rt_type, bs_idnt, bs_code, bs_concept, bs_service, bs_amount, bs_description FROM Rooms INNER JOIN RoomType ON rm_type=rt_idnt INNER JOIN BillableService ON rm_service=bs_idnt " + query);
            if (dr.HasRows) {
                while (dr.Read()) {
                    rooms.Add(new Room {
                        Id = Convert.ToInt64(dr[0]),
                        Void = Convert.ToBoolean(dr[1]),
                        Name = dr[2].ToString(),
                        Concept = new Concept { Id = Convert.ToInt64(dr[3]) },
                        Type = new RoomType {
                            Id = Convert.ToInt64(dr[4]),
                            Void = Convert.ToBoolean(dr[5]),
                            Concept = new Concept { Id = Convert.ToInt64(dr[6]) },
                            Name = dr[7].ToString(),
                        },
                        Service = new BillableService {
                            Id = Convert.ToInt64(dr[8]),
                            Code = dr[9].ToString(),
                            Concept = new Concept { Id = Convert.ToInt64(dr[10]) },
                            Name = dr[11].ToString(),
                        }
                    });
                }
            }

            return rooms;
        }

        public List<SelectListItem> GetClientCodesIEnumerable() {
            return GetIEnumerable("SELECT cl_idnt, cl_code+' :: '+cl_name FROM ClientCodes ORDER BY cl_code, cl_name");
        }

        public List<SelectListItem> GetRoomsIEnumerable(string exclude = "") {
            string query = "WHERE rt_void=0";
            if (!string.IsNullOrEmpty(exclude))
                query += " AND rt_idnt NOT IN (" + exclude + ")";
            return GetIEnumerable("SELECT rt_idnt, rt_type FROM RoomType " + query);
        }

        public List<SelectListItem> GetRoomsIEnumerable(RoomType type) {
            return GetIEnumerable("SELECT rm_idnt, rm_room FROM Rooms WHERE rm_void=0 AND rm_type=" + type.Id);
        }

        public List<SelectListItem> GetQueuePriorityIEnumerable(bool order = false) {
            return GetIEnumerable("SELECT qp_idnt, qp_priority FROM QueuesPriority " + (order ? "ORDER BY qp_order" : ""));
        }

        public List<Modules> GetApplicationModules()
        {
            List<Modules> modules = new List<Modules>();
            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT md_idnt, md_void, md_order, md_name, md_description, md_icon, md_url, md_class FROM Modules ORDER BY md_order, md_idnt");
            if (dr.HasRows) {
                while (dr.Read()) {
                    modules.Add(new Modules {
                        Id = Convert.ToInt64(dr[0]),
                        Void = Convert.ToBoolean(dr[1]),
                        Order = Convert.ToInt32(dr[2]),
                        Name = dr[3].ToString(),
                        Description = dr[4].ToString(),
                        Icon = dr[5].ToString(),
                        Urls = dr[6].ToString(),
                        Class = dr[7].ToString()
                    });
                }
            }

            return modules;
        }
    }
}
