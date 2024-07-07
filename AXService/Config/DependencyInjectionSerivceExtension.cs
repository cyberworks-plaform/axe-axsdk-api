using AXService.Services.Implementations;
using AXService.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace AXService.Config
{
    public static class DependencyInjectionSerivceExtension
    {
        public static void DependencyInjectionService(this IServiceCollection services)
        {
            //Worker with scoper to DI orther service
            services.AddScoped<IWorker, Worker>();

            services.AddSingleton<IBlobService, BlobService>();

            services.AddScoped<IInternalOcrSerivce, InternalOcrSerivce>();
            services.AddScoped<ITableSegmentationService, TableSegmentationService>();
            services.AddScoped<IExternalOcrService, ExternalOcrService>();
            services.AddScoped<ICyberWorkService, CyberWorkService>();
            services.AddScoped<IProcessRequestService, ProcessRequestService>();
        }
    }
}
