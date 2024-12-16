using System.Collections.Generic;
using System;

namespace AccountEx.CodeFirst.Models.Stock
{
    public partial class InternalStockTransfer : BaseEntity
    {
        public InternalStockTransfer()
        {
            this.InternalStockTransferItems = new HashSet<InternalStockTransferItem>();
        }
        public int VoucherNumber { get; set; }
        public string VoucherCode { get; set; }
        public int FiscalId { get; set; }
        public System.DateTime Date { get; set; }
        public int InvoiceNumber { get; set; }
        public Nullable<System.DateTime> DeliveryRequired { get; set; }
        public string Description { get; set; }
        public byte Status { get; set; }
        public string Instructions { get; set; }

        public int FromLocationId { get; set; }
        public int ToLocationId { get; set; }
        public int FromWarehouseId { get; set; }
        public int ToWarehouseId { get; set; }
        public int FromMachineId { get; set; }
        public int ToMachineId { get; set; }
        public int StockTransferType { get; set; } // Location to Warehouse or Location To Machine
        public virtual ICollection<InternalStockTransferItem> InternalStockTransferItems { get; set; }

    }
}
