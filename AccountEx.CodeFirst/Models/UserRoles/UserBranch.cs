using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccountEx.CodeFirst.Models
{
    [Table("UserBranches")]
    public partial class UserBranch : BaseEntity
    {
        public int UserId { get; set; }
        public int BranchId { get; set; }




    }
}
