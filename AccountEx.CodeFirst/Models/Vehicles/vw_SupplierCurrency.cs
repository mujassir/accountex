using System;
using System.ComponentModel.DataAnnotations.Schema;
namespace AccountEx.CodeFirst.Models.Vehicles

{

    [Table("vw_SupplierCurrencies")]
    public partial class vw_SupplierCurrency
    {
        public int Id { get; set; }
        public int CurrencyId { get; set; }
        public int AccountId { get; set; }
        public string Currency { get; set; }
        public string Supplier { get; set; }
        public bool IsDeleted { get; set; }
        public int CompanyId { get; set; }

    }
}