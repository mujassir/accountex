namespace AccountEx.Common
{
    public class AccountSummary
    {

        public string Name { get; set; }
        public decimal Balance { get; set; }
    }

    public class AccountSummary1
    {

        public string Name { get; set; }
        public int Id { get; set; }
        public int ParentId { get; set; }
        public decimal Balance { get; set; }
    }
}


