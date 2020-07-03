﻿using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.FileProviders;

namespace YS.Knife.Hosting.Web
{
    [SuppressMessage("Performance", "CA1822:将成员标记为 static", Justification = "<挂起>")]
    public class DefaultStartup
    {

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                // disable auto validate model
                options.SuppressModelStateInvalidFilter = true;
            });

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            UseStaticFiles(app);

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
        private void UseStaticFiles(IApplicationBuilder app)
        {
            var knifeOptions = app.ApplicationServices.GetService<IOptions<KnifeWebOptions>>();

            foreach (var kv in knifeOptions.Value.StaticFiles ?? new Dictionary<string, StaticFileInfo>())
            {
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(kv.Value.FolderPath),
                    ServeUnknownFileTypes = kv.Value.ServeUnknownFileTypes,
                    DefaultContentType = kv.Value.DefaultContentType,
                    RequestPath = new Microsoft.AspNetCore.Http.PathString(kv.Key)
                });
            }
        }
    }
}
