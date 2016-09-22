using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EvilApiClient.Web.Models.Home
{
    /// <summary>
    /// Model class for file upload functionality
    /// </summary>
    public class UploadFileViewModel
    {
        public string UploadFileMessage { get; set; }
        public string UploadfileName { get; set; }
        public bool UploadFailed { get; set; }
        [Required]
        public HttpPostedFileBase UploadFile { get; set; }
        [Required]
        public string UserName { get; set; }
    }
}