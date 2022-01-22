using System;
using ADCore.ApiReader.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ADCore.ApiReader.Extensions
{
   public class ApiClientFactory
   {
       private readonly IServiceProvider _serviceProvider;

       public ApiClientFactory(IServiceProvider serviceProvider)
       {
           _serviceProvider = serviceProvider;
       }

       public ApiClient Create()
       {
           return _serviceProvider.GetRequiredService<ApiClient>();
       }
   }
}
