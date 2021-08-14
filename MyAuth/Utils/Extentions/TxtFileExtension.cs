using Microsoft.AspNetCore.Http;
using MyAuth.Models.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MyAuth.Utils.Extentions
{
    public static class TxtFileExtension
    {
        public const int ImageMinimumBytes = 0;
        public const int ImageMaximumBytes = 5000000;

        public static InternalDataTransfer<bool> IsTxtFile(this IFormFile postedFile)
        {
            //-------------------------------------------
            //  Check the image mime types
            //-------------------------------------------
            if (postedFile.ContentType.ToLower() != "text/plain")
            {
                return new InternalDataTransfer<bool>(false, "Content Type");
            }

            //-------------------------------------------
            //  Check the image extension
            //-------------------------------------------
            if (Path.GetExtension(postedFile.FileName).ToLower() != ".txt")
            {
                return new InternalDataTransfer<bool>(false, "Content Type Ext");
            }

            //-------------------------------------------
            //  Attempt to read the file and check the first bytes
            //-------------------------------------------
            try
            {
                if (!postedFile.OpenReadStream().CanRead)
                {
                    return new InternalDataTransfer<bool>(false, "Read Rights");
                }
                //------------------------------------------
                //check whether the image size exceeding the limit or not
                //------------------------------------------ 
                if (postedFile.Length < ImageMinimumBytes || postedFile.Length > ImageMaximumBytes)
                {
                    return new InternalDataTransfer<bool>(false, "Txt Size");
                }

                byte[] buffer = new byte[ImageMaximumBytes];
                postedFile.OpenReadStream().Read(buffer, 0, ImageMaximumBytes);
                string content = System.Text.Encoding.UTF8.GetString(buffer);
                if (Regex.IsMatch(content, @"<script|<html|<head|<title|<body|<pre|<table|<a\s+href|<img|<plaintext|<cross\-domain\-policy",
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline))
                {
                    return new InternalDataTransfer<bool>(false, "Regex");
                }
            }
            catch (Exception e)
            {
                new InternalDataTransfer<bool>(e);
            }

            //-------------------------------------------
            //  Try to instantiate new Bitmap, if .NET will throw exception
            //  we can assume that it's not a valid image
            //-------------------------------------------

            
            postedFile.OpenReadStream().Position = 0;
            

            return new InternalDataTransfer<bool>(true);
        }

        public static async Task<InternalDataTransfer<TxtFile>> TxtFileStore(this IFormFile file, string fileNameToSave, string encryptedFaceDescriptor, string specialFolder = null, string path = null, TxtFile toDeleteFile = null)
        {
            if (path == null)
                path = EnvVariablesRetriever.GetAppWebRootPath();
            var finalPath = (specialFolder == null) ? path : Path.Combine(path, specialFolder);
            try
            {
                if (!Directory.Exists(finalPath))
                {
                    Directory.CreateDirectory(finalPath);
                }

                var finalFineName = $"{fileNameToSave}{Path.GetExtension(file.FileName).ToLower()}";

                if (toDeleteFile != null)
                {
                    if (File.Exists(Path.Combine(finalPath, toDeleteFile.FileName)))
                    {
                        File.Delete(Path.Combine(finalPath, toDeleteFile.FileName));
                    }
                }
                using (var stream = System.IO.File.Create(Path.Combine(finalPath, finalFineName)))
                {
                    file.CopyTo(stream);
                }
                
                using (StreamWriter streamWriter = new StreamWriter(Path.Combine(finalPath, finalFineName), append: true ))
                {
                    await streamWriter.WriteLineAsync(encryptedFaceDescriptor);
                }

                return new InternalDataTransfer<TxtFile>(new TxtFile(finalPath, finalFineName, Path.GetExtension(file.FileName).ToLower(), $"/{(specialFolder != null ? $"{specialFolder}/{finalFineName}" : finalFineName)}"));

            }
            catch (Exception e)
            {
                return new InternalDataTransfer<TxtFile>(e);
            }

        }
    }
}
