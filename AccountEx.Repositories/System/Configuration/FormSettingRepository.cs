using System.Collections.Generic;
using System.Linq;
using AccountEx.CodeFirst.Models;

namespace AccountEx.Repositories
{
    public class FormSettingRepository : GenericRepository<FormSetting>
    {


        public List<FormSetting> GetFormSettingByVoucherType(string vouchertype)
        {
            return Collection.Where(p => p.VoucherType == vouchertype).ToList();
        }


    }
}
