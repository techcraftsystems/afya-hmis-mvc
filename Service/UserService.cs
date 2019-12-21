using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Security.Claims;
using AfyaHMIS.Extensions;
using AfyaHMIS.Models;
using Microsoft.AspNetCore.Http;

namespace AfyaHMIS.Service
{
    public interface IUserService
    {
        public Users GetUser(long idnt);
        public Users GetUser(String username);
        public List<Users> GetUsers();
        public List<UsersRoles> GetRoles(Users user);
        public int CheckIfUserExists(Users user);

        public Users SaveUser(Users user);
        public void EnableAccount(Users user, bool opts = true);
        public void UpdateUserPassword(Users user, int toChange = 0);
        public void UpdateLastAccess(Users user);
        public void UpdateUsersFacilities(Users user, string facilities = "");
    }

    public class UserService : IUserService
    {
        private int Actor { get; set; }
        private string Username { get; set; }

        public UserService() { }
        public UserService(HttpContext context)
        {
            Actor = int.Parse(context.User.FindFirst(ClaimTypes.Actor).Value);
            Username = context.User.FindFirst(ClaimTypes.UserData).Value;
        }

        public Users GetUser(long idnt)
        {
            Users user = null;

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT log_user, log_username, usr_name, usr_email, log_tochange, log_enabled, log_admin_lvl, log_access_lvl, log_lastaccess, log_password, rl_idnt, rl_name FROM Login INNER JOIN Users ON log_user=usr_idnt INNER JOIN Roles ON log_admin_role=rl_idnt WHERE usr_idnt=" + idnt);
            if (dr.Read())
            {
                user = new Users
                {
                    Id = Convert.ToInt64(dr[0]),
                    Username = dr[1].ToString(),
                    Name = dr[2].ToString(),
                    Email = dr[3].ToString(),
                    ToChange = Convert.ToBoolean(dr[4]),
                    Enabled = Convert.ToBoolean(dr[5]),
                    AdminLevel = Convert.ToInt32(dr[6]),
                    AccessLevel = dr[7].ToString(),
                    LastSeen = Convert.ToDateTime(dr[8]).ToString("dd/MM/yyyy HH:mm"),
                    Password = dr[9].ToString(),
                    Role = new Roles(Convert.ToInt64(dr[10]), dr[11].ToString())
                };
            }

            return user;
        }

        public Users GetUser(String username)
        {
            Users user = null;

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT usr_idnt, usr_uuid, usr_name, usr_email, usr_notes, log_enabled, log_tochange, log_admin_lvl, log_access_lvl, log_username, log_password, log_lastaccess FROM Users INNER JOIN [Login] ON usr_idnt=log_user WHERE log_username='" + username + "'");
            if (dr.Read())
            {
                user = new Users
                {
                    Id = Convert.ToInt64(dr[0]),
                    Uuid = dr[1].ToString(),
                    Name = dr[2].ToString(),
                    Email = dr[3].ToString(),
                    Notes = dr[4].ToString(),
                    Enabled = Convert.ToBoolean(dr[5]),
                    ToChange = Convert.ToBoolean(dr[6]),
                    AdminLevel = Convert.ToInt64(dr[7]),
                    AccessLevel = dr[8].ToString(),
                    Username = dr[9].ToString(),
                    Password = dr[10].ToString(),
                    LastSeen = Convert.ToDateTime(dr[11]).ToString("dd/MM/yyyy")
                };

                if (Convert.ToDateTime(dr[11]).Year.Equals(1900))
                    user.LastSeen = "N/A";
            }

            return user;
        }

        public List<Users> GetUsers()
        {
            List<Users> users = new List<Users>();

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT log_user, log_username, usr_name, usr_email, log_tochange, log_enabled, log_admin_lvl, log_access_lvl, log_lastaccess, log_password, rl_idnt, rl_name FROM Login INNER JOIN Users ON log_user=usr_idnt INNER JOIN Roles ON log_admin_role=rl_idnt ORDER BY usr_name");
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    users.Add(new Users
                    {
                        Id = Convert.ToInt64(dr[0]),
                        Username = dr[1].ToString(),
                        Name = dr[2].ToString(),
                        Email = dr[3].ToString(),
                        ToChange = Convert.ToBoolean(dr[4]),
                        Enabled = Convert.ToBoolean(dr[5]),
                        AdminLevel = Convert.ToInt32(dr[6]),
                        AccessLevel = dr[7].ToString(),
                        LastSeen = Convert.ToDateTime(dr[8]).ToString("dd/MM/yyyy HH:mm"),
                        Password = dr[9].ToString(),
                        Role = new Roles(Convert.ToInt64(dr[10]), dr[11].ToString())
                    });
                }
            }

