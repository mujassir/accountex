using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.CodeFirst.Models.Transactions
{
    public class BankReceipt : BaseEntity
    {
        public BankReceipt()
        {
            this.BankReceiptItems = new HashSet<BankReceiptItem>();
        }

        public int VoucherNumber { get; set; }
        public DateTime PaymentDate { get; set; }
        public string FBR { get; set; }
        public int FiscalId { get; set; }

        public virtual ICollection<BankReceiptItem> BankReceiptItems { get; set; }
    }
}
