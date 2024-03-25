using AccountEx.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using AccountEx.BussinessLogic;
using AccountEx.Repositories;
using AccountEx.CodeFirst.Models;

namespace AccountEx.Web.Models
{
    public class ItemModel
    {
        public string Name { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Gst { get; set; }
        public decimal GstAmount { get; set; }
        public decimal Amount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal Discount { get; set; }
        public decimal TaxIncAmount { get; set; }
    }
    public class AccountModel
    {
        public string Name { get; set; }
        public string Address { get; set; }
    }
    public class InvoiceModel
    {
        public AccountModel Account { get; set; }
        public int VoucherNumber { get; set; }
        public int InvoiceNumber { get; set; }
        public string Comments { get; set; }
        public DateTime Date { get; set; }
        public decimal Quanity { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Gst { get; set; }
        public decimal Discount { get; set; }
        public decimal NetTotal { get; set; }
        public List<ItemModel> Items { get; set; }
        public InvoiceModel(Sale sale)
        {
            if (sale == null)
                 throw new OwnException("Voucher not found please enter a valid Voucher Number");
            Items = new List<ItemModel>();
            //var sale = list.FirstOrDefault(p => p.TransactionType  == VoucherType.Sale && p.EntryType == (byte)EntryType.MasterDetail);
            var customer = new AccountDetailRepository().GetByAccountId(sale.AccountId);
            // var discEntry = list.FirstOrDefault(p => p.TransactionType  == VoucherType.Sale && p.EntryType == (byte)EntryType.Discount);
            VoucherNumber = sale.VoucherNumber;
            InvoiceNumber = sale.InvoiceNumber;
            Date = sale.Date;
            Comments = sale.Comments;
            //this.SubTotal = sale.GrossTotal;
            //if (discEntry != null)
            //    this.Discount = Numerics.AddDecimal(discEntry.Debit, discEntry.Credit);
            Quanity = sale.QuantityTotal;
            SubTotal = sale.GrossTotal;
            Gst = sale.GST;
            Discount = sale.Discount;
            NetTotal = sale.NetTotal;

            Account = new AccountModel { Name = customer.Name, Address = customer.Address };

            var items = sale.SaleItems;
            foreach (var v in items.Select(item => new ItemModel
            {
                Name = item.ItemName,
                Price = Numerics.GetDecimal(item.Rate),
                Quantity = Numerics.GetInt(item.Quantity),
                Amount = Numerics.GetDecimal(item.Amount),
                Gst = Numerics.GetDecimal(item.GSTPercent),
                GstAmount = Numerics.GetDecimal(item.GSTAmount),
                Discount=Numerics.GetDecimal(item.DiscountAmount),
                NetAmount = Numerics.GetDecimal(item.NetAmount)
            }))
            {
                v.TaxIncAmount = v.Amount + v.GstAmount;
                Items.Add(v);
            }
        }




        public InvoiceModel(List<Transaction> list)
        {
            if (list.Count == 0)
                 throw new OwnException("Voucher not found please enter a valid Voucher Number");
            Items = new List<ItemModel>();
            var sale = list.FirstOrDefault(p => p.TransactionType  == VoucherType.Sale && p.EntryType == (byte)EntryType.MasterDetail);
            var customer = new AccountDetailRepository().GetByAccountId(sale.AccountId);
            var discEntry = list.FirstOrDefault(p => p.TransactionType  == VoucherType.Sale && p.EntryType == (byte)EntryType.Discount);
            VoucherNumber = sale.VoucherNumber;
            InvoiceNumber = sale.InvoiceNumber;
            Date = sale.Date;
            Comments = sale.Comments;
            SubTotal = Numerics.AddDecimal(sale.Debit, sale.Credit);
            if (discEntry != null)
                Discount = Numerics.AddDecimal(discEntry.Debit, discEntry.Credit);
            NetTotal = SubTotal - Discount;
            Account = new AccountModel { Name = sale.AccountTitle, Address = customer != null ? customer.Address : "" };

            var items = list.Where(p => p.TransactionType  == VoucherType.Sale && p.EntryType == (byte)EntryType.Item).ToList();
            foreach (var item in items)
            {
                var v = new ItemModel
                {
                    Name = item.AccountTitle,
                    Price = item.Price,
                    Quantity = item.Quantity,
                    Amount = item.Price * item.Quantity,
                    Gst = (item.Price * item.Quantity * SettingManager.Gst) / 100,
                };
                v.TaxIncAmount = v.Amount + v.Gst;
                Items.Add(v);
            }
        }



    }
}