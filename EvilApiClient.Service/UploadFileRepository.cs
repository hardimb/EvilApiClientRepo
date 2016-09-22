using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvilApiClient.Core.Domain;
using EvilApiClient.Core.Repository;
using EvilApiClient.Core.Common;
using System.IO;
using System.Web;
using System.Configuration;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNet.SignalR;
using EvilApiClient.Service.Hubs;

namespace EvilApiClient.Service
{
    /// <summary>
    /// Repository service for file upload and file process operation
    /// </summary>
    public class UploadFileRepository : IUploadFileRepository
    {
        public ConfigData config { get; set; }
        public EvilUploadRepository evilUploadRepository { get; set; }


        
        public UploadFileRepository(ConfigData configData, EvilUploadRepository evilUploadRepository)
        {
            this.config = configData;
            this.evilUploadRepository = evilUploadRepository;
        }


        /// <summary>
        /// This method is used to save file to server
        /// </summary>
        /// <param name="uploadfile"></param>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        public FileUplodResponse SaveFile(FileUpload uploadfile,string folderPath)
        {
            bool isSuccess = false;
            string fileName = string.Empty;
            if (uploadfile.UploadFileStream != null)
            {
                if (!System.IO.Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                fileName = DateTime.Now.Ticks + System.IO.Path.GetExtension(uploadfile.FileName);
                string filePath = Path.Combine(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["UploadFilePath"]), fileName);
                isSuccess = FileHelper.SaveFile(filePath, uploadfile.UploadFileStream);

            }

            string message = "";
            if (isSuccess)
                message = "File Uploaded SuccessFully";
            else
                message = "File Upload Failed";

            return new FileUplodResponse() {
                Filename = fileName,
                FileUploadFailed = !isSuccess,
                FileuploadMessage = message
            };
                     
        }


        /// <summary>
        /// This method will read csv file and initate task of processing file parallely
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="fileName"></param>
        /// <param name="originalFileName"></param>
        /// <param name="filePath"></param>
        /// <param name="authIdentity"></param>
        public void ProcessFileData(string userName,string fileName, string originalFileName, string filePath, string authIdentity)
        {
            var taskProcessParameter = new List<TaskProcessParam>();
            using (var reader = System.IO.File.OpenText(Path.Combine(filePath, fileName)))
            {
                using (var csvReader = new CsvReader(reader, new CsvConfiguration { HasHeaderRecord = false }))
                {
                    while (csvReader.Read())
                    {
                        var customer = string.Empty;
                        var customerExists = csvReader.CurrentRecord.Length >= 1 && csvReader.TryGetField(0, out customer);

                        var customervalue = 0;
                        var customervalueExists = csvReader.CurrentRecord.Length >= 2 && csvReader.TryGetField(1, out customervalue);

                        if (customerExists && customervalueExists)
                        {
                            // Process customer data and notify result

                            taskProcessParameter.Add(new TaskProcessParam
                            {
                                UserName = userName,
                                Customer = customer,
                                Value = customervalue,
                                FileName = originalFileName,
                                AuthIdentity = authIdentity
                            });
                        }
                        else
                        {
                            var response = new EvilUploadCustomerResponse
                            {
                                Customer = customer,
                                Value = customervalue,
                                Errors = new[] { "No data exists" }
                            };

                            // notify result for any error
                            NotifyResult(originalFileName, response, authIdentity);
                        }
                    }
                }
            }

            Task.Run(() => Parallel.ForEach(taskProcessParameter, param => ProcessAndNotifyResult(param)));
        }

        /// <summary>
        /// This method is used to upload customer and check status of upload asynchronously as well as it notify user about result of processing of data
        /// </summary>
        /// <param name="param"></param>
        private async void ProcessAndNotifyResult(TaskProcessParam param)
        {
            try
            {
                // #1 Upload customer data
                EvilUploadCustomerRequest evilCustRequest = new EvilUploadCustomerRequest();

                evilCustRequest.Action = config.UploadAction;
                evilCustRequest.Customer = param.Customer;
                evilCustRequest.File = param.FileName;
                evilCustRequest.Property = param.UserName;
                evilCustRequest.Value = param.Value;

                var uploadRes = await evilUploadRepository.UploadCustomerData(evilCustRequest).ConfigureAwait(false);

                // if somehow we dont get the hash value then render response accordingly
                if (string.IsNullOrWhiteSpace(uploadRes.Hash))
                {
                    NotifyResult(param.FileName, uploadRes, param.AuthIdentity);
                }
                else
                {
                    // #2 We got response and now its time to invoke check customer upload
                    var checkUploadResponse = await evilUploadRepository.GetCustomer(uploadRes.Hash).ConfigureAwait(false);

                    // Now, lets check if hash exists
                    if (string.IsNullOrWhiteSpace(checkUploadResponse.Hash))
                    {
                        uploadRes.Added = false;
                        uploadRes.Errors = checkUploadResponse.Errors;
                    }

                    // #3 Notify web about response
                    NotifyResult(param.FileName, uploadRes, param.AuthIdentity);
                }
            
            

                
            }
            catch (Exception ex)
            {
                var response = new EvilUploadCustomerResponse
                {
                    Customer = param.Customer,
                    Value = param.Value,
                    Errors = new[] { ex.Message } // Ideally, it should be some meaningful error, or it should not on the first place!!
                };

                // notify error result 
                NotifyResult(param.FileName, response, param.AuthIdentity);
            }
        }



        /// <summary>
        /// This method is notify user about result of upload data
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="response"></param>
        /// <param name="authIdentity"></param>
        private void NotifyResult(string fileName, EvilUploadCustomerResponse response, string authIdentity)
        {
            var hubContenxt = GlobalHost.ConnectionManager.GetHubContext<FileUpladResultHub>();
            hubContenxt.Clients.User(authIdentity).NotifyResult(fileName, response);
        }

    }
}
