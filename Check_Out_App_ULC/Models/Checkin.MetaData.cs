using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Check_Out_App_ULC.Models
{
    [MetadataType(typeof(tb_CSUCheckoutCheckinAnnotation))]

    public partial class tb_CSUCheckoutCheckin
    {

    }
    internal sealed class tb_CSUCheckoutCheckinAnnotation
    {
        #region Members

        [Display(Name = "Checkout Checkin ID", Prompt = "Enter Checkout Checkin ID", Description = "Checkout Checkin ID")]
        public int CheckoutCheckinId { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [Display(Name = "CSU ID #", Prompt = "CSU ID #", Description = "CSU ID #")]
        public string CSU_IDFK { get; set; }

        [Display(Name = "Checked Out By", Prompt = "Checked Out By", Description = "Checked Out By")]
        public string CheckoutLabTech { get; set; }

        [Display(Name = "Late Checkin", Prompt = "Late Checkin", Description = "Late Checkin")]
        public string isLate { get; set; }

        [Display(Name = "Fine Applied", Prompt = "Fine Applied", Description = "Fine Applied")]
        public string isLongterm { get; set; }

        [Display(Name = "Longterm Checkout", Prompt = "Longterm Checkout", Description = "Longterm Checkout")]
        public string isHome { get; set; }

        [Display(Name = "Item #", Prompt = "Item ID", Description = "Item ID")]
        public int ItemUPCFK { get; set; }

        [Display(Name = "Item ID", Prompt = "Item ID", Description = "Item ID")]
        public int ItemIDFK { get; set; }

        [Display(Name = "Checkout Time", Prompt = "Enter Checkout Time", Description = "Checkout Time")]
        public DateTime CheckoutDate { get; set; }

        [Display(Name = "Checkin By", Prompt = "Enter Checkin By", Description = "Checkin By")]
        public string CheckinLabTech { get; set; }

        [Display(Name = "Checkin Time", Prompt = "Enter Checkin Time", Description = "Checkin Time")]
        public DateTime CheckinDate { get; set; }

        [Display(Name = "Checkout Location", Prompt = "Enter Checkout Location", Description = "Check-Out Location")]
        public string CheckoutLocationFK { get; set; }

        [Display(Name = "Checkin Location", Prompt = "Enter Checkin Location", Description = "Check-In Location")]
        public string CheckinLocationFK { get; set; }

        //[Display(Name = "isBanned", Prompt = "Banned user!", Description = "this is a banned user")]
        //public string isBanned { get; set; }

        #endregion
    }

}