            return users;
        }

        public List<UsersRoles> GetRoles(Users user)
        {
            List<UsersRoles> roles = new List<UsersRoles>();

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT usrl_idnt, rl_idnt, rl_name FROM UsersRole INNER JOIN Roles ON usrl_role=rl_idnt WHERE usrl_user=" + user.Id + " ORDER BY rl_idnt");
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    roles.Add(new UsersRoles
                    {
                        User = user,
                        Id = Convert.ToInt64(dr[0]),
                        Role = new Roles
                        {
                            Id = Convert.ToInt64(dr[1]),
                            Name = dr[2].ToString()
                        }
                    });
                }
            }

            return roles;
        }

        public int CheckIfUserExists(Users user)
        {
            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT COUNT(*) FROM Login WHERE log_user<>" + user.Id + " AND log_username='" + user.Username + "'");
            if (dr.Read())
            {
                return Convert.ToInt32(dr[0]);
            }

            return 0;
        }


        /*Data Writers*/
        public Users SaveUser(Users user)
        {
            SqlServerConnection conn = new SqlServerConnection();
            user.Id = conn.SqlServerUpdate("DECLARE @idnt INT=" + user.Id + ", @name NVARCHAR(255)='" + user.Name + "', @email NVARCHAR(255)='" + user.Email + "', @user INT=" + Actor + "; IF NOT EXISTS (SELECT usr_idnt FROM Users WHERE usr_idnt=@idnt) BEGIN INSERT INTO Users (usr_name, usr_email, usr_created_by) output INSERTED.usr_idnt VALUES (@name, @email, @user) END ELSE BEGIN UPDATE Users SET usr_name=@name, usr_email=@email output INSERTED.usr_idnt WHERE usr_idnt=@idnt END");

            conn = new SqlServerConnection();
            conn.SqlServerUpdate("DECLARE @idnt INT=" + user.Id + ", @username NVARCHAR(255)='" + user.Username + "', @role INT=" + user.Role.Id + ", @level INT=" + user.AdminRole + "; IF NOT EXISTS (SELECT log_user FROM Login WHERE log_user=@idnt) BEGIN INSERT INTO Login (log_user, log_username, log_admin_role, log_admin_lvl) VALUES (@idnt, @username, @role, @level) END ELSE BEGIN UPDATE Login SET log_username=@username, log_admin_role=@role, log_admin_lvl=@level WHERE log_user=@idnt END");

            return user;
        }

        public void EnableAccount(Users user, bool opts = true)
        {
            SqlServerConnection conn = new SqlServerConnection();
            conn.SqlServerUpdate("UPDATE Login SET log_enabled='" + opts + "' WHERE log_user=" + user.Id);
        }

        public void UpdateUserPassword(Users user, int toChange = 0)
        {
            SqlServerConnection conn = new SqlServerConnection();
            conn.SqlServerUpdate("UPDATE Login SET log_tochange=" + toChange + ", log_password='" + user.Password + "' WHERE log_user=" + user.Id);
        }

        public void UpdateLastAccess(Users user)
        {
            SqlServerConnection conn = new SqlServerConnection();
            conn.SqlServerUpdate("UPDATE Login SET log_lastaccess=getdate() WHERE log_user=" + user.Id);
        }

        public void UpdateUsersFacilities(Users user, string facilities = "")
        {
            string AdditionalString = "";

            if (user.Role.Id.Equals(3))
                AdditionalString = "WHERE fc_region=" + user.AdminRole;
            else if (user.Role.Id.Equals(4))
                AdditionalString = "WHERE fc_agency=" + user.AdminRole;
            else if (user.Role.Id.Equals(5) || user.Role.Id.Equals(6))
                AdditionalString = "WHERE fc_idnt IN (" + facilities + ")";

            SqlServerConnection conn = new SqlServerConnection();
            conn.SqlServerUpdate("DELETE FROM UsersFacilities WHERE uf_user=" + user.Id);

            conn = new SqlServerConnection();
            conn.SqlServerUpdate("DECLARE @idnt INT=" + user.Id + ", @user INT=" + Actor + "; INSERT INTO UsersFacilities (uf_user, uf_added_by, uf_facility) SELECT @idnt, @user, fc_idnt FROM Facilities " + AdditionalString);
        }
    }
}