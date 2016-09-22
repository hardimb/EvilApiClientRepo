using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvilApiClient.Core.Domain
{
    /// <summary>
    /// This class is used to save the data which required to be parsed 
    /// </summary>
    public class TaskProcessParam
    {
        public string UserName { get; set; }
        public string Customer { get; set; }
        public int Value { get; set; }
        public string FileName { get; set; }
        public string AuthIdentity { get; set; }
    }
}
