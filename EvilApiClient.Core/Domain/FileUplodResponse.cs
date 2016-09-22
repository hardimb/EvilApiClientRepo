using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvilApiClient.Core.Domain
{
    /// <summary>
    /// This class represents file upload response
    /// </summary>
    public class FileUplodResponse
    {

        public string Filename { get; set; }
        public bool FileUploadFailed { get; set; }
        public string FileuploadMessage { get; set; }


    }
}
