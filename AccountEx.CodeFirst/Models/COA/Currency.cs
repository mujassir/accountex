using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountEx.CodeFirst.Models;
using System;
using System.Collections.Generic;

namespace Entities.CodeFirst
{

    public partial class Currency : BaseEntity
    {
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Unit { get; set; }

    }
}
