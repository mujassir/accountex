using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.CodeFirst.Models.CRM
{
   public partial class vw_GetNextFollowUps
    {
        public int Id { get; set; }
        public string LeadOwner { get; set; }
        public string Company { get; set; }
        public int NoOfMeetings { get; set; }
        public string Mobile { get; set; }
        public Nullable<System.DateTime> NextFollowUp { get; set; }
        public int CompanyId { get; set; }
        public bool IsDeleted { get; set; }
    }
}
