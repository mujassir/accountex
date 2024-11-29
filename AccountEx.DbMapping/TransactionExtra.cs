using System;
using System.Collections.Generic;
using AccountEx.CodeFirst.Models;
using AccountEx.Common;

namespace AccountEx.DbMapping
{
    public class OpeningBalanceExt
    {
        public int? Id { get; set; }
        public int AccountId { get; set; }
        public VoucherType TransactionType { get; set; }
        public string AccountTitle { get; set; }
        public string AccountCode { get; set; }
        public decimal? Debit { get; set; }
        public decimal? Credit { get; set; }
        public decimal Balance { get; set; }
    }
    public class StockOpeningBalanceExt
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public byte OpeningBalanceTypeId { get; set; }
    }
    public class TransactionExtra : Transaction
    {
        public TransactionExtra() { }
        public TransactionExtra(OpeningBalanceExt p)
        {
            Id = p.Id ?? 0;
            AccountId = p.AccountId;
            TransactionType = p.TransactionType;
            AccountTitle = p.AccountTitle;
            AccountCode = p.AccountCode;
            Debit = p.Debit ?? 0;
            Credit = p.Credit ?? 0;
        }

        public string Item { get; set; }
        public string Marka { get; set; }
        public string AccountCode { get; set; }
        public string Challan { get; set; }


    }
    public class SaleExtra : Transaction
    {
        public List<Transaction> Items { get; set; }
        public SaleExtra()
        {
            var items = new List<Transaction>();
        }
    }
    public class CustomerDiscountExtra
    {
        public List<CustomerDiscount> Discounts { get; set; }

        public CustomerDiscountExtra()
        {
            Discounts = new List<CustomerDiscount>();
        }
    }
    public class ProductMappingExtra
    {
        public List<ProductMapping> ProductMappings { get; set; }

        public ProductMappingExtra()
        {
            ProductMappings = new List<ProductMapping>();
        }
    }
    public class ProductStock
    {
        public List<AccountDetail> Stocks { get; set; }

        public ProductStock()
        {
            Stocks = new List<AccountDetail>();
        }
    }
    public class ProductExtra : CustomerDiscount
    {
        public Nullable<decimal> PurchasePrice { get; set; }
        public Nullable<decimal> SalePrice { get; set; }
    }
    public class ProdMappingExtra : ProductMapping
    {
        public Nullable<decimal> PurchasePrice { get; set; }
        public Nullable<decimal> SalePrice { get; set; }
    }
    public class VoucherExtra
    {
        public List<Transaction> Items { get; set; }
        public VoucherExtra()
        {
            Items = new List<Transaction>();
        }
    }
    public class PurchaseExtra : Purchase
    {
        public List<Purchase> Items { get; set; }
        public List<PurchaseTotalItem> PurchaseTotalItems { get; set; }
        public PurchaseExtra()
        {
            Items = new List<Purchase>();
            PurchaseTotalItems = new List<PurchaseTotalItem>();
        }
    }
    public class StoreTransactionExtra : StoreTransaction
    {
        public List<StoreTransaction> Items { get; set; }
        public StoreTransactionExtra()
        {
            Items = new List<StoreTransaction>();
        }
    }

    public class RecoveryExtra : IReportData
    {

        public int Id { get; set; }
        public int AccountId { get; set; }
        public string AccountTitle { get; set; }
        public decimal PreviousBalance { get; set; }
        public double TodaySale { get; set; }
        public double TodayBalance { get; set; }
        public int Received { get; set; }
        public double NetBalance { get; set; }
    }

    public class TrailBalanceExtra : IReportData
    {

        public int Sr { get; set; }
        public string CustomerName { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
    }
    public class SaleReport : IReportData
    {

        public string Date { get; set; }
        public string VoucherNumber { get; set; }
        public string CustomerName { get; set; }
        public string BillNumber { get; set; }
        public decimal Amount { get; set; }

    }
    public class StockReport
    {

        public int AccountId { get; set; }
        public string Code { get; set; }
        public string Location { get; set; }
        public string ArticleNo { get; set; }
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public string Name { get; set; }
        public string AccountName { get; set; }
        public string AccountCode { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public decimal Initial { get; set; }
        public decimal StockIn { get; set; }
        public decimal StockOut { get; set; }
        public decimal Quantity { get; set; }
        public decimal SalePrice { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal GSTAmount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal Balance { get; set; }
        public string Balance1 { get; set; }
        public decimal StockValue { get; set; }
        public decimal BookValue { get; set; }
        public decimal UnitValue { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal AvgRate { get; set; }
        public decimal AvgSaleRate { get; set; }
        public decimal AvgPurchaseRate { get; set; }
        public string Comments { get; set; }
        public int VoucherNumber { get; set; }
        public byte TransactionType { get; set; }
        public DateTime Date { get; set; }
        public string Date1 { get; set; }
        public int MinimumStock { get; set; }
        public Nullable<int> Size { get; set; }


    }
    public class ItemAvgRates
    {

        public int ItemId { get; set; }
        public decimal Rate { get; set; }



    }
    public class SaleReportByArea
    {
        public int AccountId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Group { get; set; }
        public DateTime Date { get; set; }
        public decimal SaleQty { get; set; }
        public decimal Rate { get; set; }
        public decimal SaleAmount { get; set; }
    }

    public class UsmanBrosCustomer
    {
        public int AccountId { get; set; }
        public string Area { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string GroupName { get; set; }
        public string Ordertaker { get; set; }
        public string Saleman { get; set; }
        public Nullable<int> ParentVendorAccountId { get; set; }
        public string Parent { get; set; }
    }

    public class LessAndSampling
    {

        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public string Group { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public decimal Qty { get; set; }
        public decimal Rate { get; set; }
        public decimal NetAmount { get; set; }
    }

    public class PharmacyStockReport
    {

        public int AccountId { get; set; }
        public string Code { get; set; }
        public string GroupName { get; set; }
        public string Name { get; set; }
        public string AccountName { get; set; }
        public string AccountCode { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public int Initial { get; set; }
        public int StockIn { get; set; }
        public int StockOut { get; set; }
        public int Quantity { get; set; }
        public decimal Rate { get; set; }
        public int Amount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal NetAmount { get; set; }
        public int Balance { get; set; }
        public string Balance1 { get; set; }
        public decimal StockValue { get; set; }
        public decimal BookValue { get; set; }
        public string Comments { get; set; }
        public int VoucherNumber { get; set; }
        public string TransactionType { get; set; }
        public DateTime ExpiryDate { get; set; }
        public DateTime Date { get; set; }
        public string Date1 { get; set; }



    }
    public class DetailedLedgerReport : IReportData
    {

        public string Date { get; set; }
        public string VoucherType { get; set; }
        public string VoucherNumber { get; set; }

        public string Description { get; set; }
        public string CustomerName { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Balance { get; set; }


    }

    public class ProfitLossAccount
    {
        public string SubType { get; set; }
        public int AccountId { get; set; }
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public decimal Amount { get; set; }
    }
    public class ProfitLoss
    {
        public string AccountType { get; set; }
        public string SubType { get; set; }
        public decimal ProfitTotal { get; set; }
        public decimal ExpenseTotal { get; set; }
        public decimal NetProfit { get; set; }
        public List<ProfitLossAccount> Accounts { get; set; }
    }
    public class BalanceSheet
    {
        public string Head { get; set; }
        public string ControlHead { get; set; }
        public string SubHead { get; set; }
        public decimal Amount { get; set; }
    }
    public class NTVehicleDetail
    {
        public string PartyName { get; set; }
        public string DCDate { get; set; }
        public string Description { get; set; }
        public string VehicleNo { get; set; }

        public string DeliveredTo { get; set; }
        public int GatePassNo { get; set; }
        public decimal GSM { get; set; }
        public decimal TotalMeters { get; set; }
        public decimal Kgs { get; set; }

    }
    public class CustomerAging
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string GroupName { get; set; }
        public decimal Current { get; set; }
        public decimal Day7 { get; set; }
        public decimal Day15 { get; set; }
        public decimal Day30 { get; set; }
        public decimal Day60 { get; set; }
        public decimal Day90 { get; set; }
        public decimal Day120 { get; set; }

    }

}
