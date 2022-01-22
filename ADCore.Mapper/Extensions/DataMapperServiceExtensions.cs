using  ADCore.Mapper.Models;
using  ADCore.Mapper.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace  ADCore.Mapper.Extensions
{
    public static class DataMapperServiceExtensions
    {
        /// <summary>
        /// Add ADCoreMapper Service to ServiceCollection
        /// </summary>
        /// <param name="services">ADCoreMapper Service</param>
        public static void AddADCoreMapperService<T>(this IServiceCollection services)
        {
            services.AddTransient<IMapperService, MapperService>();
        }
        
    }

}
