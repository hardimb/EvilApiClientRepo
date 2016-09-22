using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvilApiClient.Core.Domain;
using EvilApiClient.Core.Common;

namespace EvilApiClient.Core.Repository
{
    /// <summary>
    /// Repository interface for Evil Api upload or check upload process
    /// </summary>
    public interface IEvilAPIRepository
    {
        Task<EvilUploadCustomerResponse> UploadCustomerData(EvilUploadCustomerRequest customerRequest);

        Task<EvilCheckCustomerResponse> GetCustomer(string hash);
       
    }
}
