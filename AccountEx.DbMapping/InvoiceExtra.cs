using AccountEx.CodeFirst.Models;

namespace AccountEx.DbMapping
{

    public class InvoiceExtra : Sale
    {
        public int OrderNo { get; set; }
        //public int DCNo { get; set; }
        public int SaleNo { get; set; }
        public string OrderDate { get; set; }
        public string DcDate { get; set; }
        public string SaleDate { get; set; }
        public string OrderStatus { get; set; }
        public string GpNo { get; set; }
        public string Customer { get; set; }
        public string Type { get; set; }
    }
}