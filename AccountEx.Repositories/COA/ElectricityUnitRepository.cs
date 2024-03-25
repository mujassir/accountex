using AccountEx.CodeFirst.Models.COA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Repositories.COA
{
    public class ElectricityUnitRepository : GenericRepository<ElectricityUnit>
    {
        public ElectricityUnitRepository() : base() { }
        public ElectricityUnitRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }

        public int GetNextVoucherNo()
        {
            var maxnumber = 1001;
            if (!FiscalCollection.Any())
                return maxnumber;
            return FiscalCollection.OrderByDescending(p => p.VoucherNo).FirstOrDefault().VoucherNo + 1;
        }
        
        public ElectricityUnit GetByVoucherNo(int voucherno, string key, out bool next, out bool previous)
        {
            ElectricityUnit v = null;

            switch (key)
            { 
                case "first":
                    v = FiscalCollection.OrderBy(p => p.VoucherNo).FirstOrDefault();
                    break;
                case "last":
                    v = FiscalCollection.OrderByDescending(p => p.VoucherNo).FirstOrDefault();
                    break;
                case "next":
                    v = FiscalCollection.Where(p => p.VoucherNo > voucherno).OrderBy(p => p.VoucherNo).FirstOrDefault();
                    break;
                case "previous":
                    v = FiscalCollection.Where(p => p.VoucherNo < voucherno).OrderByDescending(p => p.VoucherNo).FirstOrDefault();
                    break;
                case "same":
                    v = FiscalCollection.FirstOrDefault(p => p.VoucherNo == voucherno);
                    break;

            }

            if (v != null)
                voucherno = v.VoucherNo;
            else if (key != "nextvouchernumber" && key != "same")
            {
                v = FiscalCollection.OrderByDescending(p => p.VoucherNo).FirstOrDefault();
            }
                    next = FiscalCollection.Any(p => p.VoucherNo > voucherno);
                    previous = FiscalCollection.Any(p => p.VoucherNo < voucherno);
                    return v;
                }

        public bool CheckIfVoucherNoExist(int VoucherNo,int id)
        {
            return FiscalCollection.Any(p => p.VoucherNo == VoucherNo && p.Id != id);
        }
        public bool CheckIfElectrictyExist(int id, int year, int month)
        {
            return FiscalCollection.Any(p => p.Year == year && p.Month == month && p.Id != id);
        }
        public bool CheckIfElectrictyExist(int year, int month)
        {
            return FiscalCollection.Any(p => p.Year == year && p.Month == month);
        }

        //public override void Save(ElectricityUnit entity)
        //{
        //    var repo = new ElectricityUnitRepository();

        //    if (entity.Id == 0)
        //    {
        //        //entity.Year = DateTime.Now.Year;
        //        new ElectricityUnitRepository().Add(entity);
        //    }
        //    else
        //    {
        //        repo.Update(entity);
        //    }
        //}
        //public override void Update(ElectricityUnit entity)
        //{
        //    string query = "Delete from ElectricityUnitItems where ElectricityUnitId=" + entity.Id;
        //    Db.Database.ExecuteSqlCommand(query);
        //    foreach (var row in entity.ElectricityUnitItems)
        //    {
        //        row.ElectricityUnitId = entity.Id;
        //        Db.ElectricityUnitItems.Add(row);
        //    }
        //    entity.ElectricityUnitItems = null;
        //    base.Update(entity);
        //    SaveChanges();

        //}

        public void DeleteByVoucherNo(int VoucherNo)
        {
            var record = FiscalCollection.FirstOrDefault(p => p.VoucherNo == VoucherNo);
            Db.ElectricityUnits.Remove(record);
            Db.SaveChanges();
        }
        public ElectricityUnit GetRecordByMonthYear(int month, int year)
        {
            return Collection.FirstOrDefault(p => p.Month == month && p.Year == year);
        }

        public int GetVoucherNoByMonthYear(int month, int year)
        {
            if (Collection.Any(p => p.Month == month && p.Year == year))
                return FiscalCollection.FirstOrDefault(p => p.Month == month && p.Year == year).VoucherNo;
            else
                return 0;

        }
        public ElectricityUnit GetPreviousReading(int month, int year)
        {
            var previousMonth = month - 1 ;
            if(previousMonth != 0)
                return Collection.FirstOrDefault(p => p.Month == previousMonth && p.Year == year);
            else
                return Collection.FirstOrDefault(p => p.Month == 12 && p.Year == (year - 1));
        }
    }
  }




