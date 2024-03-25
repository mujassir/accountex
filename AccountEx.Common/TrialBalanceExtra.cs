namespace AccountEx.Common
{
    /// <summary>
    /// Can contain object in value
    /// </summary>
    public class TrialBalanceExtra
    {
        public TrialBalanceExtra() { }
        public TrialBalanceExtra(TrialBalanceExtra p)
        {
            MAINACCOUNTID = p.MAINACCOUNTID;
            CONTROLACCOUNTID = p.CONTROLACCOUNTID;
            SUBACCOUNTID = p.SUBACCOUNTID;
            ACCOUNTID = p.ACCOUNTID;

            MAINACCOUNT = p.MAINACCOUNT;
            if (p.CONTROLACCOUNT == null)
                CONTROLACCOUNT = string.Empty;
            else
                p.CONTROLACCOUNT = CONTROLACCOUNT;

            if (p.SUBACCOUNT == null)
                SUBACCOUNT = string.Empty;
            else
                p.SUBACCOUNT = SUBACCOUNT;

            if (p.ACCOUNT == null)
                ACCOUNT = string.Empty;
            else
                p.ACCOUNT = ACCOUNT;

            //if (p.SUBACCOUNT == null)
            //    ACCOUNT = string.Empty;
            //else
            //    p.CONTROLACCOUNT = CONTROLACCOUNT;


            PARENTID = p.PARENTID ?? 0;
            LEVEL = p.LEVEL;
         
              YTDBALANCE = p.YTDBALANCE ?? 0;
              DEBIT = p.DEBIT ?? 0;
              CREDIT = p.CREDIT ?? 0;
              BALANCE = p.BALANCE ?? 0;

        }

        public int MAINACCOUNTID { get; set; }
        public int? CONTROLACCOUNTID { get; set; }
        public int? SUBACCOUNTID { get; set; }
        public int? ACCOUNTID { get; set; }

        public string MAINACCOUNT { get; set; }
        public string CONTROLACCOUNT { get; set; }
        public string SUBACCOUNT { get; set; }
        public string ACCOUNT { get; set; }

        public int? PARENTID { get; set; }
        public int LEVEL { get; set; }

        public decimal? YTDBALANCE { get; set; }
        public decimal? DEBIT { get; set; }
        public decimal? CREDIT { get; set; }
        public decimal? BALANCE { get; set; }

    }
}
