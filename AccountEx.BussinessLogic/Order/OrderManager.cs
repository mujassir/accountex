using System.Linq;
using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;
using System;
using System.Collections.Generic;
using AccountEx.DbMapping;

namespace AccountEx.BussinessLogic
{
    public static class OrderManager
    {


        public static void Save(Voucher voucher)
        {
            var repo = new VoucherTransRepository();
            if (voucher.Id == 0)
            {
                voucher.VoucherNumber = new TransactionRepository().GetNextVoucherNumber(voucher.TransactionType);
                repo.Add(voucher);
                repo.SaveLog(voucher, ActionType.Added);
            }
            else
            {
                repo.Update(voucher);
                repo.SaveLog(voucher, ActionType.Updated);
            }
            if (voucher.TransactionType  == VoucherType.TransferVoucher)
                AddJvTransaction(voucher);
            else
                AddTransaction(voucher);

        }
        public static void AddTransaction(Voucher v)
        {
            var dt = DateTime.Now;
            new TransactionRepository().HardDelete(v.VoucherNumber, v.TransactionType, v.AuthLocationId);
            var trans = v.VoucherItems.Select(item => new Transaction
            {
                AccountId = item.AccountId,
                InvoiceNumber = v.InvoiceNumber,
                VoucherNumber = v.VoucherNumber,
                TransactionType = v.TransactionType,
                EntryType = (byte)EntryType.Item,
                Credit = v.TransactionType  == VoucherType.CashReceipts || v.TransactionType  == VoucherType.BankReceipts
                        ? Numerics.GetInt(item.Amount)
                        : 0,
                Debit =
                    v.TransactionType  == VoucherType.CashPayments || v.TransactionType  == VoucherType.BankPayments
                        ? Numerics.GetInt(item.Amount)
                        : 0
            }).ToList();

            trans.AddRange(

                  v.VoucherItems.Select(item => new Transaction
              {
                  AccountId = Numerics.GetInt(v.AccountId),
                  InvoiceNumber = v.InvoiceNumber,
                  VoucherNumber = v.VoucherNumber,
                  TransactionType = v.TransactionType,
                  EntryType = (byte)EntryType.MasterDetail,
                  Debit = v.TransactionType  == VoucherType.CashReceipts || v.TransactionType  == VoucherType.BankReceipts
                          ? Numerics.GetInt(item.Amount)
                          : 0,
                  Credit =
                      v.TransactionType  == VoucherType.CashPayments || v.TransactionType  == VoucherType.BankPayments
                          ? Numerics.GetInt(item.Amount)
                          : 0
              }).ToList()

                  );
            foreach (var item in trans)
            {
                item.VoucherNumber = v.VoucherNumber;
                item.CreatedDate = v.CreatedDate ?? dt; ;
                item.Date = v.Date!=DateTime.MinValue ? v.Date : dt;
                item.Comments = v.Comments;
            }

            new TransactionRepository().Add(trans);
        }
        public static void AddJvTransaction(Voucher v)
        {
            var dt = DateTime.Now;
            new TransactionRepository().HardDelete(v.VoucherNumber, v.TransactionType, v.AuthLocationId);
            var trans = v.VoucherItems.Select(item => new Transaction
            {
                AccountId = item.AccountId,
                InvoiceNumber = v.InvoiceNumber,
                VoucherNumber = v.VoucherNumber,
                TransactionType = v.TransactionType,
                EntryType = (byte)EntryType.Item,
                Credit = Numerics.GetDecimal(item.Credit),
                Debit = Numerics.GetDecimal(item.Debit)
            }).ToList();
            foreach (var item in trans)
            {
                item.VoucherNumber = v.VoucherNumber;
                item.CreatedDate = dt;
                item.Date = v.Date!=DateTime.MinValue ? v.Date : dt;
                item.Comments = v.Comments;
            }
            new TransactionRepository().Add(trans);
        }

        public static void Delete(int voucherno, VoucherType transactiontype)
        {

            new TransactionRepository().HardDelete(voucherno, transactiontype);
            new VoucherTransRepository().DeleteByVoucherNumber(voucherno, transactiontype);

        }

