using AccountEx.CodeFirst.Models.COA;
using AccountEx.CodeFirst.Models.Transactions;
using AccountEx.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Repositories.Transactions
{
    public class BankReceiptRepository : GenericRepository<BankReceipt>
    {
            public BankReceiptRepository() : base() { }
            public BankReceiptRepository(BaseRepository repo)
            {
                base.Db = repo.GetContext();
            }
            public int GetNextVoucherNumber()
            {
                var maxnumber = 1001;
                if (!FiscalCollection.Any())
                    return maxnumber;
                return FiscalCollection.OrderByDescending(p => p.VoucherNumber).FirstOrDefault().VoucherNumber + 1;
            }
            public BankReceipt GetPreviousChallanByMonth(int toMonth)
            {

                return FiscalCollection.OrderByDescending(p => p.VoucherNumber).FirstOrDefault();
            }

        public override void Update(BankReceipt record)
            {
            var repo = new BankReceiptRepository(this);
            var bankReceiptItemRepo = new BankReceiptItemRepository(this);
            var challanItemRepo = new ChallanItemRepository(this);
            var dbBankReceipt = repo.GetById(record.Id, true);

            var ids = record.BankReceiptItems.Select(p => p.Id).ToList();
            var deletedIds = dbBankReceipt.BankReceiptItems.Where(p => !ids.Contains(p.Id)).Select(p => p.Id).ToList();
            foreach (var item in record.BankReceiptItems)
                    {
                if (item.Id == 0)
                {
                    var challanItem = challanItemRepo.GetById(item.ChallanItemId);
                        challanItem.IsReceived = true;
                    challanItemRepo.SaveChanges();
                        }
                    }
            foreach (int i in deletedIds)
                    {
                int challanItemId = dbBankReceipt.BankReceiptItems.Where(p => p.Id == i).FirstOrDefault().ChallanItemId;
                var challanItem = challanItemRepo.GetById(challanItemId);
                challanItem.IsReceived = false;
                challanItemRepo.SaveChanges();
                }              
            bankReceiptItemRepo.Delete(deletedIds);
            bankReceiptItemRepo.Save(false, record.BankReceiptItems.ToList());
            base.Update(record, true, false);
            }

            public BankReceipt GetByVoucherNumber(int voucherno, string key, out bool next, out bool previous)
            {
                BankReceipt v = null;
                switch (key)
                {
                    case "first":
                        v = FiscalCollection.OrderBy(p => p.VoucherNumber).FirstOrDefault();
                        break;
                    case "last":
                        v = FiscalCollection.OrderByDescending(p => p.VoucherNumber).FirstOrDefault();
                        break;
                    case "next":
                        v = FiscalCollection.Where(p => p.VoucherNumber > voucherno).OrderBy(p => p.VoucherNumber).FirstOrDefault();
                        break;
                    case "previous":
                        v = FiscalCollection.Where(p => p.VoucherNumber < voucherno).OrderByDescending(p => p.VoucherNumber).FirstOrDefault(); ;
                        break;
                    case "same":
                        v = FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno);
                        break;
                    case "challan":
                        v = FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno);
                        break;

                }

                if (v != null)
                    voucherno = v.VoucherNumber;
                else if (key != "nextvouchernumber" && key != "challan")
                {
                    v = FiscalCollection.OrderByDescending(p => p.VoucherNumber).FirstOrDefault();

                }
                if (v == null && !FiscalCollection.Any())
                {
                    v = new BankReceipt();
                    v.VoucherNumber = ConfigurationReader.GetConfigKeyValue<int>("VoucherStartNumber", 1001);
                }
                next = FiscalCollection.Any(p => p.VoucherNumber > voucherno);
                previous = FiscalCollection.Any(p => p.VoucherNumber < voucherno);
                return v;
            }
    }
}
