using System;
using AccountEx.CodeFirst.Models;
using System.Collections.Generic;
using AccountEx.Common;

namespace AccountEx.DbMapping
{
    public class GeneralLedgerEntry
    {
        public string Date { get; set; }
        public int AccountId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string VoucherType { get; set; }
        public VoucherType TransactionType { get; set; }
        public string VoucherNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public string Description { get; set; }
        public string Debit { get; set; }
        public string Credit { get; set; }
        public string Balance { get; set; }
        public GeneralLedgerEntry(Transaction tr)
        {
            Date = tr.Date.ToString(AppSetting.DateFormat);
            VoucherType = Enum.GetName(typeof(VoucherType), tr.TransactionType);
            TransactionType = tr.TransactionType;
            VoucherNumber = tr.VoucherNumber + "";
            InvoiceNumber = tr.InvoiceNumber + "";
            AccountId = tr.AccountId;
            Description = tr.Comments + "";
            Debit = tr.Debit > 0 ? Numerics.DecimalToString(tr.Debit) : "";
            Credit = tr.Credit > 0 ? Numerics.DecimalToString(tr.Credit) : "";
            Balance = Numerics.DecimalToString(tr.Debit, tr.Credit);
        }
        public GeneralLedgerEntry(ActivityEntry tr)
        {
            TransactionType = tr.TransactionType;
            Date = tr.Date.ToString(AppSetting.DateFormat);
            VoucherType = tr.VoucherType;
            VoucherNumber = tr.VoucherNumber + "";

            Description = tr.Description + "";
            Debit = tr.Debit > 0 ? Numerics.DecimalToString(tr.Debit, 0) : "";
            Credit = tr.Credit > 0 ? Numerics.DecimalToString(tr.Credit, 0) : "";
            Balance = Numerics.DecimalToString(tr.Debit, tr.Credit);
            Code = tr.Code;
            Name = tr.Name;
        }
    }

    public class GeneralLedgerMultiAccount
    {

        public string Account { get; set; }
        public decimal OpeningBalances { get; set; }
        public List<GeneralLedgerEntry> Records { get; set; }
        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
        public decimal TotalBalance { get; set; }

    }

    public class ProductLedgerEntry
    {
        public byte TransactionType { get; set; }
        public string Date { get; set; }
        public string VoucherType { get; set; }
        public string VoucherNumber { get; set; }
        public string Description { get; set; }
        public string StockIn { get; set; }
        public string StockOut { get; set; }
        public string Balance { get; set; }
        public ProductLedgerEntry(StockReport tr)
        {

            Date = tr.Date.ToString(AppSetting.DateFormat);
            TransactionType = tr.TransactionType;
            //VoucherType = tr.TransactionType;
            VoucherNumber = tr.VoucherNumber + "";
            Description = tr.Comments + "";
            StockIn = tr.StockIn > 0 ? Numerics.DecimalToString(tr.StockIn) : "";
            StockOut = tr.StockOut > 0 ? Numerics.DecimalToString(tr.StockOut) : "";
            Balance = Numerics.DecimalToString(tr.StockIn, tr.StockOut);
        }
    }

    public class ActivityEntry
    {
        public VoucherType TransactionType { get; set; }
        public DateTime Date { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string VoucherType { get; set; }
        public int VoucherNumber { get; set; }
        public int InvoiceNumber { get; set; }
        public string Description { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
    }
}