        public static Voucher GetVocuherDetail(int voucherno, VoucherType transactiontype, string key)
        {
            var d = new Voucher();
            //if (key == "next")
            //{
            //    voucherno = voucherno + 1;
            //}

            //else if (key == "previous")
            //{
            //    voucherno = voucherno - 1;
            //}
            //else if (key == "first")
            //{
            //    voucherno = 1;
            //}
            //else if (key == "last")
            //{
            //    voucherno = 1;
            //}
            //d = new VoucherTransRepository().GetByVoucherNumber(voucherno, transactiontype, key) ??
            //    new Voucher() { VoucherNumber = voucherno };
            return d;
        }

        public static List<InvoiceExtra> GetOrderListing()
        {
            //var order = repo.Where(p => p.Status == (byte)TransactionStatus.Pending || p.Status == (byte)TransactionStatus.Pending);


            var orderrepo = new OrderBookingRepository().AsQueryable();
            var dcrepo = new DeliveryChallanRepository().AsQueryable();
            var salerepo = new SaleRepository().AsQueryable();
            var orders = orderrepo.Where(p => p.Status == (byte)TransactionStatus.Pending);
            var dcs = dcrepo.Where(p => p.Status == (byte)TransactionStatus.Pending);
            var odlist = new List<InvoiceExtra>();
            foreach (var item in orders)
            {
                var od = new InvoiceExtra()
                {
                    VoucherNumber = item.VoucherNumber,
                    OrderDate = item.Date.ToString(AppSetting.DateFormat),
                    DCNo = 0,
                    OrderNo = item.VoucherNumber,
                    DcDate = "",
                    SaleNo = 0,
                    SaleDate = "",
                    GpNo = "",
                    Customer = item.AccountCode + "-" + item.AccountName,
                    NetTotal = item.NetTotal,
                    OrderStatus = item.Status == (byte)TransactionStatus.Pending ? "Pending" : "Partial Delivered",
                    Type = "Order"
                };
                odlist.Add(od);
            }
            foreach (var item in dcs)
            {
                var od = new InvoiceExtra()
                {
                    OrderNo = item.OrderNo,
                    //OrderDate = item.OrderDate.ToString(AppSetting.DateFormat),
                    DCNo = item.VoucherNumber,
                    DcDate = item.Date.ToString(AppSetting.DateFormat),
                    SaleNo = 0,
                    SaleDate = "",
                    GpNo = "",
                    //Customer = item.AccountCode + "-" + item.AccountName,
                    NetTotal = 0,
                    OrderStatus = "Delivered",
                    Type = "DC"


                };
                odlist.Add(od);
            }
            var salecount = 100 - odlist.Count();
            var sales = salerepo.Take(salecount);
            var dcnos = sales.Select(p => p.DCNo).ToList();
            var dcnoss = sales.Select(p => p.DCNo).ToList();
            var dclookup = dcrepo.Where(p => dcnos.Contains(p.VoucherNumber));
            var ordernos = dclookup.Select(p => p.OrderNo).ToList();
            var orderlookup = orderrepo.Where(p => ordernos.Contains(p.VoucherNumber));
            foreach (var item in sales)
            {
                var order = new Order();
                var dc = dclookup.FirstOrDefault(p => p.VoucherNumber == item.DCNo);
                if (dc != null)
                    order = orderlookup.FirstOrDefault(p => p.VoucherNumber == dc.OrderNo);
                else
                    dc = new DeliveryChallan();
                if (order == null) continue;
                var od = new InvoiceExtra()
                {
                    OrderNo = order.VoucherNumber,
                    OrderDate = order.Date.ToString(AppSetting.DateFormat),
                    DCNo = item.DCNo,
                    DcDate = dc.Date.ToString(AppSetting.DateFormat),
                    SaleNo = item.VoucherNumber,
                    SaleDate = item.Date.ToString(AppSetting.DateFormat),
                    GpNo = dc.GatePassNo.ToString(),
                    Customer = item.AccountCode + "-" + item.AccountName,
                    NetTotal = item.NetTotal,
                    OrderStatus = "Invoiced",
                    Type = "Sale"


                };
                odlist.Add(od);
            }
            odlist = odlist.OrderByDescending(p => p.Type == "Order").ThenByDescending(p => p.Type == "DC").ToList();
            return odlist;



        }

       

    }
}
