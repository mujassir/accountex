using System;
namespace AccountEx.CodeFirst.Models
{
    public partial class CurrencyRate : BaseEntity
    {

        public int CurrencyId { get; set; }
        public decimal Rate { get; set; }
        public DateTime FromDate { get; set; }
        public string Remarks { get; set; }
        public int? FiscalId { get; set; }


    }
}