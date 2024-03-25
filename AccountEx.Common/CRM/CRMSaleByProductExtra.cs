using System;
namespace AccountEx.Common.CRM
{
    public class CRMSaleByProductExtra
    {
        public int? ProductId { get; set; }
        public string Product { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public string SalePerson { get; set; }
        public int? SalePersonId { get; set; }

        public string Customer { get; set; }
        public string Remarks { get; set; }
        public int? CustomerId { get; set; }
        public decimal? Amount { get; set; }
        public decimal? Quantity { get; set; }
        public byte? Type { get; set; }
        public int? DivisionId { get; set; }
    }
    public class CRMSaleByProductIdExtra
    {
        public int? ProdcutId { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public int? SalePersonId { get; set; }
        public decimal? Amount { get; set; }

    }
}
