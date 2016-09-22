using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using EvilApiClient.Web.Models.Home;
using EvilApiClient.Core.Repository;
using EvilApiClient.Core.Domain;
using EvilApiClient.Core.Common;
using System.Security.Claims;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;
using System.IO;

namespace EvilApiClient.Web.Controllers
{

    /// <summary>
    /// Home controller
    /// </summary>
    public class HomeController : Controller
    {
        private readonly IUploadFileRepository uploadFileService;

        
        public HomeController(IUploadFileRepository uploadFileService)
        {            
            this.uploadFileService = uploadFileService;
        }

        public ActionResult Index()
        {
            // Authentican code will be to pass user identity to signalR and signalR will notify to that user.
            var authenticationManager = HttpContext.GetOwinContext().Authentication;

            if (authenticationManager != null)
            {
                if (authenticationManager.User == null || authenticationManager.User.Identity == null ||
                    string.Compare(Session.SessionID, authenticationManager.User.Identity.Name, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    var claims = new List<Claim> { new Claim(ClaimTypes.Name, Session.SessionID), new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) };
                    var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
                    authenticationManager.SignIn(new AuthenticationProperties { IsPersistent = false }, identity);

                    Session["Authenticated"] = true;
                }
            }

            var viewModel = new UploadFileViewModel();
            return View(viewModel);
        }

        /// <summary>
        /// This action method will upload file and return server saved file name to process further.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]        
        public ActionResult UploadFilePost(UploadFileViewModel viewModel)
        {
            UploadFile(viewModel);
            return Json(new { FileName = viewModel.UploadfileName, UserName = viewModel.UserName, OriginalFileName = viewModel.UploadFile.FileName}, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// This method is used to upload file
        /// </summary>
        /// <param name="viewModel"></param>
        private void UploadFile(UploadFileViewModel viewModel)
        {
            FileUpload file = new FileUpload()
            {
                FileName = viewModel.UploadFile.FileName,
                UploadFileStream = viewModel.UploadFile.InputStream
            };

            var response = uploadFileService.SaveFile(file, Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["UploadFilePath"]));
            viewModel.UploadFileMessage = response.FileuploadMessage;
            viewModel.UploadFailed = response.FileUploadFailed;
            viewModel.UploadfileName = response.Filename;

        }




        /// <summary>
        /// This method will take file name as input and start processing of file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="username"></param>
        /// <param name="originalFileName"></param>
        /// <returns></returns>
        [HttpGet]        
        public int StartProcessingFile(string fileName,string username, string originalFileName)
        {
            var fileInProcess= Session["fileInProcess"] as List<string>;

            if (fileInProcess == null)
            {
                fileInProcess = new List<string>();
                Session["fileInProcess"] = fileInProcess;
            }

            if (!fileInProcess.Contains(fileName))
            {                
               uploadFileService.ProcessFileData(username, fileName, originalFileName, Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["UploadFilePath"]),HttpContext.GetOwinContext().Authentication.User.Identity.Name);                
                fileInProcess.Add(fileName);

                return 1;
            }

            return 2;
        }


    }
}