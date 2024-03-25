using System;
using System.Collections.Generic;

namespace AccountEx.CodeFirst.Models
{
    public partial class SystemLog : BaseEntity
    {

        public SystemLog()
        {
            LogData = new HashSet<LogData>();
          
        }

        public string TableName { get; set; }
        public Nullable<int> DocumentId { get; set; }
        public string DocumentCode { get; set; }
        public string DocumentType { get; set; }
        public Nullable<int> DocumentNo { get; set; }
        public string DocumentYear { get; set; }
        public string Action { get; set; }
        public string UserName { get; set; }
        public string ComputerIp { get; set; }
        public string ComputerUser { get; set; }
        public string ComputerName { get; set; }
        public string Description { get; set; }
        public int ModuleType { get; set; }
        public virtual ICollection<LogData> LogData { get; set; }

    }
}
