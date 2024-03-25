using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.BussinessLogic.Security
{
    public static class RsaCrypto
    {
        public static string PrivateKey = "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTE2Ij8+DQo8UlNBUGFyYW1ldGVycyB4bWxuczp4c2k9Imh0dHA6Ly93d3cudzMub3JnLzIwMDEvWE1MU2NoZW1hLWluc3RhbmNlIiB4bWxuczp4c2Q9Imh0dHA6Ly93d3cudzMub3JnLzIwMDEvWE1MU2NoZW1hIj4NCiAgPEV4cG9uZW50PkFRQUI8L0V4cG9uZW50Pg0KICA8TW9kdWx1cz50MTI1NERhUzUxenZ5WkVHK1p3SG9ieEJHQ0xUODBlbEJMQnlSVlkzRW1ySXV0S3daZFNoTkVhclRyUWdEUk1sTXlMNTB2QmRuWlpUQ0laOVg1NC9HejRWRUo1cEZGU0xTa1JabXpnYVVTVStjM2dDYjRQangvN1VaN1FnSk0wUDQyeFppUVhJSmNQdjZIVjR0VHFoSUVLbU5LakhLaER1a3BCSVlJUEIyVG1qc1R4WXFORVRrNzArUFR2emFQYnRvaDZTTmdIWGFUdkRndkxxWGpTLzFxLzhKQUx2K215cWVLdHpmeDJrb2hscWFsTmliWGE5T2hXUEZLYTJlbHorODU0ZWdOc1VFVmNMaGw5Vk9mYXdmUEEzL1dMYUZEdEJrUWNEVmF1U3hHYjI4VHR3Szh0ZVUxMjF0MU5TaTBzcFpkU2tLa09ReHNqdkc3WkZQakpiRXc9PTwvTW9kdWx1cz4NCiAgPFA+enFob0VRaE50bUpJcEZkT29MK2FudUYyakV1bjJLNHJsc3FkWkttay9RM1JOODNSTzVtRnJhTGhlZUZFakVEY3NHeStVdHVtbXVqMmpzanVLVTZLNlJybEgwYXJUam40ZHB1VUd2b2JObENyckhuMVlPc29tSHlWVE1VMG1NUzZhUEhSaEs4akJsZ21JWk9SRkpCWDZQNEd4SEtmbFlUMmFEQkd1akNzRzIwPTwvUD4NCiAgPFE+NHlXbmpYckZHVXd5L3crMEJQYmtaSk9kRS9YUDRMazFlVS9pejZXSElsM0lsMzlRdXpzbHd4TXNaK2VGd0oxcVdQaXRCU3gvYU9YQVFTcE85bmQwNjZMYmR0bXNWSXV2OERXeHBKUFdaRWsxVnh1T0dCY1RyekZ6eCtFeWlDdVEvUTJZZnlvZnRQTmEyc285MjhzcW1MZFM5RkJLSTkrSzZkUkQzZ2Jid0g4PTwvUT4NCiAgPERQPk5kbFNES1lpUWRVSVlySlJUYmUvbllqWWNDUkNuMFo3R3phK25uMUovWnA5R3RKcG8rTVVIek9qRnpRb3ZYY2xTbDk3bEhHaHp4bkhMVnVVRzNWWTR2YmNIek0xN2hOUzkrMEFRYXRNTGZJRldkTUpxbkI4U29la0M3WVRSMzl6cDJiamxuUmdad1hQSWM2bTdqblNmbWE2OHJOL2UrR0NwNUYwekkxa08xMD08L0RQPg0KICA8RFE+RXdaSFYwa3VFdVZYN0Jiem0rclZ6RHEwMEVRcFNnZ1pHM1QrYVNmUy9xVGVOUm9idCtMSFVlUmNOc09rWnpLSUFyV3BRQ0t1OVNreEJnUTJJdHgwZFh2cFo3WHBnaGRGQUJodkxYcVlYVVBvYk1oSkNXbDlZNUgvd1lEeUM0ZWltQjg0WTRZeXE2UXFPdUx2ZFdZNXhocXNudnZPaWR5aU5OMXJaY0FyVFo4PTwvRFE+DQogIDxJbnZlcnNlUT5rczdIMW1wMFdZRXIyNzExdTdBMUFjdGxoUElhTlFzYXdDSGo4Nmc0RG05SitiVmNxTXZWOE41aVpkWWk2aGhhUmw5alNCR3lENVY0REI2VGZXbUkwVjVkSHlYdU1MTHVXSmVxTytBK1Vrblk3a3NLQXVOQXdQTnlhRktxdlJEMHFQbEliYXJYSFdBTXFUYUpzTkl4UGx0NHVIZlMySy8vSFpETU9wVmY1Njg9PC9JbnZlcnNlUT4NCiAgPEQ+QnNiMFJrKzdtSTM4UWpxTDZ6Z25WeUpJQlZLVTFENHpHN2N2QmJpUnpVZ1N5T1JKLzY1dHpWSWlHOU81MzRrNUUwc24zNC9FSDlzRWFZMjI5M2pwSTJ3OXdrMW9qVXd4dU5LLzVNaUl2cUJzN0dZOU1ERGZwdVRuUTNSc3lZOXJBdjM3OXdlc3JUNHhFVDI2a0NqUDQrUGo5d2RkblJoVkhMSE5qL0NSTC9zWmIvd2hTd05kb1Z2SlRvL3p5YjUzZkVTenJXaW5sbi8xQ3ZUL1dEcHd0QWIrb3ZYbGk4M3FGSzR2bVRZNm1HdGtpWUtVTExVUjhHcGROeWl5Y1ZKd2Z5M3dGTkwxb2kzcnV0SnNrRWxLbWVCV09BNXo1KzhQdzFOMXBoY0NyUmdGbEVSUGZlMGZFRGZkc3M1TVV3a3UxbVBWNHI5UDJ3QWc1MlB5dGJ3aHlRPT08L0Q+DQo8L1JTQVBhcmFtZXRlcnM+";
        public static string PublicKey = "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTE2Ij8+DQo8UlNBUGFyYW1ldGVycyB4bWxuczp4c2k9Imh0dHA6Ly93d3cudzMub3JnLzIwMDEvWE1MU2NoZW1hLWluc3RhbmNlIiB4bWxuczp4c2Q9Imh0dHA6Ly93d3cudzMub3JnLzIwMDEvWE1MU2NoZW1hIj4NCiAgPEV4cG9uZW50PkFRQUI8L0V4cG9uZW50Pg0KICA8TW9kdWx1cz50MTI1NERhUzUxenZ5WkVHK1p3SG9ieEJHQ0xUODBlbEJMQnlSVlkzRW1ySXV0S3daZFNoTkVhclRyUWdEUk1sTXlMNTB2QmRuWlpUQ0laOVg1NC9HejRWRUo1cEZGU0xTa1JabXpnYVVTVStjM2dDYjRQangvN1VaN1FnSk0wUDQyeFppUVhJSmNQdjZIVjR0VHFoSUVLbU5LakhLaER1a3BCSVlJUEIyVG1qc1R4WXFORVRrNzArUFR2emFQYnRvaDZTTmdIWGFUdkRndkxxWGpTLzFxLzhKQUx2K215cWVLdHpmeDJrb2hscWFsTmliWGE5T2hXUEZLYTJlbHorODU0ZWdOc1VFVmNMaGw5Vk9mYXdmUEEzL1dMYUZEdEJrUWNEVmF1U3hHYjI4VHR3Szh0ZVUxMjF0MU5TaTBzcFpkU2tLa09ReHNqdkc3WkZQakpiRXc9PTwvTW9kdWx1cz4NCjwvUlNBUGFyYW1ldGVycz4=";


        public static void MakeKey()
        {
            //lets take a new CSP with a new 2048 bit rsa key pair
            var csp = new RSACryptoServiceProvider(2048);

            //how to get the private key
            var privKey = csp.ExportParameters(true);

            //and the public key ...
            var pubKey = csp.ExportParameters(false);

            //converting the public key into a string representation
            string pubKeyString;
            {
                //we need some buffer
                var sw = new System.IO.StringWriter();
                //we need a serializer
                var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
                //serialize the key into the stream
                xs.Serialize(sw, pubKey);
                //get the string from the stream
                pubKeyString = sw.ToString();
                pubKeyString = Base64Encode(pubKeyString);
            }
            string privateKeyString;
            {
                //we need some buffer
                var sw = new System.IO.StringWriter();
                //we need a serializer
                var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
                //serialize the key into the stream
                xs.Serialize(sw, privKey);
                //get the string from the stream
                privateKeyString = sw.ToString();
                privateKeyString = Base64Encode(privateKeyString);
            }


        }

        private static RSAParameters GetKey(string key)
        {
            //get a stream from the string
            var pubKeyString = Base64Decode(key);
            var sr = new System.IO.StringReader(pubKeyString);
            //we need a deserializer
            var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
            //get the object back from the stream
            return (RSAParameters)xs.Deserialize(sr);
        }
        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        private static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        public static string Encrypt(object plainText)
        {

            try
            {


                //lets take a new CSP with a new 2048 bit rsa key pair
                var csp = new RSACryptoServiceProvider(2048);
                csp.ImportParameters(GetKey(PublicKey));
                //we need some data to encrypt
                var plainTextData = plainText.ToString();

                //for encryption, always handle bytes...
                var bytesPlainTextData = System.Text.Encoding.Unicode.GetBytes(plainTextData);

                //apply pkcs#1.5 padding and encrypt our data 
                var bytesCypherText = csp.Encrypt(bytesPlainTextData, false);

                //we might want a string representation of our cypher text... base64 will do
                var cypherText = Convert.ToBase64String(bytesCypherText);
                return cypherText;
            }
            catch (Exception)
            {

                return "";
            }

        }
        public static string Decrypt(string cypherText)
        {
            try
            {


                var bytesCypherText = Convert.FromBase64String(cypherText);

                //we want to decrypt, therefore we need a csp and load our private key
                var csp = new RSACryptoServiceProvider(2048);
                csp.ImportParameters(GetKey(PrivateKey));

                //decrypt and strip pkcs#1.5 padding
                var bytesPlainTextData = csp.Decrypt(bytesCypherText, false);

                //get our original plainText back...
                var plainTextData = System.Text.Encoding.Unicode.GetString(bytesPlainTextData);
                return plainTextData;
            }
            catch (Exception)
            {

                return "";
            }
        }
    }
}
