using AXAPIWrapper.Middleware;
using AXService.Config;
using AXService.Services.Implementations;
using AXService.Services.Interfaces;
using Azure.Storage.Blobs;
using Ce.Interaction.Lib.StartupExtensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;
using System;

namespace BGServiceAX
{
    public static class AppInfo
    {
        public const string Version = "v2.3.2"; //Todo: cần câp biến này đồng bộ với giá trị trong file csproj
        public static string LastUpdated => "2025-06-21";
    }
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(configuration)
                    .CreateLogger();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Config BaseHttpClient to call other service api
            services.AddBaseHttpClient();
            services.AddHttpClient<IAmzS3FileClient, AmzS3FileClient>();   //Transient, don't Inject to Scope or Singleton
            services.AddSingleton<IAmzFileClientFactory, AmzFileClientFactory>();
            services.AddScoped<ISimpleUserService, SimpleUserService>();
            services.AddScoped<IAutoInsuranceSerivce, AutoInsuranceSerivce>();
            services.AddScoped<IInternalOcrSerivce, InternalOcrSerivce>();
            services.AddSingleton(x => new BlobServiceClient(Configuration["AzureBlob:ConnectionString"]));
            services.DependencyInjectionService();
            services.AddControllers();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = $"AXSDK OpenAPI",
                    Version = AppInfo.Version,
                    Description = $"Last update: {AppInfo.LastUpdated} - {AppInfo.Version}",
                    Contact = new OpenApiContact
                    {
                        Name = "Huy Dinh",
                        Email = "huy.dinhquang@cybereye.vn"
                    }
                });
                options.AddSecurityDefinition("basic", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "basic",
                    In = ParameterLocation.Header,
                    Description = "Basic Authorization header using user/pass"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "basic"
                                }
                            },
                            new string[] {}
                    }
                });

            });
            services.Configure<RateLimitOptions>(Configuration.GetSection("RateLimitOptions"));

            //        services.AddAuthentication("BasicAuthentication")
            //.AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            //app.UseAuthorization();
            
            app.UseMiddleware<RateLimitingMiddleware>();

            app.UseSwagger();
            app.UseSwaggerUI(s =>
            {
                s.SwaggerEndpoint("/swagger/v1/swagger.json", "AX Api v1.1");
                s.RoutePrefix = string.Empty;
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
