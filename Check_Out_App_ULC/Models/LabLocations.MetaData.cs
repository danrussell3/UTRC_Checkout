using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Check_Out_App_ULC.Models
{
    [MetadataType(typeof(tb_CSULabLoctionsAnnotation))]

    public partial class tb_CSULabLocations
    {

    }
    internal sealed class tb_CSULabLoctionsAnnotation
    {
        #region Members

        [Display(Name = "Location ID", Prompt = "Enter Location ID", Description = "Location ID")]
        public int LocationId { get; set; }

        [Display(Name = "Location", Prompt = "Location", Description = "Location")]
        public string LocationNameAcronym { get; set; }

        [Display(Name = "Location Name", Prompt = "Enter Location Name", Description = "Location of the Item")]
        public string LocactionName { get; set; }

        #endregion
    }
}


