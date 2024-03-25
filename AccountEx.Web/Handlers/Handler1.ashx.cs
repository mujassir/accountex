using System.Collections.Generic;
using System.Web;
using System.Web.Script.Serialization;
using AccountEx.Common;

namespace AccountEx.Web.Handlers
{
    /// <summary>
    /// Summary description for Handler1
    /// </summary>
    public class Handler1 : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            var filelist = new List<Files>();
            var uploadResponse = new FileUploadResponse();
            //filelist.Add(data);
            uploadResponse.Files = filelist;
            context.Response.Write(new JavaScriptSerializer().Serialize(uploadResponse));
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}