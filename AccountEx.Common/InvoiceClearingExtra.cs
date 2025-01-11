using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Common
{
   public class InvoiceClearingExtra
    {
       public int Id { get; set; }
       public int AccountId { get; set; }
       public int VoucherNumber { get; set; }
       public DateTime Date { get; set; }
       public byte TransactionType { get; set; }
       public decimal NetTotal { get; set; }
       public decimal TotalPaid { get; set; }
       public decimal Amount { get; set; }
    }
   public class SaleDetailForDetailedLedgerExtra
   {
       public int VoucherNumber { get; set; }
       public int InvoiceNumber { get; set; }
       public Nullable<byte> TransactionType { get; set; }
       public string ItemCode { get; set; }
       public string ItemName { get; set; }
       public decimal Quantity { get; set; }
       public decimal Rate { get; set; }
       public decimal GSTAmount { get; set; }
       public decimal Amount { get; set; }
       public decimal NetAmount { get; set; }
       public int AuthLocationId { get; set; }
   }
}
