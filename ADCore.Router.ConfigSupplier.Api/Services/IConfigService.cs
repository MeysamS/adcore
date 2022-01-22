
using ADCore.Router.ConfigSupplier.Api.Models;
using System.Threading.Tasks;

namespace ADCore.Router.ConfigSupplier.Api.Services
{
    public interface IConfigService
    {
        Task<Response> ReadRouterConfigAsync(string id);
     }
}
