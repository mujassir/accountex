using AccountEx.Common;
using System;

namespace AccountEx.CodeFirst.Models.Stock
{
    public partial class InternalStockTransferItem : BaseEntity
    {

        public int InternalStockTransferId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int ItemId { get; set; }
        public int VoucherNumber { get; set; }
        public int InvoiceNumber { get; set; }
        public decimal Quantity { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
        public string ArticleNo { get; set; }

        public string Unit { get; set; }
        public byte Status { get; set; }
        public decimal QuantityDelivered { get; set; }
        public Decimal? StockBalance { get; set; }
        public int Type { get; set; }
    }
}
