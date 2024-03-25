using AccountEx.CodeFirst.Models.Lab;
using AccountEx.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AccountEx.Repositories.Lab
{
    public class ParameterRepository : GenericRepository<Parameter>
    {
        public ParameterRepository() : base() { }
        public ParameterRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }
        public List<Parameter> GetByParameterIds(List<int> pids)
        {
            return Collection.Where(p => pids.Contains(p.Id)).ToList();
        }
        public Parameter GetByName(string name)
        {
            name = name.ToLower();
            return Collection.FirstOrDefault(p => p.Name.ToLower() == name);
        }
        public List<Parameter> GetAll(TestType type)
        {
            var testType = Convert.ToByte(type);
            return Collection.Where(p => p.Type == testType).ToList();
        }
    }
}
