using System.Collections.Generic;

namespace AccountEx.Common
{
    public class FileUploadResponse
    {
        public List<Files> Files { get; set; }
        public List<Files> files = new List<Files>();

        public FileUploadResponse()
        {
            List<Files> Files = new List<Files>();
            List<Files> files = new List<Files>();
        }

    }

    public class Files
    {


        public string Name { get; set; }
        public string ThumbnailUrl { get; set; }

        public string Size { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
        public string DeleteUrl { get; set; }
        public string DeleteType { get; set; }

    }

}
