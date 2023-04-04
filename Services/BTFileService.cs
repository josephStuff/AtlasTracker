﻿using AtlasTracker.Services.Interfaces;

namespace AtlasTracker.Services
{
    public class BTFileService : IBTFileService
    {
        
        private readonly string[] suffixes = { "Bytes", "KB", "MB", "GB", "TB", "PB" };
                

        #region Convert Byte Array to File
        public string ConvertByteArrayToFile(byte[] fileData, string extension)
        {
            
                string imageBase64Data = Convert.ToBase64String(fileData);
                return string.Format($"data:{extension};base64,{imageBase64Data}");
            
        }

        #endregion

        #region Convert File to Byte Array
        public async Task<byte[]> ConvertFileToByteArrayAsync(IFormFile file)
        {
            
                MemoryStream memoryStream = new();
                await file.CopyToAsync(memoryStream);
                byte[] byteFile = memoryStream.ToArray();
                memoryStream.Close();
                memoryStream.Dispose();

                return byteFile;

        }

        #endregion

        #region Format File Size
        public string FormatFileSize(long bytes)
        {
            int counter = 0;
            decimal number = bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number /= 1024;
                counter++;
            }
            return string.Format("{0:n1}{1}", number, suffixes[counter]);

        }

        #endregion

        #region Get File Icon
        public string GetFileIcon(string file)
        {
            string ext = Path.GetExtension(file).Replace(".", "");
            return $"/img/png/{ext}.png";
        }

        #endregion    
    }
}
