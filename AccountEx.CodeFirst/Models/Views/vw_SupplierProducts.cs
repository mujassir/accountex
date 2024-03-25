using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.CodeFirst.Models.Views
{
   public class vw_SupplierProducts
    {
       public int Id { get; set; }
       public int SupplierId { get; set; }
       public int ProductId { get; set; }
       public string SupplierName { get; set; }
       public string ProductName { get; set; }
       public bool IsDeleted { get; set; }
       public int CompanyId { get; set; }

    }
}
