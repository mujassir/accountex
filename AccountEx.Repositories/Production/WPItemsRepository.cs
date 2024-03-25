using AccountEx.CodeFirst.Models;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Linq.Expressions;
using AccountEx.CodeFirst.Models.Production;
using AccountEx.Common;
namespace AccountEx.Repositories
{
    public class WPItemsRepository: GenericRepository<WPItem>
    {
        public WPItemsRepository() : base() { }
        public WPItemsRepository(BaseRepository repo) 
        {
            base.Db = repo.GetContext();
        }

        public virtual List<WPItemWithParentDetail> GetWithParentDetail(Expression<Func<WPItemWithParentDetail, bool>> predicate)
        {
            var query = from wpItem in Db.WPItem.Where(e => e.CompanyId == SiteContext.Current.User.CompanyId)
                        join workInProgress in Db.WorkInProgresses.Where(e => e.FiscalId == SiteContext.Current.Fiscal.Id)
                        on wpItem.WorkInProgressId equals workInProgress.Id into workInProgressGroup
                        from wpi in workInProgressGroup.DefaultIfEmpty()
                        select new WPItemWithParentDetail
                        {
                            OrderNo = wpi.OrderNo,
                            VoucherType = wpi.VoucherType,
                            InvoiceNumber = wpi.InvoiceNumber,
                            VoucherNumber = wpi.VoucherNumber,
                            Date = wpi.Date,
                            CreatedDate = wpi.CreatedDate,
                            // WPItem properties
                            Id = wpItem.Id,
                            WorkInProgressId = wpItem.WorkInProgressId,
                            ItemCode = wpItem.ItemCode,
                            ItemName = wpItem.ItemName,
                            ItemId = wpItem.ItemId,
                            Amount = wpItem.Amount,
                            Quantity = wpItem.Quantity,
                            Rate = wpItem.Rate,
                            Weight = wpItem.Weight,
                            TotalWeight = wpItem.TotalWeight,
                            RequisitionNo = wpItem.RequisitionNo,
                            GINPNo = wpItem.GINPNo,
                            Width = wpItem.Width,
                            Meters = wpItem.Meters,
                            Rolls = wpItem.Rolls,
                            TotalRolls = wpItem.TotalRolls,
                            TotalMeters = wpItem.TotalMeters,
                            Comments = wpItem.Comments
                        };

            return query.Where(predicate).ToList();
        }

    }
}
