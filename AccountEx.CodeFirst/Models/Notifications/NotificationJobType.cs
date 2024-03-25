using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.CodeFirst.Models
{
    public partial class NotificationJobType : GlobalBaseEntity 
    {
       
        public string Name { get; set; }
        public string Description { get; set; }

     
    }
}
