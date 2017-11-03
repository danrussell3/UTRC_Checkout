using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Data.Entity;
using System.Security.Permissions;
using System.ComponentModel.DataAnnotations;

namespace Check_Out_App_ULC.Models
{
    public class ViewModels
    {
        public class CkVw
        {
            #region Members
            [Display(Name = "CSU ID", Prompt = "CSU ID", Description = "CSU ID")]
            public string CsuId { get; set; }
            [Display(Name = "Item", Prompt = "Item", Description = "Item")]
            public string ItemUpc { get; set; }
            [Display(Name = "CheckOut Tech", Prompt = "CheckOut Tech", Description = "CheckOut Tech")]
            public string CkOutLabTech { get; set; }
            [Display(Name = "CheckOut Date", Prompt = "CheckOut Date", Description = "CheckOut Date")]
            public DateTime CkOutDt { get; set; }
            [Display(Name = "Checkin Tech", Prompt = "Checkin Tech", Description = "Checkin Tech")]
            public string CkInLabTech { get; set; }
            [Display(Name = "Checkin Date", Prompt = "Checkin Date", Description = "Checkin Date")]
            public DateTime? CkInDt { get; set; }
            [Display(Name = "Student Name", Prompt = "Name", Description = "Name")]
            public string Name { get; set; }
            [Display(Name = "EName", Prompt = "EName", Description = "EName")]
            public string Ename { get; set; }
            [Display(Name = "LongTerm", Prompt = "LongTerm", Description = "LongTerm")]
            public bool? LongTerm { get; set; }
            [Display(Name = "Waitlisted HP", Prompt = "Waitlisted HP", Description = "Waitlisted HP")]
            public bool? WaitlistHP { get; set; }
            [Display(Name = "Waitlisted Mac", Prompt = "Waitlisted Mac", Description = "Waitlisted Mac")]
            public bool? WaitlistMac { get; set; }
            [Display(Name = "DueDate", Prompt = "DueDate", Description = "DueDate")]
            [DataType(DataType.Date)]
            public DateTime? DueDate { get; set; }
            [Display(Name = "CheckOut Location", Prompt = "CheckOut Location", Description = "CheckOut Location")]
            public string CkOutLoc { get; set; }
            [Display(Name = "Checkin Location", Prompt = "Checkin Location", Description = "Checkin Location")]
            public string CkInLoc { get; set; }
            [Display(Name = "CheckOut ID#", Prompt = "CheckOut ID#", Description = "CheckOut ID#")]
            public int CkOutId { get; set; }
            #endregion
        }

        public class Checkout
        {
            #region Members
            public string csuID { get; set; }
            public string upc { get; set; }
            public string upc1 { get; set; }
            public string upc2 { get; set; }
            #endregion
        }
        public class StudentsLookup
        {
            #region Members
            public string Firstname { get; set; }
            public string Lastname { get; set; }
            public string ename { get; set; }
            public string csuid { get; set; }
            public bool waiverSigned { get; set; }
            public tb_CSUStudent matched_student { get; set; }
            public List<tb_CSUStudent> matching_external_students { get; set; }
            #endregion
        }

        public class BanUserView
        {
            #region Members
            [Display(Name = "CSU ID", Prompt = "CSU ID", Description = "CSU ID")]
            public string CsuId { get; set; }
            [Display(Name = "Banned", Prompt = "Ban", Description = "ban")]
            public bool IsBanned { get; set; }
            [Display(Name = "Unbannable?", Prompt = "PermBan", Description = "permban")]
            public bool IsPermBanned { get; set; }
            [Display(Name = "Date Banned", Prompt = "Date Banned", Description = "Date Banned")]
            public DateTime? DateBanned { get; set; }
            [Display(Name = "Date UnBanned", Prompt = "Date UnBanned", Description = "Date UnBanned")]
            public DateTime? DateUnBanned { get; set; }
            [Display(Name = "Student Name", Prompt = "Name", Description = "Name")]
            public string Name { get; set; }
            [Display(Name = "EName", Prompt = "EName", Description = "EName")]
            public string EName { get; set; }
            [Display(Name = "Banned By", Prompt = "BannedBy", Description = "BannedBy")]
            public string BannedBy { get; set; }
            [Display(Name = "Reason", Prompt = "Reason", Description = "Reason")]
            public string BanReason { get; set; }
            #endregion
        }

        public class LongtermWaitlistView
        {
            #region Members
            [Display(Name = "CSU ID", Prompt = "CSU ID", Description = "CSU ID")]
            public string CsuId { get; set; }
            [Display(Name = "EName", Prompt = "EName", Description = "EName")]
            public string EName { get; set; }
            [Display(Name = "Student Name", Prompt = "Name", Description = "Name")]
            public string Name { get; set; }
            [Display(Name = "Waitlisted", Prompt = "Waitlisted", Description = "Waitlisted")]
            public DateTime? Waitlisted { get; set; }
            [Display(Name = "Reason", Prompt = "Reason", Description = "Reason")]
            public string WaitlistReason { get; set; }
            [Display(Name = "Type", Prompt = "Type", Description = "Type")]
            public string WaitlistType { get; set; }
            [Display(Name = "Notified", Prompt = "Notified", Description = "Notified")]
            public DateTime? WaitlistNotified { get; set; }
            #endregion
        }

