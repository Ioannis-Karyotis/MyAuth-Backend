using Microsoft.Extensions.DependencyInjection;
using MyAuth.Utils.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyAuth.Utils.Extentions
{
    public static class MemoryCacheExtensions
    {
        public static IServiceCollection AddMemoryHandlerService<TKeys>(this IServiceCollection services) where TKeys : Enum
        {
            services.AddSingleton<MemoryCacheHandler<TKeys>>();
            services.AddSingleton<MemoryCacheCustomKeysHandler>();
            return services;
        }
    }
}
