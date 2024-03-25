using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;
using AccountEx.Common;
using System.Web.Http;

namespace AccountEx.Web.Handlers
{
    /// <summary>
    /// Summary description for FileUpload
    /// </summary>
    public class PoUpload : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            if (SiteContext.Current.User == null)
            {
                var response = new ApiResponse() { Success = true, Data = "Session has expired", };
                throw new HttpResponseException(System.Net.HttpStatusCode.TemporaryRedirect);
            }
            else
            {
                if (context.Request.Files.Count == 0)
                {
                    context.Response.ContentType = "text/plain";
                    context.Response.Write("No files received.");
                }
                else
                {
                    var extensions = new Dictionary<string, string>();
                    extensions.Add(".jpg", "Image");
                    extensions.Add(".png", "Image");
                    extensions.Add(".bmp", "Image");
                    extensions.Add(".gif", "Image");
                    extensions.Add(".mp3", "Audio");
                    extensions.Add(".wav", "Audio");
                    extensions.Add(".mp4", "Video");
                    extensions.Add(".flv", "Video");
                    extensions.Add(".pdf", "PDF");
                    extensions.Add(".txt", "Text UploadDocument");
                    extensions.Add(".doc", "Word UploadDocument");
                    extensions.Add(".docx", "Word UploadDocument");
                    extensions.Add(".xls", "Excel UploadDocument");
                    extensions.Add(".xlsx", "Excel UploadDocument");
                    var uploadedfile = context.Request.Files[0];

                    var url = new Random().Next() + uploadedfile.FileName;
                    var ext = Path.GetExtension(uploadedfile.FileName) + "";
                    ext.ToLower();
                    var fileName = Path.GetFileNameWithoutExtension(uploadedfile.FileName);
                    if (fileName.Length > 35)
                    {
                        fileName = fileName.Substring(0, 35);
                    }
                    var fileType = uploadedfile.ContentType;
                    var fileSize = uploadedfile.ContentLength;
                    var rootPath = context.Session["rootpath"] + "";
                    if (!rootPath.EndsWith("\\")) rootPath += "\\";
                    var dirPath = rootPath + "ProjectFiles";
                    if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
                    uploadedfile.SaveAs(dirPath + "\\" + url);
                    context.Response.ContentType = "text/plain";

                    //context.Response.Write("{\"name\":\"" + FileName + "\",\"ext\":\"" + ext + "\",\"url\":\"" + URL
                    //    + "\",\"type\":\"" + type + "\",\"size\":\"" + ToFileSize(FileSize) + "\"}");
                    var data = new Files()
                    {
                        Name = fileName,
                        Size = ToFileSize(fileSize),
                        Url = url,
                        ThumbnailUrl = url,
                        DeleteUrl = "test",
                        DeleteType = "DELETE"

                    };
                    var filelist = new List<Files>();
                    var uploadResponse = new FileUploadResponse();
                    filelist.Add(data);
                    uploadResponse.Files = filelist;
                    context.Response.Write(new JavaScriptSerializer().Serialize(uploadResponse));

                }
            }

        }
        public string ToFileSize(long source)
        {
            const int byteConversion = 1024;
            var bytes = Convert.ToDouble(source);

            if (bytes >= Math.Pow(byteConversion, 3)) //GB Range
            {
                return string.Concat(Math.Round(bytes / Math.Pow(byteConversion, 3), 2), " GB");
            }
            else if (bytes >= Math.Pow(byteConversion, 2)) //MB Range
            {
                return string.Concat(Math.Round(bytes / Math.Pow(byteConversion, 2), 2), " MB");
            }
            else if (bytes >= byteConversion) //KB Range
            {
                return string.Concat(Math.Round(bytes / byteConversion, 2), " KB");
            }
            else //Bytes
            {
                return string.Concat(bytes, " Bytes");
            }
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