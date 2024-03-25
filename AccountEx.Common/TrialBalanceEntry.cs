namespace AccountEx.Common
{
   public class TrialBalanceEntry
   {
       public int AccountId { get; set; }
       public string Code { get; set; }
       public string AccountTitle { get; set; }
       public decimal? Debit { get; set; }
       public decimal? Credit { get; set; }
       public decimal OpeningBalance { get; set; }
       public decimal Balance { get; set; }

    }
}
