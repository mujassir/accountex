using Entities.CodeFirst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Repositories
{
    public class CurrencyRepository : GenericRepository<Currency>
    {
        public CurrencyRepository() : base() { }
        public CurrencyRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }

        public Currency GetCurrencyById(int? id)
        {
            var cur = Collection.FirstOrDefault(p => p.Id == id);
            return cur;
        }
        public int GetIdByUnitName(string unit)
        {

            if (Collection.Any(p => p.Unit.ToLower() == unit.ToLower()))
                return Collection.FirstOrDefault(p => p.Unit.ToLower() == unit.ToLower()).Id;
            else return 0;
        }
        public int GetIdByShortName(string shortName)
        {

            if (Collection.Any(p => p.ShortName.ToLower() == shortName.ToLower()))
                return Collection.FirstOrDefault(p => p.ShortName.ToLower() == shortName.ToLower()).Id;
            else return 0;
        }
    }
}
