using System;
using System.Collections.Generic;

namespace AccountEx.CodeFirst.Models
{
   public class InvoiceClearing : BaseEntity
    {
       public int InvoiceId { get; set; }
       public int AccountId { get; set; }
       public int FiscalId { get; set; }
       public int InvoiceNo { get; set; }
       public decimal Amount { get; set; }
       public Nullable<DateTime> Date { get; set; }

    }
}
