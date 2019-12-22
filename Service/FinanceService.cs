using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using AfyaHMIS.Extensions;
using AfyaHMIS.Models.Concepts;
using AfyaHMIS.Models.Finances;
using AfyaHMIS.Models.Patients;
using AfyaHMIS.Models.Persons;
using AfyaHMIS.Models.Registrations;
using AfyaHMIS.Models.Rooms;

namespace AfyaHMIS.Service
{
	public interface IFinanceService
    {
        public ClientCodeRates GetRoomsBillableRate(Room room, ClientCode code);
        public ClientCodeRates GetClientCodeBillableRate(ClientCode code, BillableService service);

        public List<Bills> GetBillingCashierQueue(DateTime start, DateTime stop, BillsFlag flag);

        public Bills SaveBill(Bills bill);
        public Bills UpdateBillWaiver(Bills bill);
        public Bills UpdateBillProcess(Bills bill);

        public BillsItem SaveBillsItem(BillsItem item);

    }

	public class FinanceService : IFinanceService 
    {
        public ClientCodeRates GetClientCodeBillableRate(ClientCode code, BillableService service)
        {
            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT bs_idnt, bs_concept, bs_service, bs_amount, bs_description, ISNULL(cr_rate, bs_amount)x FROM BillableService LEFT OUTER JOIN ClientCodesRates ON bs_idnt=cr_service AND cr_code=" + code.Id + " WHERE bs_idnt=" + service.Id);
            if (dr.Read())
                return new ClientCodeRates {
                    Service = new BillableService
                    {
                        Id = Convert.ToInt64(dr[0]),
                        Concept = new Concept { Id = Convert.ToInt64(dr[1])},
                        Name = dr[2].ToString(),
                        Amount = Convert.ToDouble(dr[3]),
                        Description = dr[4].ToString()
                    },
                    Code = code,
                    Amount = Convert.ToDouble(dr[5]),
                };

            return new ClientCodeRates();
        }

        public ClientCodeRates GetRoomsBillableRate(Room room, ClientCode code)
        {
            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT bs_idnt, bs_concept, bs_service, bs_amount, bs_description, ISNULL(cr_rate, bs_amount)x FROM Rooms INNER JOIN BillableService ON rm_service=bs_idnt LEFT OUTER JOIN ClientCodesRates ON bs_idnt=cr_service AND cr_code=" + code.Id + " WHERE rm_idnt=" + room.Id);
            if (dr.Read())
                return new ClientCodeRates
                {
                    Service = new BillableService
                    {
                        Id = Convert.ToInt64(dr[0]),
                        Concept = new Concept { Id = Convert.ToInt64(dr[1]) },
                        Name = dr[2].ToString(),
                        Amount = Convert.ToDouble(dr[3]),
                        Description = dr[4].ToString()
                    },
                    Code = code,
                    Amount = Convert.ToDouble(dr[5]),
                };

            return new ClientCodeRates();
        }

        public List<Bills> GetBillingCashierQueue(DateTime start, DateTime stop, BillsFlag flag)
        {
            List<Bills> queue = new List<Bills>();
            string query = "WHERE CAST(bl_date AS DATE) BETWEEN '" + start + "' AND '" + stop + "'";
            if (flag != null)
                query += " AND bl_flag=" + flag.Id;

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT bl_idnt, bl_amts, bl_date, bl_flag, bf_flag, pt_idnt, pt_uuid, pt_identifier, ps_idnt, ps_name, ps_gender, ps_dob FROM vBillingCashierQueue " + query + " ORDER BY bl_idnt");
            if (dr.HasRows) {
                while (dr.Read()) {
                    Bills bill = new Bills {
                        Id = Convert.ToInt64(dr[0]),
                        Amount = Convert.ToDouble(dr[1]),
                        Date = Convert.ToDateTime(dr[2]).ToString("dd/MM/yyyy"),
                        CreatedOn = Convert.ToDateTime(dr[2]),
                        Flag = new BillsFlag { 
                            Id = Convert.ToInt64(dr[3]),
                            Name = dr[4].ToString(),
                        },
                        Visit = new Visit {
                            Patient = new Patient {
                                Id = Convert.ToInt64(dr[5]),
                                Uuid = dr[6].ToString(),
                                Identifier = dr[7].ToString(),
                                Person = new Person {
                                    Id = Convert.ToInt64(dr[8]),
                                    Name = dr[9].ToString(),
                                    Gender = dr[10].ToString(),
                                    DateOfBirth = Convert.ToDateTime(dr[11])
                                }
                            }
                        }
                    };

                    bill.Visit.Patient.GetAge();

                    queue.Add(bill);
                }
            }

            return queue;
        }

