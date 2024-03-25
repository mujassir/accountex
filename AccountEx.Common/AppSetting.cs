namespace AccountEx.Common
{
    public static class AppSetting
    {
        public static int AccountLevel { get { return 4; } set { } }
        public static string DateFormat { get { return "dd/MM/yyyy"; } set { } }
        public static string GridDateFormat { get { return "dd/MM/yy"; } set { } }
        public static string GridDateFormat1 { get { return "ddd, MMM d, yyyy"; } set { } }
        public static string GridTimeFormat { get { return "hh:mm tt"; } set { } }
        public static int CRMProductNameLength { get { return 25; } set { } }

    }
}
