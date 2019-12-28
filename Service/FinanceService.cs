using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using AfyaHMIS.Extensions;
using AfyaHMIS.Models;
using AfyaHMIS.Models.Administrations;
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

        public List<Bills> GetBills(Patient patient, Visit visit, DateTime? start, DateTime? stop, BillingFlag flag, bool otherFlags = false);
        public List<Bills> GetBillingCashierQueue(DateTime start, DateTime stop, BillingFlag flag);

        public List<BillsItem> GetBillsItems(string bills);
        public List<BillsItem> GetBillsItems(Bills bill, bool ignoreVoid, bool ignoreProcessed, string conditions = "");
        public List<BillsDepartment> GetBillsDepartments(Patient patient, BillingFlag flag);

        public List<Invoice> GetInvoices(Patient patient);

        public bool GetBillProcessingStatus(Bills bill);

        public Bills SaveBill(Bills bill);
        public Bills UpdateBillWaiver(Bills bill);
        public Bills UpdateBillProcess(Bills bill);
        public Bills UpdateBillsAutoFlag(Bills bill);

        public BillsItem SaveBillsItem(BillsItem item);
        public BillsItem VoidBillsItem(BillsItem item);

        public Invoice SaveInvoice(Invoice invoice);
        public InvoiceDetails SaveInvoiceDetails(InvoiceDetails details);
        public void RemoveInvoiceDetail(InvoiceDetails dtls);
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

        public List<Bills> GetBills(Patient patient, Visit visit, DateTime? start, DateTime? stop, BillingFlag flag, bool otherFlags = false)
        {
            List<Bills> bills = new List<Bills>();
            string query = "";

            if (patient != null)
                query = "WHERE pt_idnt=" + patient.Id;
            if (start.HasValue && stop.HasValue)
                query += (string.IsNullOrEmpty(query) ? "WHERE " : " AND ") + "CAST(bl_created_on AS DATE) BETWEEN '" + start + "' AND '" + stop + "'";
            if (start.HasValue)
                query += (string.IsNullOrEmpty(query) ? "WHERE " : " AND ") + "CAST(bl_created_on AS DATE)='" + start + "'";
            if (stop.HasValue)
                query += (string.IsNullOrEmpty(query) ? "WHERE " : " AND ") + "CAST(bl_created_on AS DATE)='" + stop + "'";
            if (otherFlags)
                query += (string.IsNullOrEmpty(query) ? "WHERE " : " AND ") + "bl_flag<>0";
            else {
                if (flag != null)
                    query += (string.IsNullOrEmpty(query) ? "WHERE " : " AND ") + "bl_flag=" + flag.Id;
            }

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT bl_idnt, bl_amount, bl_paid, bl_waiver, bl_waiver_reason, bl_created_on, bl_processed_on, bl_waived_on, bl_notes, bf_idnt, bf_flag, dpt_idnt, dpt_name, vst_idnt, vst_type, cl_idnt, cl_code, cl_name, pt_idnt, pt_uuid, pt_identifier, pt_notes, ps_idnt, ps_name, ps_gender, ps_dob, ps_notes, bl_created_by, cb.usr_name, bl_processed_by, pb.usr_name, bl_waived_by, wb.usr_name FROM Bills INNER JOIN BillsFlag ON bl_flag=bf_idnt INNER JOIN Departments ON bl_dept=dpt_idnt INNER JOIN Visit ON bl_visit=vst_idnt INNER JOIN ClientCodes ON cl_idnt=vst_client_code INNER JOIN Patient ON vst_patient=pt_idnt INNER JOIN Person ON pt_person=ps_idnt INNER JOIN Users cb ON cb.usr_idnt=bl_created_by LEFT OUTER JOIN Users pb ON pb.usr_idnt=bl_processed_by LEFT OUTER JOIN Users wb ON wb.usr_idnt=bl_waived_by " + query + " ORDER BY bl_created_on DESC, bl_idnt");
            if (dr.HasRows) {
                while (dr.Read()) {
                    Bills bill = new Bills {
                        Id = Convert.ToInt64(dr[0]),
                        Amount = Convert.ToDouble(dr[1]),
                        Paid = Convert.ToDouble(dr[2]),
                        Waiver = Convert.ToDouble(dr[3]),
                        WaiverReason = dr[4].ToString(),
                        CreatedOn = Convert.ToDateTime(dr[5]),
                        ProcessedOn = dr[6].ToString() == "" ? (DateTime?)null : Convert.ToDateTime(dr[6]),
                        WaivedOn = dr[7].ToString() == "" ? (DateTime?)null : Convert.ToDateTime(dr[7]),
                        Notes = dr[8].ToString(),
                        Flag = new BillingFlag {
                            Id = Convert.ToInt64(dr[9]),
                            Name = dr[10].ToString()
                        },
                        Department = new Department {
                            Id = Convert.ToInt64(dr[11]),
                            Name = dr[12].ToString()
                        },
                        Visit = new Visit {
                            Id = Convert.ToInt64(dr[13]),
                            Type = new VisitType {
                                Id = Convert.ToInt64(dr[14]),
                            },
                            ClientCode = new ClientCode {
                                Id = Convert.ToInt64(dr[15]),
                                Code = dr[16].ToString(),
                                Name = dr[17].ToString(),
                            },
                            Patient = new Patient {
                                Id = Convert.ToInt64(dr[18]),
                                Uuid = dr[19].ToString(),
                                Identifier = dr[20].ToString(),
                                Notes = dr[21].ToString(),
                                Person = new Person {
                                    Id = Convert.ToInt64(dr[22]),
                                    Name = dr[23].ToString(),
                                    Gender = dr[24].ToString(),
                                    DateOfBirth = Convert.ToDateTime(dr[25]),
                                    Notes = dr[26].ToString()
                                }
                            }
                        },
                        CreatedBy = new Users {
                            Id = Convert.ToInt64(dr[27]),
                            Name = dr[28].ToString()
                        },
                        ProcessedBy = new Users {
                            Id = Convert.ToInt64(dr[29]),
                            Name = dr[30].ToString()
                        },
                        WaivedBy = new Users {
                            Id = Convert.ToInt64(dr[31]),
                            Name = dr[32].ToString()
                        }
                    };

                    bill.Balance = bill.Amount - bill.Paid - bill.Waiver;
                    bill.Date = bill.CreatedOn.ToString("dd/MM/yyyy");
                    bill.Visit.Patient.GetAge();
                    bills.Add(bill);
                }
            }


            return bills;
        }

        public List<Bills> GetBillingCashierQueue(DateTime start, DateTime stop, BillingFlag flag)
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
                        Flag = new BillingFlag { 
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

        public List<BillsItem> GetBillsItems(string bills) {
            return GetBillsItems(null, true, true, "bi_bill IN (" + bills + ")");
        }

        public List<BillsItem> GetBillsItems(Bills bill, bool ignoreVoid, bool ignoreProcessed, string conditions = "") {
            List<BillsItem> items = new List<BillsItem>();

            string q = "";
            if (bill != null)
                q = "WHERE bi_bill=" + bill.Id;
            if (ignoreVoid)
                q += (string.IsNullOrEmpty(q) ? "WHERE" : " AND") + " bi_void=0";
            if (ignoreProcessed)
                q += (string.IsNullOrEmpty(q) ? "WHERE" : " AND") + " bi_idnt NOT IN (SELECT id_bill_item FROM InvoiceDetails)";
            if (!string.IsNullOrEmpty(conditions))
                q += (string.IsNullOrEmpty(q) ? "WHERE" : " AND") +  " " + conditions;
            //Consider Left outer JOIN with large Data

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT bi_idnt, bi_quantity, bi_amount, bi_created_on, bi_description, bi_created_by, bs_idnt, bs_code, bs_service, bs_amount, bs_description, bs_concept, ct_name, bi_void, bi_void_on, bi_void_by, bi_void_reason, bl_idnt, bl_visit, bl_flag, bl_dept, bl_amount, bl_paid, bl_waiver, bl_waiver_reason, bl_notes FROM BillsItem INNER JOIN Bills ON bi_bill=bl_idnt INNER JOIN BillableService ON bi_service=bs_idnt LEFT OUTER JOIN Concept ON bs_concept=ct_idnt " + q);
            if (dr.HasRows) {
                while (dr.Read()) {
                    items.Add(new BillsItem {
                        Id = Convert.ToInt64(dr[0]),
                        Quantity = Convert.ToDouble(dr[1]),
                        Price = Convert.ToDouble(dr[2]),
                        CreatedOn = Convert.ToDateTime(dr[3]),
                        Description = dr[4].ToString(),
                        CreatedBy = new Users {
                            Id = Convert.ToInt64(dr[5])
                        },
                        Service = new BillableService {
                            Id = Convert.ToInt64(dr[6]),
                            Code = dr[7].ToString(),
                            Name = dr[8].ToString(),
                            Amount = Convert.ToDouble(dr[9]),
                            Description = dr[10].ToString(),
                            Concept = new Concept {
                                Id = Convert.ToInt64(dr[11]),
                                Name = dr[12].ToString()
                            }
                        },
                        Voided = Convert.ToBoolean(dr[13]),
                        VoidedOn = dr[14].ToString() == "" ? (DateTime?)null : Convert.ToDateTime(dr[14]),
                        VoidedBy = new Users { Id = Convert.ToInt64(dr[15]) },
                        VoidedReason = dr[16].ToString(),
                        Bill = new Bills {
                            Id = Convert.ToInt64(dr[17]),
                            Visit = new Visit { Id = Convert.ToInt64(dr[18]) },
                            Flag = new BillingFlag { Id = Convert.ToInt64(dr[19]) },
                            Department = new Department { Id = Convert.ToInt64(dr[20]) },
                            Amount = Convert.ToDouble(dr[21]),
                            Paid = Convert.ToDouble(dr[22]),
                            Waiver = Convert.ToDouble(dr[23]),
                            WaiverReason = dr[24].ToString(),
                            Notes = dr[25].ToString()
                        }
                    });
                }
            }

            return items;
        }

        public List<BillsDepartment> GetBillsDepartments(Patient patient, BillingFlag flag) {
            List<BillsDepartment> depts = new List<BillsDepartment>();
            string query = "WHERE vst_patient=" + patient.Id;
            if (flag != null)
                query += " AND bl_flag=" + flag.Id;

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT TOP(6) dpt_idnt, dpt_name, ISNULL(bl_count,0)bl_count, ISNULL(bl_amt,0)bl_amt FROM Departments LEFT OUTER JOIN (SELECT bl_dept, COUNT(*) bl_count, SUM(bl_amount-bl_paid-bl_waiver) bl_amt FROM Bills INNER JOIN Visit ON vst_idnt=bl_visit " + query + " GROUP BY bl_dept) As Foo ON bl_dept=dpt_idnt ORDER BY bl_amt DESC, dpt_name");
            if (dr.HasRows) {
                while (dr.Read()) {
                    depts.Add( new BillsDepartment {
                        Department = new Department {
                            Id = Convert.ToInt64(dr[0]),
                            Name = dr[1].ToString()
                        },
                        Count = Convert.ToDouble(dr[2]),
                        Total = Convert.ToDouble(dr[3]),
                    });
                }
            }

            return depts;
        }

        public List<Invoice> GetInvoices(Patient patient)
        {
            List<Invoice> invoices = new List<Invoice>();

            string query = "";
            if (patient != null)
                query = "WHERE inv_patient=" + patient.Id;

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT inv_idnt, inv_created_by, inv_created_on, inv_notes, inv_amount, ISNULL(ip_tendered,0) inv_paid, bf_idnt, bf_flag, pt_idnt, pt_uuid, pt_identifier, pt_added_on, pt_added_by, pt_notes, pst_idnt, pst_status, ps_idnt, ps_name, ps_gender, ps_dob, ps_estimate, ps_added_on, ps_added_by, ps_notes FROM vInvoice INNER JOIN BillsFlag ON inv_flag=bf_idnt INNER JOIN Patient ON inv_patient=pt_idnt INNER JOIN PatientStatus ON pt_status=pst_idnt INNER JOIN Person ON pt_person=ps_idnt LEFT OUTER JOIN vInvoicePayment ON inv_idnt=ip_invoice " + query + " ORDER BY inv_created_on DESC, inv_idnt DESC");
            if (dr.HasRows) {
                while (dr.Read()) {
                    Invoice invoice = new Invoice {
                        Id = Convert.ToInt64(dr[0]),
                        CreatedBy = new Users { Id = Convert.ToInt64(dr[1]) },
                        CreatedOn = Convert.ToDateTime(dr[2]),
                        Notes = dr[3].ToString(),
                        Amount = Convert.ToDouble(dr[4]),
                        Paid = Convert.ToDouble(dr[5]),
                        Flag = new BillingFlag {
                            Id = Convert.ToInt64(dr[6]),
                            Name = dr[7].ToString()
                        },
                        Patient = new Patient {
                            Id = Convert.ToInt64(dr[8]),
                            Uuid = dr[9].ToString(),
                            Identifier = dr[10].ToString(),
                            AddedOn = Convert.ToDateTime(dr[11]),
                            AddedBy = new Users { Id = Convert.ToInt64(dr[12]) },
                            Notes = dr[13].ToString(),
                            Status = new PatientStatus {
                                Id = Convert.ToInt64(dr[14]),
                                Name = dr[15].ToString(),
                            },
                            Person = new Person {
                                Id = Convert.ToInt64(dr[16]),
                                Name = dr[17].ToString(),
                                Gender = dr[18].ToString(),
                                DateOfBirth = Convert.ToDateTime(dr[19]),
                                Estimate = Convert.ToBoolean(dr[20]),
                                AddedOn = Convert.ToDateTime(dr[21]),
                                AddedBy = new Users { Id = Convert.ToInt64(dr[22]) },
                                Notes = dr[23].ToString()
                            }
                        }
                    };

                    invoice.Date = invoice.CreatedOn.ToString("dd/MM/yyyy");
                    invoice.Balance = invoice.Amount - invoice.Paid;
                    invoice.Patient.GetAge();
                    invoices.Add(invoice);
                }
            }

            return invoices;
        }

        public bool GetBillProcessingStatus(Bills bill) {
            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("SELECT TOP(1) bli_inv FROM vBillingInvoices WHERE bli_bill=" + bill.Id);
            if (dr.Read())
                return true;
            return false;
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

        public Bills UpdateBillProcess(Bills bill) {
            SqlServerConnection conn = new SqlServerConnection();
            conn.SqlServerUpdate("DECLARE @idnt INT=" + bill.Id + ", @flag INT=" + bill.Flag.Id + ", @user INT=" + bill.ProcessedBy.Id + "; UPDATE Bills SET bl_flag=@flag, bl_processed_on=GETDATE(), bl_processed_by=@user WHERE bl_idnt =@idnt");

            return bill;
        }

        public Bills UpdateBillsAutoFlag(Bills bill) {
            SqlServerConnection conn = new SqlServerConnection();
            conn.SqlServerUpdate("DECLARE @idnt INT=" + bill.Id + ", @flag INT=0, @user INT=" + bill.ProcessedBy.Id + "; SELECT @flag=CASE WHEN COUNT(*)=1 THEN MAX(bi_state) ELSE 1 END FROM vBillingState WHERE bi_bill=@idnt GROUP BY bi_bill; UPDATE Bills SET bl_flag=@flag, bl_processed_on=GETDATE(), bl_processed_by=@user WHERE bl_flag=0 AND bl_idnt=@idnt");

            return bill;
        }

        public BillsItem SaveBillsItem(BillsItem item) {
            SqlServerConnection conn = new SqlServerConnection();
            item.Id = conn.SqlServerUpdate("DECLARE @idnt INT=" + item.Id + ", @bill INT=" + item.Bill.Id + ", @service INT=" + item.Service.Id + ", @amount FLOAT=" + item.Price + ", @user INT=" + item.CreatedBy.Id + ", @desc NVARCHAR(MAX)='" + item.Description + "'; IF NOT EXISTS (SELECT bi_idnt FROM BillsItem WHERE bi_idnt=@idnt) BEGIN INSERT INTO BillsItem (bi_bill, bi_service, bi_amount, bi_created_by, bi_description) output INSERTED.bi_idnt VALUES (@bill, @service, @amount, @user, @desc) END ELSE BEGIN UPDATE BillsItem SET bi_service=@service, bi_amount=@amount, bi_description=@desc output INSERTED.bi_idnt WHERE bi_idnt=@idnt END");

            return item;
        }

        public BillsItem VoidBillsItem(BillsItem item) {
            SqlServerConnection conn = new SqlServerConnection();
            conn.SqlServerUpdate("DECLARE @idnt INT=" + item.Id + ", @user INT=" + item.VoidedBy.Id + ", @reason NVARCHAR(MAX)='" + item.VoidedReason + "'; UPDATE BillsItem SET bi_void=1, bi_void_on=GETDATE(), bi_void_by=@user, bi_void_reason=@reason WHERE bi_idnt=@idnt");

            return item;
        }

        public Invoice SaveInvoice(Invoice invoice) {
            SqlServerConnection conn = new SqlServerConnection();
            invoice.Id = conn.SqlServerUpdate("DECLARE @idnt INT=" + invoice.Id + ", @patient INT=" + invoice.Patient.Id + ", @flag INT=" + invoice.Flag.Id + ", @user INT=" + invoice.CreatedBy.Id + ", @notes NVARCHAR(MAX)='" + invoice.Notes + "'; IF NOT EXISTS (SELECT inv_idnt FROM Invoice WHERE inv_idnt=@idnt) BEGIN INSERT INTO Invoice (inv_flag, inv_patient, inv_created_by, inv_notes) output INSERTED.inv_idnt VALUES (@flag, @patient, @user, @notes) END ELSE BEGIN UPDATE Invoice SET inv_notes=@notes output INSERTED.inv_idnt WHERE inv_idnt=@idnt END");

            return invoice;
        }

        public InvoiceDetails SaveInvoiceDetails(InvoiceDetails dtls) {
            SqlServerConnection conn = new SqlServerConnection();
            dtls.Id = conn.SqlServerUpdate("DECLARE @idnt INT=" + dtls.Id + ", @invs INT=" + dtls.Invoice.Id + ", @item INT=" + dtls.Item.Id + ", @user INT=" + dtls.CreatedBy.Id + ", @notes NVARCHAR(MAX)='" + dtls.Notes + "'; IF NOT EXISTS (SELECT id_idnt FROM InvoiceDetails WHERE id_idnt=@idnt OR id_bill_item=@item) BEGIN INSERT INTO InvoiceDetails (id_invoice, id_bill_item, id_added_by, id_notes) output INSERTED.id_idnt VALUES (@invs, @item, @user, @notes) END ELSE BEGIN UPDATE InvoiceDetails SET id_notes=@notes output INSERTED.id_idnt WHERE id_idnt=@idnt END");

            return dtls;
        }

        public void RemoveInvoiceDetail(InvoiceDetails dtls) {
            SqlServerConnection conn = new SqlServerConnection();
            conn.SqlServerUpdate("DELETE FROM InvoiceDetails WHERE id_idnt=" + dtls.Id);
        }
    }
}
