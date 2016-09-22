using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace EvilApiClient.Core.Common
{
    /// <summary>
    /// This helper class is used to manage file operation like save file.
    /// </summary>
    public static class FileHelper
    {
        /// <summary>
        /// Save file to specified location
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static bool SaveFile(string filePath, Stream stream)
        {
            bool isSaved = true;
            try
            {

                using (FileStream fileStream = File.Create(filePath, (int)stream.Length))
                {
                    // Initialize the bytes array with the stream length and then fill it with data
                    byte[] bytesInStream = new byte[stream.Length];
                    stream.Read(bytesInStream, 0, bytesInStream.Length);
                    // Use write method to write to the file specified above
                    fileStream.Write(bytesInStream, 0, bytesInStream.Length);
                }
                  
                
            }
            catch (Exception ex)
            {
                isSaved = false;
            }

            return isSaved;
                     
        }
    }
}
