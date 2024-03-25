using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccountEx.CodeFirst.Models
{
    [Table("LogData")]
    public partial class LogData : BaseEntity
    {


        public int SystemLogId { get; set; }
        public string Data { get; set; }
      
    }
}
