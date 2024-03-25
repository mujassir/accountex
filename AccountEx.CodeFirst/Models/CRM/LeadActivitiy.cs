using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.CodeFirst.Models
{
    public partial class LeadActivity : BaseEntity
    {
        public string Subject { get; set; }
        public string Location { get; set; }
        public Nullable<System.DateTime> FromDate { get; set; }
        public Nullable<System.DateTime> ToDate { get; set; }
        public string Priority { get; set; }
        public Nullable<System.DateTime> DueDate { get; set; }
        public int LeadId { get; set; }
        public string Owner { get; set; }
        public byte ActivityType { get; set; }
        public string CallType { get; set; }
        public string CallPurpose { get; set; }
        public string Status { get; set; }
        public string ContactName { get; set; }
        public string CallResult { get; set; }
        public string Description { get; set; }
        public string RelatedTo { get; set; }

        public string MeetingStatus { get; set; }
        public string InterestLevel { get; set; }
        public string FollowUpStatus { get; set; }
        public Nullable<System.DateTime> NextFollowUp { get; set; }
        public string ExpectedMaturityLevel { get; set; }
    }
}
