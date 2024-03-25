using System;
using System.IO;
using System.Security.Cryptography;
using System.Web.Mvc;

namespace AccountEx.Web.Code
{
   

    public class ETagAttribute : ActionFilterAttribute
    {
        private string GetToken(Stream stream)
        {

            MD5 md5 = MD5.Create();
            byte[] checksum = md5.ComputeHash(stream);
            return Convert.ToBase64String(checksum, 0, checksum.Length);
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            filterContext.HttpContext.Response.AppendHeader("ETag", GetToken(filterContext.HttpContext.Response.OutputStream));
            base.OnResultExecuted(filterContext);
        }
    }
}