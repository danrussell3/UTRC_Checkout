using System;
using System.Net.Mail;
using System.Text;
using System.Web.Mvc;
using Check_Out_App_ULC.Models;

namespace Check_Out_App_ULC.Controllers
{
    public class EmailController : Controller
    {
        #region Constructors
        readonly SmtpClient smtp_client = new SmtpClient();
        readonly StringBuilder sb = new StringBuilder();
        Checkin_Checkout_Entities db = new Checkin_Checkout_Entities();
        private string text;
        readonly MailMessage mailMessage = new MailMessage();
        #endregion Constructors

        #region Public Functions
        
        // Sends the email
        private void SendMail(MailMessage message)
        {
            smtp_client.UseDefaultCredentials = true;
            smtp_client.Send(message);
        }

        // Creates HTML markup, then calls SendMail
        private void SendMailHtml(MailMessage message, string text)
        {
            sb.Clear();

            #region HTML Text Builder
            

            
            sb.Append(@"

<!DOCTYPE html>

<html lang=""en"" xmlns=""http://www.w3.org/1999/xhtml"">
<head>
    <meta charset=""utf-8"" />
    <title>Undeclared Technology Resource Center</title>
    <style>
    body {
        margin: 0;
        padding: 0;
        font-family: sans-serif;
        
    }

    .header img {
       height: 150px;
       

}

        

    .section {
//        margin: 25px auto;
//        padding: 10px 10px 20px 10px;
             

    }

      
         

     .footer {
        display: none;
        

    }

    

    

    
            
</style>
</head>
<body>
    <div class=""header"">
        
        
    </div>

    <div class=""section"">
        
"); ;

            sb.Append(text);

            sb.Append(@"

                              									
    </div>

    
</body>
</html>"
            );
            #endregion
            message.Body = sb.ToString();
            message.IsBodyHtml = true;
            message.BodyEncoding = System.Text.Encoding.ASCII;
            SendMail(mailMessage);
        }

        public void OverdueItemEmail(tb_CSUStudent student, tb_CSUCheckoutCheckin checkout, bool isTest = false)
        {
            mailMessage.To.Clear();
            mailMessage.From = new MailAddress("UTRC_checkout@mail.colostate.edu");
            mailMessage.To.Add(isTest ? SessionVariables.CurrentUser.EMAIL : student.EMAIL_ADDRESS);
            // add subject
            mailMessage.Subject =
                "Notice of OverDue Checkout from UTRC Checkout";
            // add body
            text =
                student.FIRST_NAME + " " + student.LAST_NAME + "," + "</br> </br>" +
                "Our records indicate that you checked out " + checkout.ItemUPCFK + " on " + checkout.CheckoutDate.Value.ToString("M") + " at " + checkout.CheckoutDate.Value.ToString("HH:mm tt") + "</br>"
                + checkout.ItemUPCFK + " was due back at " + checkout.DueDate.Value.ToString("f") + ". Late charges will be issued if the item is not reutrned within 24 hours of checkout time." + "</br>"
                + "</br>This includes a NON-REFUNDABLE fine of $5 per day, upto a maximum  of $25.00. Failure to return the item without all internal and external components intact, will be billed at its replacement charge plus a billing fee of $5.00."
                + "Replacement charges are assessed in addition to the maximum overdue fine. Replacement charges are: Laptops = $1,800, each accessory = $95.00." + "</br>"
                + "</br>If damage occurs to any laptop components or accessories, damage charges will be imposed as appropriate." + "</br>"
                + "To avoid further charges on your student account  please return the item to the  checkout desk." + "</br></br>"
                + " -UTRC Management";

            // now send it

            SendMailHtml(mailMessage, text);
        }

        public void CheckoutEmail(tb_CSUStudent student, tb_CSUCheckoutCheckin checkout, bool isTest = false)
        {
            mailMessage.To.Clear();
            mailMessage.From = new MailAddress("UTRC_checkout@mail.colostate.edu");
            mailMessage.To.Add(isTest ? SessionVariables.CurrentUser.EMAIL : student.EMAIL_ADDRESS);
            // add subject
            mailMessage.Subject =
                checkout.ItemUPCFK + " Checked-out from UTRC Checkout";
            // add body
            text =
                student.FIRST_NAME + " " + student.LAST_NAME + "," + "</br> </br>" +
                "Our records indicate that you checked out " + checkout.ItemUPCFK + " on " + checkout.CheckoutDate.Value.ToString("M") + " at " + checkout.CheckoutDate.Value.ToString("h:mm:ss tt") + "</br>"
                + "<h3>" + checkout.ItemUPCFK + " is due back at " + checkout.DueDate.Value.ToString("dddd, MMMM dd, yyyy h:mm tt") + ".</h3> Late charges will be issued if the item is not reutrned on time."
                + "This includes a NON-REFUNDABLE fine of $5 per day, up to a maximum  of $25.00. Failure to return " + checkout.ItemUPCFK + " within 6 days, or returning the item without all internal and external components intact,"
                + "will cause your student account to be billed for the cost of " + checkout.ItemUPCFK + "'s replacement plus a non-refundable billing fee of $5.00."
                + "Replacement charges are assessed in addition to the maximum overdue fine. Replacement charges can be upto: Laptops = $1,800, Accessory = $95.00."
                + "If damage occurs to any laptop components or accessories, damage charges will be imposed as appropriate." + "</br> </br>"

                + " -UTRC Management </br> http://utrc.casa.colostate.edu/";

            // now send it
            SendMailHtml(mailMessage, text);
        }

        public void BanEmail(bool? isPermBan, tb_CSUStudent banStu, bool isTest = false)
        {
            mailMessage.To.Clear();
            mailMessage.From = new MailAddress("UTRC_checkout@mail.colostate.edu");
            mailMessage.To.Add(isTest ? SessionVariables.CurrentUser.EMAIL : banStu.EMAIL_ADDRESS);
            // add subject
            mailMessage.Subject = isPermBan == true
                ? "Notice of Permanent Ban from UTRC Checkout"
                : "Notice of Temporary Ban from UTRC Checkout";
            // add body
            text = isPermBan == true
                ? banStu.FIRST_NAME + ",  </br></br>" +
                  "Our records indicate that you either failed to return the item(s) you checked out from the" +
                  " Undeclared Technology Resource Center (UTRC) within the six hour checkout limit or the item(s) you" +
                  " returned was damaged. Due to either the severity of the broken item(s) or the length of time you had" +
                  " the item(s) you have been permanently banned from future use of our services. If you feel this is" +
                  " an error and wish to contest the ban you can set up a time to meet with one of our managers" +
                  " to discuss the issue further. If you would like to contest please respond back to this email" +
                  " and we will reply to setup a time to meet with you." +
                  " </br></br> -UTRC Management </br> http://utrc.casa.colostate.edu/"
                : banStu.FIRST_NAME + ", </br></br>" +
                  "Our records indicate that you either failed to return the item(s) you checkout from the" +
                  " Undeclared Technology Resource Center (UTRC) within the six hour checkout limit or the item(s) you" +
                  " returned was damaged. Due to not complying with the terms of our agreement you have been temporarily" +
                  " banned from our system. To lift the ban you may come to the checkout location and talk to one of" +
                  " our staff members. We will lift the ban for you and you can utilize our services in the" +
                  " future." +
                  " </br></br> -UTRC Management </br> http://utrc.casa.colostate.edu/";
            SendMailHtml(mailMessage, text);
        }

        public void CheckinEmail(bool? isLate, bool? isfined, tb_CSUStudent student, tb_CSUCheckoutCheckin checkout, bool isTest = false)
        {
            mailMessage.To.Clear();
            mailMessage.From = new MailAddress("UTRC_checkout@mail.colostate.edu");
            mailMessage.To.Add(isTest ? SessionVariables.CurrentUser.EMAIL : student.EMAIL_ADDRESS);
            // add subject
            mailMessage.Subject =
                checkout.ItemUPCFK + " Check-in at UTRC Checkout";
            // add body
            if (isLate == true && isfined != true)
            {
                text = student.FIRST_NAME + " " + student.LAST_NAME + ", </br> </br>" +
                       "Thank you for returning " + checkout.ItemUPCFK + ". Unfortunately, this item was returned late." +
                       "</br></br> -UTRC Management </br> http://utrc.casa.colostate.edu/";
            }
            if (isfined == true)
            {
                text = student.FIRST_NAME + " " + student.LAST_NAME + ", </br></br>" +
                       "Thank you for returning " + checkout.ItemUPCFK +
                       ". Unfortunately, this item was returned late and a fine will be assessed to your student account. " +
                       "Fines are applied on the 15th of each month, and will appear on your account the following month." +
                       " </br></br> -UTRC Management </br> http://utrc.casa.colostate.edu/";
            }
            if (isfined != true && isLate != true)
            {
                text = student.FIRST_NAME + " " + student.LAST_NAME + ", </br></br>" +
                       "Thank you for returning " + checkout.ItemUPCFK + " on time. " +
                       "</br></br> -UTRC Management </br> http://utrc.casa.colostate.edu/";
            }
            // now send it
            SendMailHtml(mailMessage, text);
        }

        public void WaiverEmail(tb_CSUStudent student, bool isTest = false)
        {
            mailMessage.To.Clear();
            mailMessage.From = new MailAddress("UTRC_checkout@mail.colostate.edu");
            mailMessage.To.Add(isTest ? SessionVariables.CurrentUser.EMAIL : student.EMAIL_ADDRESS);
            // add subject
            mailMessage.Subject = "UTRC Waiver Agreement";
            text = student.FIRST_NAME + " " + student.LAST_NAME + ", </br> </br>" +
                   "Thank you for joining the UTRC checkout system! UTRC_checkout@mail.colostate.edu is the official email of the program, " +
                   "and will be sending you emails regarding your check-outs. You'll recieve an email shortly with the terms and conditions of the checkout program, " +
                   "and information about your first checkout. A digital copy of the waiver you signed will be provided in an email by SmartWaiver, and the waiver may be found here: " +
                   "http://utrc.casa.colostate.edu/waiver/" + "." +
                   "</br></br> -UTRC Management";

            SendMailHtml(mailMessage, text);
        }

        public void LateEmail(tb_CSUStudent student, tb_CSUCheckoutCheckin checkout, bool isTest = false)
        {
            mailMessage.To.Clear();
            mailMessage.From = new MailAddress("UTRC_checkout@mail.colostate.edu");
            mailMessage.To.Add(isTest ? SessionVariables.CurrentUser.EMAIL : student.EMAIL_ADDRESS);
            // add subject
            mailMessage.Subject = checkout.ItemUPCFK + " NOT RETURNED!!!";
            text = student.FIRST_NAME + " " + student.LAST_NAME + ", </br> </br>" +
                   "Our records indicate you have not returned " + checkout.ItemUPCFK + " by " + checkout.DueDate + " and our checkout desk staff has closed for the day. " +
                   "Be advised, you will be assesed a late fee on the item in accordance with the Laptop Agreement found here: " +
                   "http://utrc.casa.colostate.edu/waiver/" + "." +

                   "</br></br> -UTRC Management";

            SendMailHtml(mailMessage, text);
        }

        public void LongtermDue(tb_CSUStudent student, tb_CSUCheckoutCheckin checkout, bool isTest = false)
        {
            mailMessage.To.Clear();
            mailMessage.From = new MailAddress("UTRC_checkout@mail.colostate.edu");
            mailMessage.To.Add(isTest ? SessionVariables.CurrentUser.EMAIL : student.EMAIL_ADDRESS);
            // add subject
            mailMessage.Subject = checkout.ItemUPCFK + " Due Back Soon!";
            text = student.FIRST_NAME + " " + student.LAST_NAME + ", </br> </br>" +
                   "Our records indicate you have had " + checkout.ItemUPCFK + " on longterm checkout since " + checkout.CheckoutDate + " and the due date for the item is " + checkout.DueDate +
                   ". Please return the item before the due date or" +
                   " you will be assesed a late fee on the item in accordance with the Laptop Agreement found here: " +
                   "http://utrc.casa.colostate.edu/waiver/" + "." +

                   "</br></br> -UTRC Management";

            SendMailHtml(mailMessage, text);
        }

        public void WaitlistAddEmail(tb_CSUStudent student, bool isTest = false)
        {
            mailMessage.To.Clear();
            mailMessage.From = new MailAddress("UTRC_checkout@mail.colostate.edu");
            mailMessage.To.Add(isTest ? SessionVariables.CurrentUser.EMAIL : student.EMAIL_ADDRESS);
            // add subject
            mailMessage.Subject = "Added to Waitlist for Longterm Checkout";
            text = student.FIRST_NAME + " " + student.LAST_NAME + ", </br> </br>" +
                   "You were added to the waitlist for a longterm checkout at " + DateTime.Now + ". You will be" +
                   " notified by email when your item is ready, at which point you will have 48 hours to check it out. " +
                   "</br></br> -UTRC Management";

            SendMailHtml(mailMessage, text);
        }

        public void WaitlistRemoveEmail(tb_CSUStudent student, bool isTest = false)
        {
            mailMessage.To.Clear();
            mailMessage.From = new MailAddress("UTRC_checkout@mail.colostate.edu");
            mailMessage.To.Add(isTest ? SessionVariables.CurrentUser.EMAIL : student.EMAIL_ADDRESS);
            // add subject
            mailMessage.Subject = "Removed From Waitlist for Longterm Checkout";
            text = student.FIRST_NAME + " " + student.LAST_NAME + ", </br> </br>" +
                   "You were removed from the waitlist for longterm checkout at " + DateTime.Now + ". " +
                   "</br></br> -UTRC Management";

            SendMailHtml(mailMessage, text);
        }

        public void WaitlistRemoveAfter48HoursEmail(tb_CSUStudent student, bool isTest = false)
        {
            mailMessage.To.Clear();
            mailMessage.From = new MailAddress("UTRC_checkout@mail.colostate.edu");
            mailMessage.To.Add(isTest ? SessionVariables.CurrentUser.EMAIL : student.EMAIL_ADDRESS);
            // add subject
            mailMessage.Subject = "Removed From Waitlist for Longterm Checkout";
            text = student.FIRST_NAME + " " + student.LAST_NAME + ", </br> </br>" +
                   "You were previously notified that a longterm checkout laptop was available for you and that you had 48 hours " +
                   "to pick it up. Since you did not, you were removed from the waitlist at " + DateTime.Now + ". " +
                   "</br></br> -UTRC Management";

            SendMailHtml(mailMessage, text);
        }

        public void WaitlistItemReadyEmail(tb_CSUStudent student, bool isTest = false)
        {
            mailMessage.To.Clear();
            mailMessage.From = new MailAddress("UTRC_checkout@mail.colostate.edu");
            mailMessage.To.Add(isTest ? SessionVariables.CurrentUser.EMAIL : student.EMAIL_ADDRESS);
            // add subject
            mailMessage.Subject = "Your item is ready for Longterm Checkout";
            text = student.FIRST_NAME + " " + student.LAST_NAME + ", </br> </br>" +
                   "A laptop is ready for you to checkout longterm. You have 48 hours to check it out, after which " +
                   "you will be able to keep the computer for 2 weeks. You can pick it up from the UTRC checkout desk in the BSB during the " +
                   "following times:" +
                   "</br></br><strong>MONDAY: </strong>1:00pm-3:00pm" +
                   "</br><strong>TUESDAY: </strong>1:00pm-4:00pm" +
                   "</br><strong>THURSDAY: </strong>2:00pm-6:00pm" +
                   "</br><strong>FRIDAY: </strong>2:00pm-5:00pm" +
                   "</br></br>If you do not check it out within 48 hours, the item will go to the next person and you " +
                   "will be removed from the waitlist." +
                   "</br></br> -UTRC Management";

            SendMailHtml(mailMessage, text);
        }

        public void WaitlistReservationProblemEmail(tb_CSUStudent student, tb_CSULabInventoryItems item, bool isTest = false)
        {
            mailMessage.To.Clear();
            mailMessage.From = new MailAddress("UTRC_checkout@mail.colostate.edu");
            mailMessage.To.Add(isTest ? SessionVariables.CurrentUser.EMAIL : student.EMAIL_ADDRESS);
            // add subject
            mailMessage.Subject = "Your waitlist reservation had a problem";
            text = student.FIRST_NAME + " " + student.LAST_NAME + ", </br> </br>" +
                   "There was an issue with the available laptop and unfortunately it is no longer available for you to checkout. You are at the " +
                   "top of the waitlist and will be notified as soon as an item becomes available. If you have " +
                   "any questions, please feel free to contact a manager at the BSB checkout desk during the following times:" +
                   "</br></br><strong>MONDAY: </strong>1:00pm-3:00pm" +
                   "</br><strong>TUESDAY: </strong>1:00pm-4:00pm" +
                   "</br><strong>THURSDAY: </strong>2:00pm-6:00pm" +
                   "</br><strong>FRIDAY: </strong>2:00pm-5:00pm" +
                   "</br></br> -UTRC Management";

            SendMailHtml(mailMessage, text);
        }

        public void EndOfDayReport(string report, bool isTest = false)
        {
            // investigate sending an email by manager rights??

            mailMessage.To.Clear();
            mailMessage.From = new MailAddress("UTRC_checkout@mail.colostate.edu");
            if (isTest)
            {
                mailMessage.To.Add(SessionVariables.CurrentUser.EMAIL);
            }
            else
            {
                mailMessage.To.Add("cawahler@colostate.edu");
                mailMessage.To.Add("UTRC_Checkout@mail.colostate.edu");
                mailMessage.To.Add("Casa_Laptop_Checkout@mail.colostate.edu");
                mailMessage.To.Add("crkava@rams.colostate.edu");
                mailMessage.To.Add("bdelong3@rams.colostate.edu");
                mailMessage.To.Add("jdruss@colostate.edu");
                mailMessage.To.Add("mgbrake@colostate.edu");
                mailMessage.To.Add("charbert@rams.colostate.edu");
                
            }
            
            // add subject
            mailMessage.Subject = DateTime.Now.ToShortDateString() + " End of Day Report";

            SendMailHtml(mailMessage, report);
        }

        public void ErrorEmail(string student, bool isTest = false)
        {
            mailMessage.To.Clear();

        }

        #endregion Public Functions

        #region Test Functions

        public void testOverdueItemEmail()
        {
            tb_CSUStudent student = new tb_CSUStudent();
            student.FIRST_NAME = SessionVariables.CurrentUser.First_Name;
            student.LAST_NAME = SessionVariables.CurrentUser.Last_Name;
            tb_CSUCheckoutCheckin checkout = new tb_CSUCheckoutCheckin();
            checkout.ItemUPCFK = "Test Item";
            checkout.ItemIDFK = 1234;
            checkout.CheckoutDate = DateTime.Now;
            checkout.DueDate = DateTime.Now;
            OverdueItemEmail(student, checkout, true);
        }

        public void testCheckoutEmail()
        {
            tb_CSUStudent student = new tb_CSUStudent();
            student.FIRST_NAME = SessionVariables.CurrentUser.First_Name;
            student.LAST_NAME = SessionVariables.CurrentUser.Last_Name;
            tb_CSUCheckoutCheckin checkout = new tb_CSUCheckoutCheckin();
            checkout.CSU_IDFK = SessionVariables.CurrentUserId;
            checkout.ItemUPCFK = "Test Item";
            checkout.ItemIDFK = 1234;
            checkout.CheckoutDate = DateTime.Now;
            checkout.DueDate = DateTime.Now;
            CheckoutEmail(student, checkout, true);
        }

        public void testBanEmailTemp()
        {
            tb_CSUStudent student = new tb_CSUStudent();
            student.FIRST_NAME = SessionVariables.CurrentUser.First_Name;
            student.LAST_NAME = SessionVariables.CurrentUser.Last_Name;
            student.EMAIL_ADDRESS = SessionVariables.CurrentUser.EMAIL;
            BanEmail(false, student, true);
        }

        public void testBanEmailPerm()
        {
            tb_CSUStudent student = new tb_CSUStudent();
            student.FIRST_NAME = SessionVariables.CurrentUser.First_Name;
            student.LAST_NAME = SessionVariables.CurrentUser.Last_Name;
            student.EMAIL_ADDRESS = SessionVariables.CurrentUser.EMAIL;
            BanEmail(true, student, true);
        }

        public void testCheckinEmail()
        {
            tb_CSUStudent student = new tb_CSUStudent();
            student.FIRST_NAME = SessionVariables.CurrentUser.First_Name;
            student.LAST_NAME = SessionVariables.CurrentUser.Last_Name;
            tb_CSUCheckoutCheckin checkout = new tb_CSUCheckoutCheckin();
            checkout.ItemUPCFK = "Test Item";
            CheckinEmail(false, false, student, checkout, true);
        }

        public void testCheckinEmailLate()
        {
            tb_CSUStudent student = new tb_CSUStudent();
            student.FIRST_NAME = SessionVariables.CurrentUser.First_Name;
            student.LAST_NAME = SessionVariables.CurrentUser.Last_Name;
            tb_CSUCheckoutCheckin checkout = new tb_CSUCheckoutCheckin();
            checkout.ItemUPCFK = "Test Item";
            CheckinEmail(true, false, student, checkout, true);
        }

        public void testCheckinEmailLateFine()
        {
            tb_CSUStudent student = new tb_CSUStudent();
            student.FIRST_NAME = SessionVariables.CurrentUser.First_Name;
            student.LAST_NAME = SessionVariables.CurrentUser.Last_Name;
            tb_CSUCheckoutCheckin checkout = new tb_CSUCheckoutCheckin();
            checkout.ItemUPCFK = "Test Item";
            CheckinEmail(true, true, student, checkout, true);
        }

        public void testWaiverEmail()
        {
            tb_CSUStudent student = new tb_CSUStudent();
            student.FIRST_NAME = SessionVariables.CurrentUser.First_Name;
            student.LAST_NAME = SessionVariables.CurrentUser.Last_Name;
            WaiverEmail(student, true);
        }

        public void testLateEmail()
        {
            tb_CSUStudent student = new tb_CSUStudent();
            student.FIRST_NAME = SessionVariables.CurrentUser.First_Name;
            student.LAST_NAME = SessionVariables.CurrentUser.Last_Name;
            tb_CSUCheckoutCheckin checkout = new tb_CSUCheckoutCheckin();
            checkout.ItemUPCFK = "Test Item";
            checkout.DueDate = DateTime.Now;
            LateEmail(student, checkout, true);
        }

        public void testLongtermDue()
        {
            tb_CSUStudent student = new tb_CSUStudent();
            student.FIRST_NAME = SessionVariables.CurrentUser.First_Name;
            student.LAST_NAME = SessionVariables.CurrentUser.Last_Name;
            tb_CSUCheckoutCheckin checkout = new tb_CSUCheckoutCheckin();
            checkout.ItemUPCFK = "Test Item";
            checkout.DueDate = DateTime.Now;
            checkout.CheckoutDate = DateTime.Now;
            LongtermDue(student, checkout, true);
        }

        public void testEndOfDayReport(string report)
        {
            //string report = "Today was fantastic! This is a sample report for testing purposes.";
            EndOfDayReport(report);
        }
        
        #endregion Test Functions
    }

}