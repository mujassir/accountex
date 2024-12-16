using AccountEx.Common;
using System;
using System.Collections.Generic;

namespace AccountEx.CodeFirst.Models.Production
{
    public class ProductionUnit : BaseEntity
    {
        public DateTime Date { get; set; }
        public int VoucherNumber { get; set; }
        public int FiscalId { get; set; }
        public int LocationId { get; set; }
        public int MachineId { get; set; }
        public int StockWarehouseId { get; set; }
        public int RawStockWarehouseId { get; set; }
        public int FinishStockWarehouseId { get; set; }
        public decimal Quantity { get; set; }
        public int ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Comments { get; set; }
        public ProductionStatus Status { get; set; }
        public virtual ICollection<ProductionUnitItem> Items { get; set; }
    }
}
