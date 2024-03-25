using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Common
{
    public class DataTableExtra
    {
        public int Id { get; set; }
        public int InvoiceNumber { get; set; }
        public int VoucherNumber { get; set; }
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public System.DateTime Date { get; set; }
        public int OrderNumber { get; set; }
    }
    public class DataTableProjectReceiptExtra
    {
        public int InvoiceNumber { get; set; }
        public int VoucherNumber { get; set; }
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public System.DateTime Date { get; set; }
        public decimal Amount { get; set; }

    }
    public class DataTableCustomerGroupPromotionExtra
    {
        public string Group { get; set; }
        public int GroupId { get; set; }
        public string ItemGroup { get; set; }
        public int ItemGroupId { get; set; }
        public int PromotionId { get; set; }
        public string Promotion { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

    }
}
