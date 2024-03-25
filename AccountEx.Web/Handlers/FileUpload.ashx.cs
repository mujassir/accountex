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
    public class FileUpload : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {

            try
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
                        var extensions = new Dictionary<string, string>
                        {
                            {".jpg", "Image"},
                            {".png", "Image"},
                            {".bmp", "Image"},
                            {".gif", "Image"},
                            {".mp3", "Audio"},
                            {".wav", "Audio"},
                            {".mp4", "Video"},
                            {".flv", "Video"},
                            {".pdf", "PDF"},
                            {".txt", "Text UploadDocument"},
                            {".doc", "Word UploadDocument"},
                            {".docx", "Word UploadDocument"},
                            {".xls", "Excel UploadDocument"},
                            {".xlsx", "Excel UploadDocument"}
                        };
                        var uploadedfile = context.Request.Files[0];

                        var url = new Random().Next(1, 1000000) + "_" + DateTime.Now.ToString("yyMMddHHmmssff") + "_" + uploadedfile.FileName;
                        var ext = Path.GetExtension(uploadedfile.FileName) + "";
                        ext = ext.ToLower();
                        if (extensions.ContainsKey(ext))
                            ;
                        else
                            ;
                        var fileName = Path.GetFileNameWithoutExtension(uploadedfile.FileName);
                        if (fileName.Length > 35)
                        {
                            fileName = fileName.Substring(0, 35);
                        }
                        var fileSize = uploadedfile.ContentLength;
                        var rootPath = context.Session["rootpath"] + "";
                        if (!rootPath.EndsWith("\\")) rootPath += "\\";

                        var uploadPath = rootPath + "Upload";
                        if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

                        if (!uploadPath.EndsWith("\\")) uploadPath += "\\";
                        var companyPath = uploadPath + SiteContext.Current.UploadFolder;
                        if (!Directory.Exists(companyPath)) Directory.CreateDirectory(companyPath);

                        var dirPath = rootPath + "Upload/" + SiteContext.Current.UploadFolder + "/" + context.Request["directory"];
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
                        uploadResponse.files = uploadResponse.Files = filelist;
                        context.Response.Write(new JavaScriptSerializer().Serialize(uploadResponse));

                    }
                }
            }
            catch (Exception ex)
            {
                context.Response.Write(ex.Message);

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