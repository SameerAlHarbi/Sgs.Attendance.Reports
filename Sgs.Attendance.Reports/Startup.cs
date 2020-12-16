using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using Sameer.Shared.Data;
using Sameer.Shared.Mvc;
using Sgs.Attendance.Reports.Data;
using Sgs.Attendance.Reports.Helpers;
using Sgs.Attendance.Reports.Services;
using System;
using System.Globalization;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Sgs.Attendance.Reports
{
    public class Startup
    {
        private IConfiguration _config { get; }
        private IHostingEnvironment _env;

        public Startup(IConfiguration configuration
            , IHostingEnvironment environment)
        {
            _config = configuration;
            _env = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSameerDbDataManagers<AttendanceReportsDb>(_config);

            services.AddHttpClient<IErpManager, ErpManager>(client =>
            {
                client.BaseAddress = new System.Uri(@"http://172.16.11.44:810/HrPortalApi/api/Hr/portal/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
            });

            services.AddAutoMapper();

            services.AddHostedService<TimedHostedService>();
            services.AddScoped<IScopedProcessingService, ScopedProcessingService>();

            services.AddSingleton<IClaimsTransformation, ClaimsTransformer>();

            services.AddAuthentication(IISDefaults.AuthenticationScheme);

            services.AddMvc()
                .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_2)
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

            services.AddKendo();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error/500");
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
            }

            app.UseStaticFiles();

            app.UseNodeModules(env.ContentRootPath);


            app.UseAuthentication();

            app.UseMvcWithDefaultRoute();
        }
    }
}