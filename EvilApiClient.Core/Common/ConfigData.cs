using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvilApiClient.Core.Common
{
    /// <summary>
    /// This class is used to pass static information which is saved in app.config section
    /// </summary>
    public class ConfigData
    {
        public string ApiUrl { get; set; }
        public string UploadUrl { get; set; }
        public string CheckUrl { get; set; }
        public string UploadAction { get; set; }
    }
}
