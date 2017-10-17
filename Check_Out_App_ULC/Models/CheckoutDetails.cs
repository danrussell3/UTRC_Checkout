using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Check_Out_App_ULC.Models
{
    public class CheckoutDetails : tb_CSUCheckoutCheckin
    {
        #region Members

        public string ENAME { get; set; }
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string ItemDescription { get; set; }
        public string ItemSerialNumber { get; set; }
        public string Itemupcfk { get; set; }

        #endregion
    }

    public class WaiverDetails : tb_CSUStudent
    {

    }

}