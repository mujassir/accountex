using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Common
{
   public class VehicleInstallmentDetailExtra
    {
       public int Id { get; set; }
       public int CompanyId { get; set; }
       public bool IsDeleted { get; set; }
       public int VoucherNumber { get; set; }
       public decimal SalePrice { get; set; }
       public decimal Received { get; set; }
       public decimal Balance { get; set; }
       public string Color { get; set; }
       public string ChassisNo { get; set; }
       public string EngineNo { get; set; }
       public int InstalmentNo { get; set; }
       public decimal Amount { get; set; }
       public decimal RecievedAmount { get; set; }
       public decimal Discount { get; set; }

    }
}
