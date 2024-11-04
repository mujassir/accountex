using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Common
{
   public class ProductionDetail
    {

     
        public int InvoiceNumber { get; set; }
        public int VoucherNumber { get; set; }
        public decimal NetTotal { get; set; }
        public decimal FinishedNetTotal { get; set; }
        public System.DateTime Date { get; set; }
        public decimal TotalCharges { get; set; }
        public decimal Difference { get; set; }


    }
   public class InternalStockTransferDetail
    {
        public int Id { get; set; }
        public int VoucherNumber { get; set; }
        public int FromLocationId { get; set; }
        public int FromWarehouseId { get; set; }
        public int ToLocationId { get; set; }
        public int ToWarehouseId { get; set; }
        public string FromLocationName { get; set; }
        public string FromWarehouseName { get; set; }
        public string ToLocationName { get; set; }
        public string ToWarehouseName { get; set; }
        public System.DateTime Date { get; set; }
        public int ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal Quantity { get; set; }


    }
}
