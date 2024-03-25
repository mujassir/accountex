using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Common
{
    /// <summary>
    /// DataModel class used in NexusSyncApiController to receive Post request
    /// </summary>
    public class NexusSyncRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string TableName { get; set; }
        public string Json { get; set; }
    }
}
