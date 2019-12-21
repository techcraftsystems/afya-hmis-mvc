using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using AfyaHMIS.Extensions;
using AfyaHMIS.Models.Rooms;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AfyaHMIS.Service
{
    public interface ICoreService {
        public List<SelectListItem> GetIEnumerable(string query);
        public List<SelectListItem> GetClientCodesIEnumerable();
        public List<SelectListItem> GetRoomsIEnumerable();
        public List<SelectListItem> GetRoomsIEnumerable(RoomType Type);
    }

    public class CoreService : ICoreService
    {
        public List<SelectListItem> GetIEnumerable(string query)
        {
            List<SelectListItem> ienumarable = new List<SelectListItem>();
            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect(query);
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    ienumarable.Add(new SelectListItem
                    {
                        Value = dr[0].ToString(),
                        Text = dr[1].ToString()
                    });
                }
            }

            return ienumarable;
        }

        public List<SelectListItem> GetClientCodesIEnumerable()
        {
            return GetIEnumerable("SELECT cl_idnt, cl_code+' :: '+cl_name FROM ClientCodes ORDER BY cl_code, cl_name");
        }

        public List<SelectListItem> GetRoomsIEnumerable()
        {
            return GetIEnumerable("SELECT rt_idnt, rt_type FROM RoomType WHERE rt_void=0");
        }

        public List<SelectListItem> GetRoomsIEnumerable(RoomType type)
        {
            return GetIEnumerable("SELECT rm_idnt, rm_room FROM Rooms WHERE rm_void=0 AND rm_type=" + type.Id);
        }
    }
}
