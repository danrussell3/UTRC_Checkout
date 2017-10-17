using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Check_Out_App_ULC.Models
{
    [MetadataType(typeof(tb_CSUStudentAnnotation))]
    public partial class tb_CSUStudent
    {

    }
    internal sealed class tb_CSUStudentAnnotation
    {
        #region Members

        // [Required(ErrorMessage = "{0} is required")]
        [Display(Name = "CSU ID", Prompt = "Enter CSU ID", Description = "CSU ID")]
        public string CSU_ID { get; set; }


        //[Required(ErrorMessage = "{0} is required")]
        [Display(Name = "ENAME", Prompt = "Enter ENAME", Description = "ENAME")]
        public string ENAME { get; set; }

        //[Required(ErrorMessage = "{0} is required")]
        [Display(Name = "First Name", Prompt = "Enter First Name", Description = "First Name")]
        public string FIRST_NAME { get; set; }

        //[Required(ErrorMessage = "{0} is required")]
        [Display(Name = "Last Name", Prompt = "Enter Last Name", Description = "Last Name")]
        public string LAST_NAME { get; set; }

       // [Required(ErrorMessage = "{0} is required")]
        [Display(Name = "Email", Prompt = "Enter Email", Description = "Email Address")]
        public string EMAIL_ADDRESS { get; set; }

        //[Required(ErrorMessage = "{0} is required")]
        //[MinLength(10, ErrorMessage = "{0} should have 10 characters")]
        [Display(Name = "Phone #", Prompt = "Enter Phone #", Description = "Phone #")]
        public string PHONE { get; set; }


        #endregion
    }

}