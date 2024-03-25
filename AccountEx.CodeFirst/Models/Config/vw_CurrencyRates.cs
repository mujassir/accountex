using System;
using System.ComponentModel.DataAnnotations.Schema;
namespace AccountEx.CodeFirst.Models
{

    [Table("vw_CurrencyRates")]
    public partial class vw_CurrencyRate
    {
        public int Id { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public decimal Rate { get; set; }
        public DateTime FromDate { get; set; }
        public string Remarks { get; set; }
        public bool IsDeleted { get; set; }
        public int CompanyId { get; set; }
        public int? FiscalId { get; set; }

    }
}