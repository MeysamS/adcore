
using System.Net.Http;
using ADCore.ApiReader.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ADCore.ApiReader.Extensions
{
    public static class AddApiClientExtensions
    {
        public static void AddApiClient(this IServiceCollection services)
        {
            services.AddHttpClient<IApiClient, ApiClient>();
            //services.AddHttpClient<IApiClient, ApiClient>(client =>
            //{

            //}).ConfigurePrimaryHttpMessageHandler(() =>
            //{
            //    var handler = new HttpClientHandler
            //    {
            //        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
            //    };
            //    return handler;
            //});
            services.AddSingleton<ApiClientFactory>();
        }
    }
}