        public class SlingArticlesView
        {
            #region Members
            [Display(Name = "Post Content", Prompt = "Post Content", Description = "Post Content")]
            public string PostContent { get; set; }
            [Display(Name = "Posted", Prompt = "Posted", Description = "Posted")]
            public DateTime Posted { get; set; }
            [Display(Name = "User ID", Prompt = "User ID", Description = "User ID")]
            public string UserId { get; set; }
            [Display(Name = "Posted By", Prompt = "Posted By", Description = "Posted By")]
            public string PostedBy { get; set; }
            [Display(Name = "Post ID", Prompt = "Post ID", Description = "Post ID")]
            public string PostId { get; set; }
            [Display(Name = "Comments", Prompt = "Comments", Description = "Comments")]
            public List<Sling.SlingArticleComments> PostComments { get; set; }

            #endregion
        }

        #region Public Functions
        public bool IsValidUser(string id)
        {
            var ent = new HeraStudents_Entities();
            var valid = ent.v_CSUG_DIRECTORY_ALL_LOCAL_No_Dupes_forCheckinCheckout.Select(s => s.CSU_ID == id).Any();
            return valid;
        }

        public bool IsBannedUser(string id)
        {
            var ent = new Checkin_Checkout_Entities();
            var tbValid = ent.tb_BannedUserTable.FirstOrDefault(s => s.CSU_ID == id && s.isBanned == true);
            return tbValid != null;
            
        }

        public bool IsPermBannedUser(string id)
        {
            var ent = new Checkin_Checkout_Entities();
            var tbValid = ent.tb_BannedUserTable.FirstOrDefault(s => s.CSU_ID == id && s.isPermBanned == true);
            return tbValid != null;
            
        }

        #endregion

        /*
                private void textBox1_TextChanged(object sender, EventArgs e)

                {


                }
       
        public void CreateLocalDb()
        {
            if (HttpContext.Current.Session["startAt"] == null)
            {
                HttpContext.Current.Session["startAt"] = "";
            }

            var startAt = Convert.ToString(HttpContext.Current.Session["startAt"]);

            while (startAt != "999999999")
            {


                var dbCsuCheckinCheckout = new Checkin_Checkout_Entities();
                var dbHera = new HeraStudents_Entities();

                var q = dbHera.v_CSUG_DIRECTORY_ALL_LOCAL_No_Dupes_forCheckinCheckout.OrderBy(s => s.CSU_ID).Take(1000);
                foreach (var heraStudents in q
                )

                {
                    startAt = heraStudents.CSU_ID;
                    var csuStudents = new tb_CSUStudent
                    {
                        CSU_ID = heraStudents.CSU_ID,
                        ENAME = heraStudents.ENAME,
                        FIRST_NAME = heraStudents.FIRST_NAME,
                        LAST_NAME = heraStudents.LAST_NAME,
                        PHONE = heraStudents.PHONE,
                        EMAIL_ADDRESS = heraStudents.EMAIL_ADDRESS
                    };


                    using (var dbContext = new Checkin_Checkout_Entities())
                    {
                        dbContext.tb_CSUStudent.Add(csuStudents);
                        // _dbContext.Entry(_csuStudents).State = EntityState.Modified;
                        try
                        {
                            dbContext.SaveChanges();
                        }
                        catch
                        {
                            // ignored
                        }
                        dbContext.Dispose();
                    }
                }

                if (q.Count() < 1000)
                {
                    startAt = "999999999";
                }
                HttpContext.Current.Session["startAt"] = startAt;
                dbCsuCheckinCheckout.tb_CSUStudent.SqlQuery(@"
DELETE
  FROM [UTRC_Checkout].[dbo].[tb_CSUStudent]
  WHERE CSUStudentID <> (SELECT TOP 1 CSUStudentID FROM [UTRC_Checkout].[dbo].[tb_CSUStudent] s2 WHERE s2.CSU_ID = [UTRC_Checkout].[dbo].[tb_CSUStudent].CSU_ID ORDER BY CSUStudentID) ");
            }


        } 
        public void DeleteLocalDB()
        {
            var deleteStudents = new CSUCheckinCheckoutContext();
            // _deleteStudents.CSUStudent.RemoveRange(_deleteStudents.CSUStudent);   
            using (var context = new DbContext("Checkin_Checkout_Entities"))
            {
                var ctx = ((System.Data.Entity.Infrastructure.IObjectContextAdapter)context).ObjectContext;
                ctx.ExecuteStoreCommand("DELETE FROM [tb_csustudent] ", "tb_CSUStudent");

            }
        }*/
    }



}

