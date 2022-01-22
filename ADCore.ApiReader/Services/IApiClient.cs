using System.Threading.Tasks;
using ADCore.ApiReader.Models;

namespace ADCore.ApiReader.Services
{
    public interface IApiClient 
    {
        Task<ResponseModel> GetAsync(RequestModel model, Proxy proxy=null);
    }
}
