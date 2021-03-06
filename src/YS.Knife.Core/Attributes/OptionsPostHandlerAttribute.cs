﻿using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace YS.Knife
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class OptionsPostHandlerAttribute : KnifeAttribute
    {
        public OptionsPostHandlerAttribute() : base(typeof(IPostConfigureOptions<>))
        {

        }
        public override void RegisterService(IServiceCollection services, IRegisterContext context, Type declareType)
        {
            _ = declareType ?? throw new ArgumentNullException(nameof(declareType));
            var optionsType = FindOptionsType(declareType);
            services.AddSingleton(typeof(IPostConfigureOptions<>).MakeGenericType(optionsType), declareType);
        }
        private Type FindOptionsType(Type declareType)
        {
            return declareType.GetInterfaces()
                 .Where(p => p.IsGenericType && p.GetGenericTypeDefinition() == typeof(IPostConfigureOptions<>))
                 .Select(p => p.GetGenericArguments().First())
                 .FirstOrDefault();
        }
    }
}
