using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountEx.Common
{
    public static class Encrypt
    {
        public static string Encode(string phrase)
        {
            try
            {
                byte[] encbuff = System.Text.Encoding.UTF8.GetBytes(phrase);
                string s = Convert.ToBase64String(encbuff);
                char[] ar = s.ToCharArray();
                Array.Reverse(ar);
                string er = new string(ar);
              
                return er;
            }
            catch (Exception)
            {
                return "";
            }
        }
        public static string Reverse(string phrase)
        {
            try
            {
                char[] ar = phrase.ToCharArray();
                Array.Reverse(ar);
                string er = new string(ar);

                return er;
            }
            catch (Exception)
            {
                return "";
            }
        }
        public static string Decode(string phrase)
        {

            try
            {
                char[] ar = phrase.ToCharArray();
                Array.Reverse(ar);
                phrase = new string(ar);
              
                byte[] decbuff = Convert.FromBase64String(phrase);
                return System.Text.Encoding.UTF8.GetString(decbuff);
            }
            catch (Exception)
            {
                return "";
            }
        }
        public static string Base64Decode(string phrase)
        {

            try
            {
                char[] ar = phrase.ToCharArray();
               // Array.Reverse(ar);
                phrase = new string(ar);

                byte[] decbuff = Convert.FromBase64String(phrase);
                return System.Text.Encoding.UTF8.GetString(decbuff);
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}