        public Bills SaveBill(Bills bill)
        {
            SqlServerConnection conn = new SqlServerConnection();
            bill.Id = conn.SqlServerUpdate("DECLARE @idnt INT=" + bill.Id + ", @visit INT=" + bill.Visit.Id + ", @dept INT=" + bill.Department.Id + ", @amount FLOAT=" + bill.Amount + ", @user INT=" + bill.CreatedBy.Id + ", @notes NVARCHAR(MAX)='" + bill.Notes + "'; IF NOT EXISTS (SELECT bl_idnt FROM Bills WHERE bl_idnt=@idnt) BEGIN INSERT INTO Bills (bl_visit, bl_dept, bl_amount, bl_created_by, bl_notes) output INSERTED.bl_idnt VALUES (@visit, @dept, @amount, @user, @notes) END ELSE BEGIN UPDATE Bills SET bl_visit=@visit, bl_dept=@dept, bl_amount=@amount, bl_notes=@notes output INSERTED.bl_idnt WHERE bl_idnt=@idnt END");

            return bill;
        }

        public Bills UpdateBillWaiver(Bills bill)
        {
            SqlServerConnection conn = new SqlServerConnection();
            conn.SqlServerUpdate("DECLARE @idnt INT=" + bill.Id + ", @amount FLOAT=" + bill.Waiver + ", @user INT=" + bill.WaivedBy.Id + ", @reason NVARCHAR(MAX)='" + bill.WaiverReason + "'; UPDATE Bills SET bl_waiver=@amount, bl_waived_on=GETDATE(), bl_waived_by=@user, bl_waiver_reason=@reason WHERE bl_idnt =@idnt");

            return bill;
        }

        public Bills UpdateBillProcess(Bills bill)
        {
            SqlServerConnection conn = new SqlServerConnection();
            conn.SqlServerUpdate("DECLARE @idnt INT=" + bill.Id + ", @flag INT=" + bill.Flag.Id + ", @user INT=" + bill.ProcessedBy.Id + "; UPDATE Bills SET bl_flag=@flag, bl_processed_on=GETDATE(), bl_processed_by=@user WHERE bl_idnt =@idnt");

            return bill;
        }

        public BillsItem SaveBillsItem(BillsItem item)
        {
            SqlServerConnection conn = new SqlServerConnection();
            item.Id = conn.SqlServerUpdate("DECLARE @idnt INT=" + item.Id + ", @bill INT=" + item.Bill.Id + ", @service INT=" + item.Service.Id + ", @amount FLOAT=" + item.Amount + ", @user INT=" + item.CreatedBy.Id + ", @desc NVARCHAR(MAX)='" + item.Description + "'; IF NOT EXISTS (SELECT bi_idnt FROM BillsItem WHERE bi_idnt=@idnt) BEGIN INSERT INTO BillsItem (bi_bill, bi_service, bi_amount, bi_created_by, bi_description) output INSERTED.bi_idnt VALUES (@bill, @service, @amount, @user, @desc) END ELSE BEGIN UPDATE BillsItem SET bi_service=@service, bi_amount=@amount, bi_description=@desc output INSERTED.bi_idnt WHERE bi_idnt=@idnt END");

            return item;
        }
    }
}
