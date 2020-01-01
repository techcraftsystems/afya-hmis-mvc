using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using AfyaHMIS.Extensions;
using AfyaHMIS.Models;
using AfyaHMIS.Models.Rooms;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AfyaHMIS.Service
{
    public interface ICoreService {
        public List<Modules> GetApplicationModules();

        public List<SelectListItem> GetIEnumerable(string query);
        public List<SelectListItem> GetClientCodesIEnumerable();
        public List<SelectListItem> GetRoomsIEnumerable();
        public List<SelectListItem> GetRoomsIEnumerable(RoomType Type);
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

        public List<SelectListItem> GetClientCodesIEnumerable() {
            return GetIEnumerable("SELECT cl_idnt, cl_code+' :: '+cl_name FROM ClientCodes ORDER BY cl_code, cl_name");
        }

        public List<SelectListItem> GetRoomsIEnumerable() {
            return GetIEnumerable("SELECT rt_idnt, rt_type FROM RoomType WHERE rt_void=0");
        }

        public List<SelectListItem> GetRoomsIEnumerable(RoomType type) {
            return GetIEnumerable("SELECT rm_idnt, rm_room FROM Rooms WHERE rm_void=0 AND rm_type=" + type.Id);
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
