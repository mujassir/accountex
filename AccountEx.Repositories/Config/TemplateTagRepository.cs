
using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Repositories
{
    public class TemplateTagRepository : GenericRepository<TemplateTag>
    {
        public  List<TemplateTagExtra> GetNames()
        {
            return Collection.Select(p => new TemplateTagExtra()
            {
               
                Name = p.Lable + " (" + p.Tag + " )",
                Tag=p.Tag

            }).ToList();
        }

    }
}
