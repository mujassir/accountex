using AccountEx.CodeFirst.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity.Core.Objects;
using AccountEx.Common;
using AccountEx.DbMapping;

namespace AccountEx.Repositories
{
    public class SaleRepository : GenericRepository<Sale>
    {
        public SaleRepository() : base() { }
        public SaleRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }
        public List<LatestSaleRate> GetLatestSaleRates()
        {
            var query = "";
            query = string.Format("EXEC [dbo].[GetSJFMLatestSaleRates] @CompanyId={0}", SiteContext.Current.User.CompanyId);
            var data = Db.Database.SqlQuery<LatestSaleRate>(query).ToList();
            return data;
        }
        public int GetNextVoucherNumber(VoucherType vouchertype)
        {
            var maxnumber = 1001;
            if (!FiscalCollection.Any(p => p.TransactionType == vouchertype))
                return maxnumber;
            return FiscalCollection.Where(p => p.TransactionType == vouchertype).OrderByDescending(p => p.VoucherNumber).FirstOrDefault().VoucherNumber + 1;
        }
        public int GetLastInvoiceNumber(VoucherType vouchertype)
        {

            var maxnumber = ConfigurationReader.GetConfigKeyValue<int>("InvoiceStartNumber", 1);
            if (!FiscalCollection.Any(p => p.TransactionType == vouchertype))
                return maxnumber;
            return FiscalCollection.Where(p => p.TransactionType == vouchertype).OrderByDescending(p => p.Id).FirstOrDefault().InvoiceNumber + 1;
        }
        public int GetNextBookNumber(VoucherType vouchertype)
        {
            var maxnumber = 1001;
            if (!FiscalCollection.Any(p => p.TransactionType == vouchertype))
                return maxnumber;
            return FiscalCollection.Where(p => p.TransactionType == vouchertype).OrderByDescending(p => p.InvoiceNumber).FirstOrDefault().InvoiceNumber + 1;
        }
        public int GetId(int voucherno, VoucherType vouchertype)
        {

            if (!FiscalCollection.Any(p => p.VoucherNumber == voucherno && p.TransactionType == vouchertype))
                return 0;
            return FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno && p.TransactionType == vouchertype).Id;
        }
        public int GetNextManualVoucherNumber(VoucherType vouchertype)
        {
            var maxnumber = 1001;
            if (!FiscalCollection.Any(p => p.TransactionType == vouchertype))
                return maxnumber;
            return FiscalCollection.Where(p => p.TransactionType == vouchertype).OrderByDescending(p => p.ManualVoucherNumber).FirstOrDefault().ManualVoucherNumber + 1;
        }
        public List<Sale> GetSales(DateTime fromdate, DateTime todate, int accountId, VoucherType voutyp, int areaId, int groupId = 0, int salesmanId = 0)
        {

            var records = new List<Sale>();
            var groupProductIds = new List<int>();
            var query = FiscalCollection.Where(p => EntityFunctions.TruncateTime(p.Date) >= EntityFunctions.TruncateTime(fromdate) && EntityFunctions.TruncateTime(p.Date) <= EntityFunctions.TruncateTime(todate) && p.TransactionType == voutyp);

            if (accountId > 0)
                query = query.Where(p => p.AccountId == accountId);


            if (areaId > 0)
            {
                var cusromerIds = new AccountRepository().GetLeafAccount(areaId).Select(p => p.Id).ToList();
                query.Where(p => cusromerIds.Contains(p.AccountId));
            }
            if (salesmanId > 0)
            {
                query = query.Where(p => p.SalemanId == salesmanId || p.SalesmanId == salesmanId);
            }
            if (groupId > 0)
            {
                groupProductIds = AsQueryable<AccountDetail>().Where(p => p.AccountDetailFormId == (byte)AccountDetailFormType.Products && p.GroupId == groupId).Select(p => p.AccountId).ToList();
                query = query.Where(p => p.SaleItems.Any(q => groupProductIds.Contains(q.ItemId)));

            }
            records = query.ToList();
            if (groupId > 0)
            {
                foreach (var sale in records)
                {
                    sale.SaleItems = sale.SaleItems.Where(p => groupProductIds.Contains(p.ItemId)).ToList();
                    sale.GrossTotal = sale.SaleItems.Sum(p => p.Amount);
                    sale.Discount = sale.SaleItems.Sum(p => p.DiscountAmount);
                    sale.GstAmountTotal = sale.SaleItems.Sum(p => p.GSTAmount);
                    sale.NetTotal = sale.SaleItems.Sum(p => p.NetAmount);

                }
            }

            return records;
        }
        public Sale GetByVoucherNumber(int voucherno, VoucherType vtype)
        {
            return FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno && p.TransactionType == vtype);
        }
        public List<Sale> GetByVoucherNumber(VoucherType vtype, int voucherno)
        {
            return FiscalCollection.Where(p => p.VoucherNumber == voucherno && p.TransactionType == vtype).ToList();
        }
        public Sale GetByVoucherNumber(int voucherno, VoucherType type, int id)
        {
            return FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno && p.TransactionType == type && p.Id != id);
        }
        public bool IsVoucherExits(int voucherno, VoucherType type, int id)
        {
            return FiscalCollection.Any(p => p.VoucherNumber == voucherno && p.TransactionType == type && p.Id != id);
        }
        public Sale GetByBookNumber(int bookNo, VoucherType type, int id)
        {
            return FiscalCollection.FirstOrDefault(p => p.InvoiceNumber == bookNo && p.TransactionType == type && p.Id != id);
        }
        public bool IsBookNoExits(int bookNo, VoucherType type, int id)
        {
            return FiscalCollection.Any(p => p.InvoiceNumber == bookNo && p.TransactionType == type && p.Id != id);
        }
        public List<Sale> GetByDCNo(VoucherType type, int DCNo)
        {
            return FiscalCollection.Where(p => p.DCNo == DCNo && p.TransactionType == type).ToList();
        }
        public List<Sale> GetByDCNo(List<VoucherType> voucherTypes, int DCNo)
        {
            return FiscalCollection.Where(p => p.DCNo == DCNo && voucherTypes.Contains(p.TransactionType)).ToList();
        }
        public bool CheckIfSaleExistByDCNo(List<VoucherType> voucherTypes, int DCNo)
        {
            return FiscalCollection.Any(p => p.DCNo == DCNo && voucherTypes.Contains(p.TransactionType));
        }
        public bool CheckIfSaleExistByDCId(int id)
        {
            return FiscalCollection.Any(p => p.Id == id);
        }

        public Sale GetByVoucherNumber(int voucherno, VoucherType vtype, string key, out bool next, out bool previous)
        {
            Sale v = null;
            switch (key)
            {
                case "first":
                    v = FiscalCollection.Where(p => p.TransactionType == vtype).OrderBy(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "last":
                    v = FiscalCollection.Where(p => p.TransactionType == vtype).OrderByDescending(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "next":
                    v = FiscalCollection.Where(p => p.TransactionType == vtype && p.VoucherNumber > voucherno).OrderBy(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "previous":
                    v = FiscalCollection.Where(p => p.TransactionType == vtype && p.VoucherNumber < voucherno).OrderByDescending(p => p.VoucherNumber).FirstOrDefault(); ;
                    break;
                case "same":
                    v = FiscalCollection.FirstOrDefault(p => p.TransactionType == vtype && p.VoucherNumber == voucherno);
                    break;
                case "challan":
                    v = FiscalCollection.FirstOrDefault(p => p.TransactionType == vtype && p.VoucherNumber == voucherno);
                    break;

            }

            if (v != null)
                voucherno = v.VoucherNumber;
            else if (key != "nextvouchernumber" && key != "challan")
            {
                v = FiscalCollection.Where(p => p.TransactionType == vtype).OrderByDescending(p => p.VoucherNumber).FirstOrDefault();

            }
            if (v == null && !FiscalCollection.Any(p => p.TransactionType == vtype))
            {
                v = new Sale();
                v.VoucherNumber = ConfigurationReader.GetConfigKeyValue<int>("VoucherStartNumber", 1001);
                v.InvoiceNumber = ConfigurationReader.GetConfigKeyValue<int>("InvoiceStartNumber", 1001);
                v.Date = DateTime.Now;
                v.CreatedDate = DateTime.Now;
            }
            next = FiscalCollection.Any(p => p.TransactionType == vtype && p.VoucherNumber > voucherno);
            previous = FiscalCollection.Any(p => p.TransactionType == vtype && p.VoucherNumber < voucherno);
            return v;
        }

        //overload for service order
        public Sale GetByVoucherNumber(int voucherno, List<VoucherType> vtype, string key, out bool next, out bool previous)
        {
            Sale v = null;
            switch (key)
            {
                case "first":
                    v = FiscalCollection.Where(p => vtype.Contains(p.TransactionType)).OrderBy(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "last":
                    v = FiscalCollection.Where(p => vtype.Contains(p.TransactionType)).OrderByDescending(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "next":
                    v = FiscalCollection.Where(p => vtype.Contains(p.TransactionType) && p.VoucherNumber > voucherno).OrderBy(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "previous":
                    v = FiscalCollection.Where(p => vtype.Contains(p.TransactionType) && p.VoucherNumber < voucherno).OrderByDescending(p => p.VoucherNumber).FirstOrDefault(); ;
                    break;
                case "same":
                    v = FiscalCollection.FirstOrDefault(p => vtype.Contains(p.TransactionType) && p.VoucherNumber == voucherno);
                    break;
                case "challan":
                    v = FiscalCollection.FirstOrDefault(p => vtype.Contains(p.TransactionType) && p.VoucherNumber == voucherno);
                    break;

            }

            if (v != null)
                voucherno = v.VoucherNumber;
            else if (key != "nextvouchernumber" && key != "challan")
            {
                v = FiscalCollection.Where(p => vtype.Contains(p.TransactionType)).OrderByDescending(p => p.VoucherNumber).FirstOrDefault();

            }
            if (v == null && !FiscalCollection.Any(p => vtype.Contains(p.TransactionType)))
            {
                v = new Sale();
                v.VoucherNumber = 1001;
                v.InvoiceNumber = 1;
                v.Date = DateTime.Now;
            }
            next = FiscalCollection.Any(p => vtype.Contains(p.TransactionType) && p.VoucherNumber > voucherno);
            previous = FiscalCollection.Any(p => vtype.Contains(p.TransactionType) && p.VoucherNumber < voucherno);
            return v;
        }
        public void DeleteByVoucherNumber(int voucherno, VoucherType trtype)
        {
            var sale = FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno && p.TransactionType == trtype);
            Delete(sale);
        }
        public void DeleteByVoucherNumber(int voucherno, List<VoucherType> transactionTypes)
        {
            var sale = FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno && transactionTypes.Contains(p.TransactionType));
            Delete(sale);
        }


        public override void Delete(int id)
        {
            var sale = FiscalCollection.FirstOrDefault(p => p.Id == id);
            Delete(sale);
        }
        public void Delete(Sale sale)
        {

            if (sale == null) return;
            if (sale.InvoiceDcs.Count() > 0)
            {
                var dcIds = sale.InvoiceDcs.Select(p => p.DcId).ToList();
                var dbDcs = AsQueryable<DeliveryChallan>(true).Where(p => dcIds.Contains(p.Id)).ToList();
                foreach (var invoiceDc in sale.InvoiceDcs)
                {



                    var transactionType = sale.TransactionType == VoucherType.Sale || sale.TransactionType == VoucherType.GstSale ? VoucherType.GoodIssue : VoucherType.GoodReceive;
                    var dc = dbDcs.FirstOrDefault(p => p.Id == invoiceDc.DcId);
                    if(dc != null)
                    {
                        foreach (var item in dc.DCItems)
                        {

                            item.QtyDelivered = 0;
                            if (item.QtyDelivered >= item.Quantity)
                                item.Status = (byte)AccountEx.Common.TransactionStatus.Delivered;
                            else if (item.QtyDelivered > 0)
                                item.Status = (byte)AccountEx.Common.TransactionStatus.PartialyDelivered;
                            else
                                item.Status = (byte)AccountEx.Common.TransactionStatus.Pending;
                        }
                        if (dc.DCItems.Any(p => p.Status == (byte)AccountEx.Common.TransactionStatus.PartialyDelivered))
                            dc.Status = (byte)AccountEx.Common.TransactionStatus.PartialyDelivered;
                        else if (!dc.DCItems.Any(p => p.Status == (byte)AccountEx.Common.TransactionStatus.PartialyDelivered) && !dc.DCItems.Any(p => p.Status == (byte)AccountEx.Common.TransactionStatus.Pending))
                            dc.Status = (byte)AccountEx.Common.TransactionStatus.Delivered;
                        else
                            dc.Status = (byte)AccountEx.Common.TransactionStatus.Pending;
                    }

                }

            }

            base.Delete(sale.Id);
        }
        public List<Sale> GetAgingSale2(int id, int count)
        {
            return FiscalCollection.Where(p => p.Id < id).Take(count).ToList();
        }
        public List<Sale> GetAgingSale(int id, int accountId, DateTime date, int count)
        {
            return FiscalCollection.Where(p => p.AccountId == accountId && p.Date <= date && p.Id != id).OrderByDescending(p => p.Date).Take(count).ToList();
        }
        public decimal GetSumByVoucherType(DateTime fromdate, DateTime todate, VoucherType voutype)
        {
            var data = FiscalCollection.Where(p => EntityFunctions.TruncateTime(p.Date) >= EntityFunctions.TruncateTime(fromdate) && EntityFunctions.TruncateTime(p.Date) <= EntityFunctions.TruncateTime(todate) && p.TransactionType == voutype);
            return data.Sum(p => (decimal?)p.NetTotal) ?? 0.0m;

        }
        public Sale GetByVoucherNo(int voucherno, int id, VoucherType transtype)
        {
            return FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno && p.TransactionType == transtype && p.Id != id);
        }
        public Sale GetByInvoiceNo(int invoiceno, int id, VoucherType transtype)
        {
            return FiscalCollection.FirstOrDefault(p => p.InvoiceNumber == invoiceno && p.TransactionType == transtype && p.Id != id);
        }
        public override void Save(Sale sale)
        {
            var repo = new SaleRepository();
            if (sale.Id == 0)
            {
                repo.Add(sale);
            }
            else
            {
                repo.Update(sale);
            }
        }
        //public override void Add(Sale sale)
        //{
        //    if (sale.DCNo > 0)
        //    {
        //        var transactionType = sale.TransactionType  == VoucherType.Sale || sale.TransactionType  == VoucherType.GstSale ? VoucherType.GoodIssue : VoucherType.GoodReceive;
        //        var dc = AsQueryable<DeliveryChallan>(true).FirstOrDefault(p => p.VoucherNumber == sale.DCNo && p.TransactionType == transactionType);
        //        foreach (var item in dc.DCItems)
        //        {
        //            var saleItem = sale.SaleItems.FirstOrDefault(p => p.ItemId == item.ItemId);
        //            if (saleItem != null)
        //            {
        //                item.QtyDelivered += saleItem.Quantity;
        //                if (item.QtyDelivered >= item.Quantity)
        //                    item.Status = (byte)AccountEx.Common.TransactionStatus.Delivered;
        //                else if (item.QtyDelivered > 0)
        //                    item.Status = (byte)AccountEx.Common.TransactionStatus.PartialyDelivered;
        //                else
        //                    item.Status = (byte)AccountEx.Common.TransactionStatus.Pending;
        //            }
        //        }
        //        if (dc.DCItems.Any(p => p.Status == (byte)AccountEx.Common.TransactionStatus.PartialyDelivered))
        //            dc.Status = (byte)AccountEx.Common.TransactionStatus.PartialyDelivered;
        //        else if (!dc.DCItems.Any(p => p.Status == (byte)AccountEx.Common.TransactionStatus.PartialyDelivered) && !dc.DCItems.Any(p => p.Status == (byte)AccountEx.Common.TransactionStatus.Pending))
        //            dc.Status = (byte)AccountEx.Common.TransactionStatus.Delivered;
        //        else
        //            dc.Status = (byte)AccountEx.Common.TransactionStatus.Pending;

        //    }
        //    base.Add(sale);
        //    SaveChanges();
        //}

        public override void Add(Sale sale)
        {
            if (sale.InvoiceDcs.Count() > 0)
            {
                var dcIds = sale.InvoiceDcs.Select(p => p.DcId).ToList();
                var dbDcs = AsQueryable<DeliveryChallan>(true).Where(p => dcIds.Contains(p.Id)).ToList();
                foreach (var invoiceDc in sale.InvoiceDcs)
                {



                    var transactionType = sale.TransactionType == VoucherType.Sale || sale.TransactionType == VoucherType.GstSale ? VoucherType.GoodIssue : VoucherType.GoodReceive;
                    var dc = dbDcs.FirstOrDefault(p => p.Id == invoiceDc.DcId);
                    foreach (var item in dc.DCItems)
                    {

                        item.QtyDelivered = item.Quantity;
                        if (item.QtyDelivered >= item.Quantity)
                            item.Status = (byte)AccountEx.Common.TransactionStatus.Delivered;
                        else if (item.QtyDelivered > 0)
                            item.Status = (byte)AccountEx.Common.TransactionStatus.PartialyDelivered;
                        else
                            item.Status = (byte)AccountEx.Common.TransactionStatus.Pending;
                    }

                    if (dc.DCItems.Any(p => p.Status == (byte)AccountEx.Common.TransactionStatus.PartialyDelivered))
                        dc.Status = (byte)AccountEx.Common.TransactionStatus.PartialyDelivered;
                    else if (!dc.DCItems.Any(p => p.Status == (byte)AccountEx.Common.TransactionStatus.PartialyDelivered) && !dc.DCItems.Any(p => p.Status == (byte)AccountEx.Common.TransactionStatus.Pending))
                        dc.Status = (byte)AccountEx.Common.TransactionStatus.Delivered;
                    else
                        dc.Status = (byte)AccountEx.Common.TransactionStatus.Pending;

                }
            }
            base.Add(sale, true, false);
        }
        public override void Update(Sale sale)
        {
            var saleItemsRepo = new SaleItemRepository(this);
            var dbSale = GetById(sale.Id, true);
            //if (sale.DCNo > 0)
            //{
            //    var transactionType = sale.TransactionType  == VoucherType.Sale || sale.TransactionType  == VoucherType.GstSale ? VoucherType.GoodIssue : VoucherType.GoodReceive;
            //    var dc = AsQueryable<DeliveryChallan>(true).FirstOrDefault(p => p.VoucherNumber == sale.DCNo && p.TransactionType == transactionType);
            //    foreach (var item in dbSale.SaleItems)
            //    {
            //        var DCItem = dc.DCItems.FirstOrDefault(p => p.ItemId == item.ItemId);
            //        if (DCItem == null)
            //            continue;
            //        var saleItem = sale.SaleItems.FirstOrDefault(p => p.ItemId == item.ItemId);
            //        if (saleItem == null)
            //            DCItem.QtyDelivered -= item.Quantity;
            //        else
            //            DCItem.QtyDelivered += saleItem.Quantity - item.Quantity;

            //        if (DCItem.QtyDelivered >= DCItem.Quantity)
            //            DCItem.Status = (byte)AccountEx.Common.TransactionStatus.Delivered;
            //        else if (DCItem.QtyDelivered > 0)
            //            DCItem.Status = (byte)AccountEx.Common.TransactionStatus.PartialyDelivered;
            //        else
            //            DCItem.Status = (byte)AccountEx.Common.TransactionStatus.Pending;
            //    }
            //    if (dc.DCItems.Any(p => p.Status == (byte)AccountEx.Common.TransactionStatus.PartialyDelivered))
            //        dc.Status = (byte)AccountEx.Common.TransactionStatus.PartialyDelivered;
            //    else if (!dc.DCItems.Any(p => p.Status == (byte)AccountEx.Common.TransactionStatus.PartialyDelivered) && !dc.DCItems.Any(p => p.Status == (byte)AccountEx.Common.TransactionStatus.Pending))
            //        dc.Status = (byte)AccountEx.Common.TransactionStatus.Delivered;    // This Line of Code is changed Because Order status should be delived if all of its items status is delivered
            //    //order.Status = (byte)AccountEx.Common.TransactionStatus.PartialyDelivered;
            //    else
            //        dc.Status = (byte)AccountEx.Common.TransactionStatus.Pending;
            //    dc.Status = dc.Status;
            //}
            //add,update & remove services items
            var Ids = sale.SaleItems.Select(p => p.Id).ToList();
            var deletedIds = dbSale.SaleItems.Where(p => !Ids.Contains(p.Id)).Select(p => p.Id).ToList();
            saleItemsRepo.Delete(deletedIds);
            saleItemsRepo.Save(sale.SaleItems.ToList());

            //var query = "Delete from SaleItems where SaleId=" + sale.Id + " and CompanyId=" + SiteContext.Current.User.CompanyId;
            //Db.Database.ExecuteSqlCommand(query);
            //foreach (var item in sale.SaleItems)
            //{
            //    Db.SaleItems.Add(item);
            //}
            //SaveChanges();
            sale.SaleItems = null;
            sale.InvoiceDcs = null;

            //Db.Entry(dbSale).CurrentValues.SetValues(sale);

            base.Update(sale, true, false);
        }

        public List<Sale> GetByAccountId(int accountid, List<VoucherType> transactiontypes)
        {
            return FiscalCollection.Where(p => p.AccountId == accountid).Where(p => transactiontypes.Contains(p.TransactionType)).ToList();
        }
        public List<IdName> IsSaleExitAfterPurchase(int purchaseId, Sale currenEntry = null)
        {

            var itemIds = new List<int>();
            var dbSaleItems = new List<IdName>();
            var purchaseDate = DateTime.Now.Date;
            if (purchaseId > 0)
            {
                var dbPurchase = GetById(purchaseId, true);
                purchaseDate = dbPurchase.Date;
                itemIds = dbPurchase.SaleItems.Select(p => p.ItemId).ToList();
                if (currenEntry != null)
                {
                    itemIds = new List<int>();
                    foreach (var item in dbPurchase.SaleItems)
                    {
                        var oldItem = currenEntry.SaleItems.FirstOrDefault(p => p.ItemId == item.ItemId);
                        if (oldItem == null)
                        {
                            //items is deleted from new updated purcahse entry
                            itemIds.Add(item.ItemId);
                        }
                        //make sure only changed items are chcked
                        else if (oldItem.Quantity != item.Quantity || oldItem.Rate != item.Rate)
                        {
                            itemIds.Add(item.ItemId);
                        }
                    }
                    itemIds.AddRange(currenEntry.SaleItems.Where(p => p.Id == 0).Select(p => p.ItemId));
                }
            }
            else if (currenEntry != null)
            {

                purchaseDate = currenEntry.Date;
                itemIds = currenEntry.SaleItems.Select(p => p.ItemId).ToList();
            }



            if (itemIds.Count > 0)
            {
                var sqlquery = string.Format("EXEC [dbo].[IsSaleExitAfterPurchase] @Date={0},@ItemIds = '{1}',@FiscalId={2},@CompanyId = {3}", "'" + purchaseDate.ToString("yyyy-MM-dd") + "'", string.Join(",", itemIds), SiteContext.Current.Fiscal.Id, SiteContext.Current.User.CompanyId);
                return Db.Database.SqlQuery<IdName>(sqlquery).ToList();
            }
            else
                return dbSaleItems;


            //return (from s in FiscalCollection
            //        join si in Db.SaleItems on s.Id equals si.SaleId
            //        where s.Date >= purchaseDate && itemIds.Contains(si.Id)
            //        && (s.TransactionType == VoucherType.Sale || s.TransactionType == VoucherType.SaleReturn)
            //        select new IdName() { Id = si.Id, Name = si.ItemName })
            //   .ToList();

        }

        public List<InvoiceClearingExtra> GetInvoicesByAccountId(int accountid, List<int> types)
        {
            var sqlquery = string.Format("EXEC [dbo].[GetInvoiceClearance] @CompanyId = {0},@AccountId={1},@VoucherTypes = '{2}',@FiscalId={3}", SiteContext.Current.User.CompanyId, accountid, string.Join(",", types), SiteContext.Current.Fiscal.Id);
            return Db.Database.SqlQuery<InvoiceClearingExtra>(sqlquery).ToList();
        }
        public List<GetSalePurchaseDateWiseSummaryByVoucherTypeExtra> GetSalePurchaseDateWiseSummary(DateTime FromDate, DateTime ToDate, VoucherType VoucherType)
        {
            var sqlquery = string.Format("EXEC [dbo].[GetSalePurchaseDateWiseSummaryByVoucherType] @FromDate = {0},@ToDate = {1},@CompanyId = {2},@VoucherType = '{3}',@FiscalId={4}",
               "'" + FromDate.ToString("yyyy-MM-dd") + "'", "'" + ToDate.ToString("yyyy-MM-dd") + "'", SiteContext.Current.User.CompanyId, VoucherType, SiteContext.Current.Fiscal.Id);
            return Db.Database.SqlQuery<GetSalePurchaseDateWiseSummaryByVoucherTypeExtra>(sqlquery).ToList();
        }
        public List<NTVehicleDetail> GetNTVehicleDetail(DateTime FromDate, DateTime ToDate)
        {
            var sqlquery = string.Format("EXEC [dbo].[GetNTDeliveryChallanDetail] @FromDate = {0},@ToDate = {1},@CompanyId = {2},@FiscalId = '{3}'",
               "'" + FromDate.ToString("yyyy-MM-dd") + "'", "'" + ToDate.ToString("yyyy-MM-dd") + "'", SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id);
            return Db.Database.SqlQuery<NTVehicleDetail>(sqlquery).ToList();
        }
        public List<SaleDetailForDetailedLedgerExtra> GetSaleDetailForDetailedLedger(DateTime fromDate, DateTime toDate, List<int> types)
        {
            var sqlquery = string.Format("EXEC [dbo].[GetSaleDetailForDetailedLedger]  @FROMDATE = '{0}', @TODATE='{1}', @CompanyId = {2},@VoucherTypeIds = '{3}',@FiscalId={4}", fromDate.ToString("yyyy-MM-dd"), toDate.ToString("yyyy-MM-dd"), SiteContext.Current.User.CompanyId, string.Join(",", types), SiteContext.Current.Fiscal.Id);
            return Db.Database.SqlQuery<SaleDetailForDetailedLedgerExtra>(sqlquery).ToList();
        }
        public List<SaleDetailForDetailedLedgerExtra> GetDCDetailForNTDetailedLedger(DateTime fromDate, DateTime toDate, List<VoucherType> types)
        {
            var sqlquery = string.Format("EXEC [dbo].[GetDCDetailForNTDetailedLedger]  @FROMDATE = '{0}', @TODATE='{1}', @CompanyId = {2},@VoucherTypeIds = '{3}',@FiscalId={4}", fromDate.ToString("yyyy-MM-dd"), toDate.ToString("yyyy-MM-dd"), SiteContext.Current.User.CompanyId, string.Join(",", types), SiteContext.Current.Fiscal.Id);
            return Db.Database.SqlQuery<SaleDetailForDetailedLedgerExtra>(sqlquery).ToList();
        }

        public List<KlassVoucherExtra> GetKlassInvalidData(string query)
        {

            return Db.Database.SqlQuery<KlassVoucherExtra>(query).ToList();

        }



    }
}