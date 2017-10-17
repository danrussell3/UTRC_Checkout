using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Check_Out_App_ULC.Models
{
    [MetadataType(typeof(tb_CSULabInventoryItemsAnnotation))]

    public partial class tb_CSULabInventoryItems
    {

    }
    internal sealed class tb_CSULabInventoryItemsAnnotation
    {
        #region Members

        [Display(Name = "Item ID", Prompt = "Enter Item ID", Description = "Item ID")]
        public int ItemId { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [Display(Name = "Item UPC (Barcode)", Prompt = "Enter Item UPC (Barcode)", Description = "Item UPC (barcode)")]
        public string ItemUPC { get; set; }

        [Display(Name = "Item Description", Prompt = "Enter Item Description", Description = "Item Description")]
        public string ItemDescription { get; set; }

        [Display(Name = "Item Serial", Prompt = "Enter Item Serial", Description = "Item Serial")]
        public string ItemSerialNumber { get; set; }

        [Display(Name = "Item Location", Prompt = "Enter Item Location", Description = "Item Location")]
        public int ItemLocationFK { get; set; }


        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }

        #endregion
    }


}