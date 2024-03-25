
namespace AccountEx.CodeFirst.Models
{
    using Common;
    using System;
    using System.Collections.Generic;

    public class UserToken : BaseEntity
    {

        public string Token { get; set; }
        public TokenType Type { get; set; }
        public int UserId { get; set; }
        public DateTime? ExpireOn { get; set; }

    }
}
