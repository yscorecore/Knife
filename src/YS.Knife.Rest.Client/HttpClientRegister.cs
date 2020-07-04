﻿using System;
using Microsoft.Extensions.DependencyInjection;

namespace YS.Knife.Rest.Client
{
    public class HttpClientRegister : IServiceRegister
    {
        public void RegisteServices(IServiceCollection services, IRegisteContext context)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));
            var apiServiceOptions = context.Configuration.GetConfigOrNew<ApiServicesOptions>();
            // add default
            services.AddHttpClient();
            foreach (var kv in apiServiceOptions.Services)
            {
                var baseAddress = kv.Value.BaseAddress;
                services.AddHttpClient(kv.Key, (client) =>
                {
                    if (apiServiceOptions.Timeout > 0)
                    {
                        client.Timeout = TimeSpan.FromSeconds(apiServiceOptions.Timeout);
                    }
                    client.BaseAddress = new Uri(baseAddress);
                });
            }
        }
    }
}
