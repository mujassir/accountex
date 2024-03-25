using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountEx.CodeFirst;
using AccountEx.Common;

namespace AccountEx.Repositories
{
    public class SalaryConfigRepository : GenericRepository<SalaryConfig>
    {

        public SalaryConfig GetById(int configId, string key, out bool next, out bool previous)
        {
            SalaryConfig v = null;
            switch (key)
            {
                case "first":
                    v = Collection.OrderBy(p => p.Id).FirstOrDefault();
                    break;
                case "last":
                    v = Collection.OrderByDescending(p => p.Id).FirstOrDefault();
                    break;
                case "next":
                    v = Collection.Where(p => p.Id > configId).OrderBy(p => p.Id).FirstOrDefault();
                    break;
                case "previous":
                    v = Collection.Where(p => p.Id < configId).OrderByDescending(p => p.Id).FirstOrDefault();
                    break;
                case "same":
                    v = Collection.FirstOrDefault(p => p.Id == configId);
                    break;
            }
            if (v != null) configId = v.Id;
            next = Collection.Any(p => p.Id > configId);
            previous = Collection.Any(p => p.Id < configId);
            return v;
        }

        public bool IsAlreadyExistInDates(int id, DateTime fromDate, DateTime toDate, out string configName)
        {
            configName = "";
            if (id != 0) return false;
            var config = Collection.FirstOrDefault(p =>
                    (fromDate >= p.FromDate && fromDate <= p.ToDate)
                || (toDate >= p.FromDate && toDate <= p.ToDate)
                );
            if (config != null)
            {
                configName = config.Name;
                return true;
            }
            return false;

        }
        public bool IsAlreadyExistInDates(int id, DateTime fromDate, DateTime toDate)
        {
            return Collection.Any(p =>
                    (fromDate >= p.FromDate && fromDate <= p.ToDate)
                || (toDate >= p.FromDate && toDate <= p.ToDate)
                );
        }
        public static bool PublicInstancePropertiesEqual<T>(T self, T to, params string[] ignore) where T : class
        {
            if (self != null && to != null)
            {
                Type type = typeof(T);
                List<string> ignoreList = new List<string>(ignore);
                foreach (System.Reflection.PropertyInfo pi in type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
                {
                    if (!ignoreList.Contains(pi.Name))
                    {
                        object selfValue = type.GetProperty(pi.Name).GetValue(self, null);
                        object toValue = type.GetProperty(pi.Name).GetValue(to, null);

                        if (selfValue != toValue && (selfValue == null || !selfValue.Equals(toValue)))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            return self == to;
        }

        public SalaryConfig GetSalaryConfigBydate(DateTime fromDate, DateTime toDate)
        {
          
            var config = Collection.FirstOrDefault(p =>
                    (fromDate >= p.FromDate && fromDate <= p.ToDate)
                || (toDate >= p.FromDate && toDate <= p.ToDate)
                );

            return config;

        }

        public override void Save(SalaryConfig entity)
        {
            if (entity.Id == 0)
            {
                base.Save(entity,true,true);
                return;
            }


            var config = Collection.FirstOrDefault(p => p.Id == entity.Id);
            var dbItems = config.SalaryConfigItems.ToList();
            var dt = DateTime.Now;
            foreach (var item in entity.SalaryConfigItems)
            {
                var dbItem = dbItems.FirstOrDefault(p => p.Id == item.Id);
                if (dbItem == null)
                {
                    item.SalaryConfigId = entity.Id;
                    Db.SalaryConfigItems.Add(item);
                }
                else
                {
                    item.CreatedAt = dbItem.CreatedAt;
                    item.ModifiedAt = dbItem.ModifiedAt;
                    item.CreatedBy = dbItem.CreatedBy;
                    item.ModifiedBy = dbItem.ModifiedBy;
                    item.CompanyId = dbItem.CompanyId;
                    item.SalaryConfigId = config.Id;
                    if (PublicInstancePropertiesEqual(dbItem, item) == false)
                    {
                        if (dbItem.CreatedAt.Date == DateTime.Today)
                        {
                            Db.SalaryConfigItems.Remove(dbItem);
                        }
                        else
                        {
                            dbItem.IsActive = false;
                        }
                        dbItem.ModifiedAt = dt;
                        dbItem.ModifiedBy = SiteContext.Current.User.Id;
                        item.Id = 0;
                        Db.SalaryConfigItems.Add(item);
                    }

                }
            }


            Db.SaveChanges();
        }
    }
}
