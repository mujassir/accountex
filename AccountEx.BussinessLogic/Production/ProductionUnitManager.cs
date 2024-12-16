using AccountEx.CodeFirst.Models;
using AccountEx.CodeFirst.Models.Production;
using AccountEx.CodeFirst.Models.Stock;
using AccountEx.Common;
using AccountEx.Repositories;
using AccountEx.Repositories.Production;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace AccountEx.BussinessLogic.Production
{
    public class ProductionUnitManager
    {
        public static void Save(ProductionUnit p)
        {
            var repo = new ProductionUnitRepository();
           if(p.Id > 0)
            {
                p.ModifiedAt = DateTime.Now;
                repo.Update(p);
            } else
            {
                p.CreatedDate = DateTime.Now;
                p.VoucherNumber = new ProductionUnitRepository().GetNextVoucherNumber(p.LocationId);
                repo.Add(p, true, false);
                new ProductionUnitItemRepository().Save(p.Items.ToList());
            }
            var repo2 = new InternalStockTransferRepository();
            var VoucherNumber = repo2.GetNextVoucherNumber((int)StockTransferType.MachineWise);

            switch (p.Status)
            {
                case ProductionStatus.StockReceived:
                    StockReceived(p);
                    break;
                case ProductionStatus.ProductionCompleted:
                    ManageWasteConsumeAndFinishStock(p);
                    break;
            }
            repo.SaveChanges();
        }
        private static void StockReceived(ProductionUnit p)
        {
            var repo = new InternalStockTransferRepository();
            var VoucherNumber = repo.GetNextVoucherNumber((int)StockTransferType.MachineWise);
            var items = p.Items
                .Where(x => x.IssueQty > 0 && x.Type == ProductionUnitItemType.Raw)
                .Select(x => new InternalStockTransferItem()
                {
                    InternalStockTransferId = 0,
                    ItemCode = x.ItemCode,
                    ItemName = x.ItemName,
                    ItemId = x.ItemId,
                    VoucherNumber = VoucherNumber,
                    InvoiceNumber = 0,
                    Quantity = x?.IssueQty ?? 0,
                    Rate = x.Rate,
                    Amount = (x?.IssueQty ?? 0) * x.Amount,
                    ArticleNo = null,
                    Unit = null,
                    Status = 1,
                    QuantityDelivered = x?.IssueQty ?? 0,
                    StockBalance = 0,
                    Type = (int)StockType.Active
                }).ToList();

            if(items.Count > 0)
            {
                InternalStockTransfer item = new InternalStockTransfer()
                {
                    VoucherNumber = VoucherNumber,
                    VoucherCode = null,
                    FiscalId = p.FiscalId,
                    Date = p.Date,
                    InvoiceNumber = 0,
                    DeliveryRequired = null,
                    Description = null,
                    Status = 1,
                    Instructions = null,
                    FromLocationId = p.LocationId,
                    ToLocationId = p.LocationId,
                    FromWarehouseId = p.StockWarehouseId,
                    ToWarehouseId = 0,
                    FromMachineId = 0,
                    ToMachineId = p.MachineId,
                    StockTransferType = 2,
                    InternalStockTransferItems = items
                };
                repo.Add(item, true, true);
            }
        }
        private static void ManageWasteConsumeAndFinishStock(ProductionUnit p)
        {
            var repo = new InternalStockTransferRepository();
            var pVoucherNumber = repo.GetNextVoucherNumber((int)StockTransferType.Production);
            var voucherNumber = repo.GetNextVoucherNumber((int)StockTransferType.MachineWise);

            // Create consume and waste items
            var consumeAndWasteItems = GetConsumeAndWasteItems(p, pVoucherNumber);

            // If there are any items, create the transfer
            if (consumeAndWasteItems.Count > 0)
            {
                CreateStockTransfer(p, pVoucherNumber, consumeAndWasteItems, 3, p.LocationId, p.LocationId, 0, p.MachineId, 0);
            }

            // Create Remaining stock items
            var remainingItems = p.Items
                .Where(x => x.Type == ProductionUnitItemType.Raw && x.RemainingQty > 0)
                .Select(x => CreateInternalStockTransferItem(x, voucherNumber, StockType.Active, x.RemainingQty ?? 0))
                .ToList();

            // If there are remaining items, create the transfer
            if (remainingItems.Count > 0)
            {
                CreateStockTransfer(p, voucherNumber, remainingItems, 2, p.LocationId, p.LocationId, p.RawStockWarehouseId, 0, p.MachineId);
            }

            // Create finished stock items
            var finishItems = p.Items
                .Where(x => x.Type == ProductionUnitItemType.Finished)
                .Select(x => CreateInternalStockTransferItem(x, pVoucherNumber + 1, StockType.Active, x.Quantity))
                .ToList();

            // If there are finished items, create the transfer
            if (finishItems.Count > 0)
            {
                CreateStockTransfer(p, pVoucherNumber + 1, finishItems, 3, p.LocationId, p.LocationId, 0, p.MachineId, 0);

                var finishItemsToMove = p.Items
                    .Where(x => x.Type == ProductionUnitItemType.Finished)
                    .Select(x => CreateInternalStockTransferItem(x, voucherNumber + 1, StockType.Active, x.Quantity))
                    .ToList();
                CreateStockTransfer(p, voucherNumber + 1, finishItemsToMove, 2, p.LocationId, p.LocationId, p.FinishStockWarehouseId, 0, p.MachineId);
            }
        }

        /// <summary>
        /// Generates a list of consume and waste items from the production unit.
        /// </summary>
        private static List<InternalStockTransferItem> GetConsumeAndWasteItems(ProductionUnit p, int voucherNumber)
        {
            var items = p.Items
                .Where(x => x.Type == ProductionUnitItemType.Raw)
                .Select(x => CreateInternalStockTransferItem(x, voucherNumber, StockType.Consume, x.Quantity)).ToList();

            var wasteItems = p.Items
                .Where(x => x.WasteQty > 0 && x.Type == ProductionUnitItemType.Raw)
                .Select(x => CreateInternalStockTransferItem(x, voucherNumber, StockType.Waste, x.WasteQty ?? 0))
                .ToList();

            return items.Concat(wasteItems).ToList();
        }

        /// <summary>
        /// Creates a single InternalStockTransferItem.
        /// </summary>
        private static InternalStockTransferItem CreateInternalStockTransferItem(
            ProductionUnitItem item, int voucherNumber, StockType stockType, decimal quantity)
        {
            return new InternalStockTransferItem()
            {
                InternalStockTransferId = 0,
                ItemCode = item.ItemCode,
                ItemName = item.ItemName,
                ItemId = item.ItemId,
                VoucherNumber = voucherNumber,
                InvoiceNumber = 0,
                Quantity = quantity,
                Rate = item.Rate,
                Amount = quantity * item.Amount,
                ArticleNo = null,
                Unit = null,
                Status = 1,
                QuantityDelivered = quantity,
                StockBalance = 0,
                Type = (int)stockType
            };
        }

        /// <summary>
        /// Creates and adds an InternalStockTransfer to the repository.
        /// </summary>
        private static void CreateStockTransfer(
            ProductionUnit p, int voucherNumber, List<InternalStockTransferItem> items,
            int StockTransferType,
            int fromLocationId, int toLocationId, int toWarehouseId, int toMachineId, int fromMachineId)
        {
            var transfer = new InternalStockTransfer()
            {
                VoucherNumber = voucherNumber,
                VoucherCode = null,
                FiscalId = p.FiscalId,
                Date = p.Date,
                InvoiceNumber = 0,
                DeliveryRequired = null,
                Description = null,
                Status = 1,
                Instructions = null,
                FromLocationId = fromLocationId,
                ToLocationId = toLocationId,
                FromWarehouseId = 0,
                ToWarehouseId = toWarehouseId,
                FromMachineId = fromMachineId,
                ToMachineId = toMachineId,
                StockTransferType = StockTransferType,
                InternalStockTransferItems = items
            };

            var repo = new InternalStockTransferRepository();
            repo.Add(transfer, true, true);
        }
    }
}
