using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
namespace Check_Out_App_ULC.Models
{
    public class CSUCheckinCheckoutContext:DbContext
    {
        #region Members
        public DbSet<tb_CSUStudent> CSUStudent { get; set; }
        //public DbSet<CkOutViewModel> CkdOutView { get; set; }
        #endregion
    }
}