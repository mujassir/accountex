using AccountEx.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using AccountEx.CodeFirst.Models;

namespace AccountEx.Web.Models
{
    public class KlassInvoice
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
    }
    public class KlassInvoiceModels
    {
        public AccountModel Account { get; set; }
        public int VoucherNumber { get; set; }
        public int InvoiceNumber { get; set; }
        public string Comments { get; set; }
        public DateTime Date { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Discount { get; set; }
        public decimal NetTotal { get; set; }
        public List<ItemModel> Items { get; set; }
        public KlassInvoiceModels(List<Transaction> list, List<Transaction> previousbill)
        {
            if (list.Count == 0)
                 throw new OwnException("Voucher not found please enter a valid Voucher Number");
            Items = new List<ItemModel>();
            var sale = list.FirstOrDefault(p => p.TransactionType  == VoucherType.Sale && p.EntryType == (byte)EntryType.MasterDetail);
            var discEntry = list.FirstOrDefault(p => p.TransactionType  == VoucherType.Sale && p.EntryType == (byte)EntryType.Discount);
            VoucherNumber = sale.VoucherNumber;
            InvoiceNumber = sale.InvoiceNumber;
            Date = sale.Date;
            Comments = sale.Comments;
            SubTotal = Numerics.AddDecimal(sale.Debit, sale.Credit);
            if (discEntry != null)
                Discount = Numerics.AddDecimal(discEntry.Debit, discEntry.Credit);
            NetTotal = SubTotal - Discount;
            Account = new AccountModel { Name = sale.AccountTitle };

            var items = list.Where(p => p.TransactionType  == VoucherType.Sale && p.EntryType == (byte)EntryType.Item).ToList();
            foreach (var item in items)
            {
                Items.Add(new ItemModel
                {
                    Name = item.AccountTitle,
                    Price = item.Price,
                    Quantity = item.Quantity,
                    Amount = item.Price * item.Quantity,
                });
            }
        }
    }
}