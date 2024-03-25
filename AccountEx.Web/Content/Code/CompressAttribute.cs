using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AccountEx.Web.Code
{
    public class CompressAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            var encodingsAccepted = filterContext.HttpContext.Request.Headers["Accept-Encoding"];
            if (string.IsNullOrEmpty(encodingsAccepted)) return;

            encodingsAccepted = encodingsAccepted.ToLowerInvariant();
            var response = filterContext.HttpContext.Response;

            if (encodingsAccepted.Contains("deflate"))
            {
                if (!response.IsRequestBeingRedirected)
                {
                    response.AppendHeader("Content-encoding", "deflate");
                    response.Filter = new DeflateStream(response.Filter, CompressionMode.Compress);
                }
            }
            else if (encodingsAccepted.Contains("gzip"))
            {
                if (!response.IsRequestBeingRedirected)
                {
                    response.AppendHeader("Content-encoding", "gzip");
                    response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
                }
            }
        }
    }

}