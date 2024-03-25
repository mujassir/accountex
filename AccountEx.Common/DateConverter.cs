using System;

namespace AccountEx.Common
{
    public static class DateConverter
    {
       


        public static DateTime ConvertFromDmy(string date)
        {
            try
            {
                var provider = System.Globalization.CultureInfo.InvariantCulture;
                return DateTime.ParseExact(date, "dd/MM/yyyy", provider);
            }
            catch (Exception ex) 
            {
                return DateTime.MinValue;
            }
        }

        public static DateTime ConvertStringToDate(string s)
        {
            try
            {
                if (s == "") return DateTime.Now;
                return ConvertFromDmy(s);
            }
            catch (Exception)
            {
                return DateTime.MinValue;
            }
        }
        public static DateTime ConvertStandardDate(string s, DateTime defaultDate)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(s)) return defaultDate;
                return Convert.ToDateTime(s);
            }
            catch (Exception)
            {
                return defaultDate;
            }
        }
        public static DateTime ConvertStandardDate(string s)
        {
            return ConvertStandardDate(s, DateTime.MinValue);
        }
       


      
      
    }
}
