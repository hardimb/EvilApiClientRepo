using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace EvilApiClient.Core.Domain
{
    /// <summary>
    /// This class has been used to manage requested upload
    /// </summary>
    public class FileUpload
    {
        public Stream UploadFileStream { get; set; } 

        public string FileName { get; set; }
    }
}
