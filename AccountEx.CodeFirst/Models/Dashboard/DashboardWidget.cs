using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.CodeFirst.Models
{
   public class DashboardWidget
    {
        [Key, Column(Order = 0)]
        public int DashboardId { get; set; }
        [Key, Column(Order = 1)]
        public int WidgetId { get; set; }
        public int SequenceNo { get; set; }
    }
}
