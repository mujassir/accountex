using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace AccountEx.CodeFirst.Models
{
    public class vw_PendingDeliveryChallan
    {
        public int Id { get; set; }
        public int VoucherNumber { get; set; }
        public Nullable<int> InvoiceNumber { get; set; }
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public System.DateTime Date { get; set; }
        public Nullable<int> OrderNumber { get; set; }
        public byte Status { get; set; }
        public byte InvoiceTransactionType { get; set; }
        public int DCQuatityTotal { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public int FiscalId { get; set; }
        public bool IsDeleted { get; set; }
        public  Nullable<int> CompanyPartnerId { get; set; }

    }
}
