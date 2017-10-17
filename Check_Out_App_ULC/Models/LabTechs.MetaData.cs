using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Check_Out_App_ULC.Models
{
    [MetadataType(typeof(tb_CSULabTechsAnnotaion))]

    public partial class tb_CSUlabTechs
    {

    }
    internal sealed class tb_CSULabTechsAnnotaion
    {
        #region Members

        //automatically assigned user ID, count + 1
        [Display(Name = "User ID#", Prompt = "User's ID #", Description = "User ID")]
        public int UserID { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [Display(Name = "Location", Prompt = "BSB or LSC", Description = "Location")]
        public string LocationId { get; set; }

        [Display(Name = "First Name", Prompt = "First Name", Description = "First Name")]
        public string First_Name { get; set; }

        [Display(Name = "Last Name", Prompt = "Last Name", Description = "Last Name")]
        public string Last_Name { get; set; }

        [Display(Name = "CSU ID #", Prompt = "CSU ID #", Description = "CSU ID #")]
        public string CSU_ID { get; set; }

        [Display(Name = "Email", Prompt = "Email", Description = "Email")]
        public string EMAIL { get; set; }

        [Display(Name = "Phone", Prompt = "Phone", Description = "Phone")]
        public int PHONE { get; set; }

        [Display(Name = "Ename", Prompt = "Ename", Description = "Ename")]
        public int ENAME { get; set; }

        [Display(Name = "User", Prompt = "is User", Description = "LabTech Employee")]
        public bool UserRights { get; set; }

        #endregion
    }

}