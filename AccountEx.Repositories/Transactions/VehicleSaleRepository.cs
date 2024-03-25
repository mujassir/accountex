using AccountEx.CodeFirst.Models;
using AccountEx.CodeFirst.Models.Vehicles;
using AccountEx.Common;
using Entities.CodeFirst;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AccountEx.Repositories
{
    public class VehicleSaleRepository : GenericRepository<VehicleSale>
    {

        public VehicleSaleRepository() : base() { }
        public VehicleSaleRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }
        public override void Update(VehicleSale input)
        {
            var vehicleRepo = new VehicleRepository(this);
            var dbSale = FiscalCollection.FirstOrDefault(p => p.Id == input.Id);
            var dbVehicleId = dbSale.VehicleId;
            var query = "Delete from VehicleSaleDetails where VehicleSaleId=" + input.Id + " and CompanyId=" + SiteContext.Current.User.CompanyId;
            //query += "; Delete from SaleDocuments where SaleId=" + input.Id + " and CompanyId=" + SiteContext.Current.User.CompanyId;
            query += "; Delete from VehicleSaleDeposits where SaleId=" + input.Id + " and CompanyId=" + SiteContext.Current.User.CompanyId;
            Db.Database.ExecuteSqlCommand(query);
            //foreach (var item in input.SaleDocuments)
            //{
            //    item.Id = 0;
            //    item.CompanyId = SiteContext.Current.User.CompanyId;
            //    Db.SaleDocuments.Add(item);
            //}
            foreach (var item in input.VehicleSaleDetails)
            {
                item.Id = 0;
                item.CompanyId = SiteContext.Current.User.CompanyId;
                Db.VehicleSaleDetails.Add(item);
            }
            foreach (var item in input.VehicleSaleDeposits)
            {
                item.SaleId = input.Id;
                Db.VehicleSaleDeposits.Add(item);
            }
            SaveChanges();
            input.VehicleSaleDetails = null;
            input.SaleDocuments = null;
            input.VehicleSaleDeposits = null;

            input.TransactionType = VoucherType.Sale;

            if (input.VehicleId != dbVehicleId)
            {
                var vehicle = vehicleRepo.GetById(dbVehicleId);
                vehicle.IsSale = false;

            }
            Db.Entry(dbSale).CurrentValues.SetValues(input);
            SaveChanges();
        }
        public int GetNextVoucherNumber(VoucherType vouchertype)
        {
            var maxnumber = ConfigurationReader.GetConfigKeyValue<int>("VoucherStartNumber", 1001);
            if (!FiscalCollection.Any())
                return maxnumber;
            return FiscalCollection.OrderByDescending(p => p.VoucherNumber).FirstOrDefault().VoucherNumber + 1;
        }
        public bool IsVoucherExits(int voucherno, VoucherType type, int id)
        {
            return FiscalCollection.Any(p => p.VoucherNumber == voucherno && p.TransactionType == type && p.Id != id);
        }
        public int GetCustomerId(int Id)
        {
            if (Collection.Any(p => p.Id == Id))
                return Collection.FirstOrDefault(p => p.Id == Id).AccountId;
            else return 0;
        }
        public decimal GetAgreementBalance(int Id)
        {
            if (Collection.Any(p => p.Id == Id))
                return Collection.FirstOrDefault(p => p.Id == Id).Balance;
            else return 0;
        }
        public int GetBranchId(int Id)
        {
            if (Collection.Any(p => p.Id == Id))
                return Collection.FirstOrDefault(p => p.Id == Id).BranchId;
            else return 0;
        }

        public decimal GetBalance(int saleId, int customerId, int vehicleId)
        {

            var balance = 0.0M;
            var reportRepo = new ReportRepository();
            var vehicleRepo = new vw_VehiclesRepository();
            var paymentRepo = new VehicleInstallmentPaymentRepository();
            var transRepo = new TransactionRepository();
            var agreementBalance = GetAgreementBalance(saleId);
            var auctionpenaltyTotal = transRepo.GetVehicleAuctionnerPenaltyTotal(vehicleId, customerId);
            var payments = paymentRepo.GetSumBySaleId(saleId);
            balance = (agreementBalance + auctionpenaltyTotal) - payments;
            return balance;

        }

        public VehicleSale UpdateRecoveryStatus(int id, byte status)
        {
            var sale = Collection.FirstOrDefault(p => p.Id == id);
            if (sale != null)
            {
                sale.RecoveryStatus = status;
            }
            if (status == (byte)RecoveryStatus.InventoryReturn && sale != null)
            {
                var v = AsQueryable<Vehicle>().FirstOrDefault(p => p.Id == sale.VehicleId);
                if (v != null)
                {
                    v.IsSale = false;
                }
            }
            if (status == (byte)RecoveryStatus.Recovered && sale != null)
            {
                var v = AsQueryable<VehicleAcution>().FirstOrDefault(p => p.SaleId == sale.Id);
                if (v != null)
                {
                    v.RepossessedDate = DateTime.Now;
                }
            }
            SaveChanges();
            return sale;
        }
        public VehicleSale UpdateRecoveryOnCustomerReturn(int id, byte status, string remarks)
        {
            var sale = Collection.FirstOrDefault(p => p.Id == id);
            if (sale != null)
            {
                sale.RecoveryStatus = status;
                sale.AgreementRemarks = remarks;
            }

            SaveChanges();
            return sale;
        }
        public void Delete(VehicleSale sale)
        {
            if (sale != null)
                Db.VehicleSales.Remove(sale);
            Db.SaveChanges();
        }
        public int GetVehicleId(int Id)
        {
            if (Collection.Any(p => p.Id == Id))
                return Collection.FirstOrDefault(p => p.Id == Id).VehicleId;
            else return 0;
        }
        public VehicleSale GetByVehicleId(int vehicleId)
        {

            return Collection.FirstOrDefault(p => p.VehicleId == vehicleId);

        }
        public bool IsTradeUnitUsed(int vehicleId)
        {

            return Collection.Any(p => p.TradeInVehicleId == vehicleId && p.RecoveryStatus != (byte)RecoveryStatus.SaleReturn);

        }
        public int GetNextInvoiceNumber(VoucherType vouchertype)
        {

            var maxnumber = ConfigurationReader.GetConfigKeyValue<int>("InvoiceStartNumber", 1);
            if (!FiscalCollection.Any())
                return maxnumber;
            return FiscalCollection.OrderByDescending(p => p.InvoiceNumber).FirstOrDefault().InvoiceNumber + 1;
        }
        public VehicleSale GetByVoucherNumber(int voucherno, VoucherType vtype, string key, out bool next, out bool previous)
        {
            VehicleSale v = null;
            switch (key)
            {
                case "first":
                    v = FiscalCollection.Where(p => p.SaleType == vtype).OrderBy(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "last":
                    v = FiscalCollection.Where(p => p.SaleType == vtype).OrderByDescending(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "next":
                    v = FiscalCollection.Where(p => p.SaleType == vtype && p.VoucherNumber > voucherno).OrderBy(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "previous":
                    v = FiscalCollection.Where(p => p.SaleType == vtype && p.VoucherNumber < voucherno).OrderByDescending(p => p.VoucherNumber).FirstOrDefault(); ;
                    break;
                case "same":
                    v = FiscalCollection.FirstOrDefault(p => p.SaleType == vtype && p.VoucherNumber == voucherno);
                    break;
                case "challan":
                    v = FiscalCollection.FirstOrDefault(p => p.SaleType == vtype && p.VoucherNumber == voucherno);
                    break;

            }

            if (v != null)
                voucherno = v.VoucherNumber;
            else if (key != "nextvouchernumber" && key != "challan")
            {
                v = FiscalCollection.Where(p => p.SaleType == vtype).OrderByDescending(p => p.VoucherNumber).FirstOrDefault();

            }
            if (v == null && !FiscalCollection.Any())
            {
                v = new VehicleSale();
                v.VoucherNumber = ConfigurationReader.GetConfigKeyValue<int>("VoucherStartNumber", 1001);
                v.InvoiceNumber = ConfigurationReader.GetConfigKeyValue<int>("InvoiceStartNumber", 1001);
                v.Date = DateTime.Now;
            }
            next = FiscalCollection.Any(p => p.SaleType == vtype && p.VoucherNumber > voucherno);
            previous = FiscalCollection.Any(p => p.SaleType == vtype && p.VoucherNumber < voucherno);
            return v;
        }
    }
}
