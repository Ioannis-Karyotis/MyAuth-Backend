using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyAuth.Models.Data
{
    public class TxtFile
    {
        public string Path { get; set; }
        public string FileName { get; set; }
        public string Ext { get; set; }
        public string Url { get; set; }
        public string ConvertToBase64()
        {
            string ext = Ext.Trim(new Char[] { '.' });
            //string[] name = FileName.Split('.');
            var paths = new string[] { Path, FileName };
            string finalPath = System.IO.Path.Combine(paths);
            byte[] imageByteData = System.IO.File.ReadAllBytes(finalPath);
            string imageBase64Data = Convert.ToBase64String(imageByteData);
            string imageDataURL = string.Format("data:file/plain;base64,{0}", imageBase64Data);
            return imageDataURL;
        }

        public TxtFile(string path, string fileName, string ext, string url)
        {
            Path = path;
            FileName = fileName;
            Ext = ext;
            Url = url;
        }
        public TxtFile(string path, string fileName, string ext, string url, string thumbFileName, string thumbUrl)
        {
            Path = path;
            FileName = fileName;
            Ext = ext;
            Url = url;
        }

        public TxtFile()
        {
        }
    }
}
