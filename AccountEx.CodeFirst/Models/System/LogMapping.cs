using System;

namespace AccountEx.CodeFirst.Models
{
    public partial class LogMapping : BaseEntity
    {


        public string TableName { get; set; }
        public string LogKey { get; set; }
        public byte LogType { get; set; }
        public string Description { get; set; }
        public string ModuleKey { get; set; }
     

    }
}
