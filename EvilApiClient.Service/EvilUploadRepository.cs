using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvilApiClient.Core.Domain;
using EvilApiClient.Core.Repository;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using EvilApiClient.Core.Common;

namespace EvilApiClient.Service
{
    /// <summary>
    /// Repository service for Evil Api upload or check upload process
    /// </summary>
    public class EvilUploadRepository : IEvilAPIRepository
    {
        private ConfigData configData;
        public EvilUploadRepository(ConfigData objConfigData)
        {
            configData = objConfigData;
        }

        /// <summary>
        /// This method is used to upload customer data to 3rd party API
        /// </summary>
        /// <param name="customerRequest"></param>
        /// <returns></returns>
        public async Task<EvilUploadCustomerResponse> UploadCustomerData(EvilUploadCustomerRequest customerRequest)
        {
            using (var client = GetHttpClient(configData.ApiUrl))
            {              
                var content = new StringContent(JsonConvert.SerializeObject(customerRequest), Encoding.UTF8, "application/json");

                var response = await client.PostAsync(configData.UploadUrl, content).ConfigureAwait(false);

                var responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                var result = JsonConvert.DeserializeObject<EvilUploadCustomerResponse>(responseText);
                result.Customer = customerRequest.Customer;
                result.Value = customerRequest.Value;

                return result;
            }
        }

        /// <summary>
        /// This method is used to check status of upload
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public async Task<EvilCheckCustomerResponse> GetCustomer(string hash)
        {
            using (var client = GetHttpClient(configData.ApiUrl))
            {
                var response = await client.GetAsync(string.Format("{0}?hash={1}", configData.CheckUrl, hash)).ConfigureAwait(false);

                var responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                var result = JsonConvert.DeserializeObject<EvilCheckCustomerResponse>(responseText);

                return result;
            }
        }

        /// <summary>
        /// This is generic method to get reference of HTTP client
        /// </summary>
        /// <param name="apiUrl"></param>
        /// <returns></returns>
        private HttpClient GetHttpClient(string apiUrl)
        {
            var client = new HttpClient { BaseAddress = new Uri(apiUrl) };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }
    }
}
