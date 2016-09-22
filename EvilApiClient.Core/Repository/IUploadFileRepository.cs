using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvilApiClient.Core.Domain;
using System.IO;

namespace EvilApiClient.Core.Repository
{
    /// <summary>
    /// Repository interface for file upload and file process operation
    /// </summary>
    public interface IUploadFileRepository
    {
        FileUplodResponse SaveFile(FileUpload uploadfile, string folderPath);

        void ProcessFileData(string userName, string fileName, string originalFileName, string filePath, string authIdentity);
    }
}
