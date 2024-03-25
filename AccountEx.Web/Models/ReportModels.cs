using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.DbMapping;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AccountEx.Web.Models
{

    public class GeneralLedgerModel
    {
        public GeneralLedgerModel()
        {
            var dt = new DateTime(DateTime.Now.Year, 7, 1);
            if (dt > DateTime.Now)
                dt = dt.AddYears(-1);
            FromDate = SiteContext.Current.Fiscal.FromDate;
            ToDate =SiteContext.Current.Fiscal.ToDate;
        }
        public decimal OpeningBalance { get; set; }
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FromDate { get; set; }
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ToDate { get; set; }
        public int AccountId { get; set; }
        public ICollection<IdName> Accounts { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
    }
    public class DetailedLedgerModel
    {
        public DetailedLedgerModel()
        {
         FromDate = ToDate = DateTime.Now;
        }
        public decimal OpeningBalance { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime FromDate { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime ToDate { get; set; }
        public int AccountId { get; set; }
        public ICollection<IdName > Accounts { get; set; }
        public ICollection<TransactionExtra> Transactions { get; set; }
    }
    public class LedgerItemModel
    {
        public LedgerItemModel()
        {

        }
        public DateTime Date { get; set; }
        public string Item { get; set; }
        public string Marka { get; set; }
        public string Challan { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
    }
    public class SaleLedgerModel
    {
        public SaleLedgerModel()
        {
            FromDate = ToDate = DateTime.Now;
        }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime FromDate { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime ToDate { get; set; }
        public ICollection<Sale> Sales { get; set; }
    }


}