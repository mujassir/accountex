using AccountEx.CodeFirst.Models;
using System.Collections.Generic;
using System.Linq;

namespace AccountEx.Repositories
{
    public class LogMappingRepository : GenericRepository<LogMapping>
    {
        public List<LogMapping> GetAllMapping()
        {
            return Db.LogMappings.ToList();
        }
    }
}
