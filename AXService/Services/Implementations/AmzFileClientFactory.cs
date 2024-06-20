using AXService.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace AXService.Services.Implementations
{
    public class AmzFileClientFactory:IAmzFileClientFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public AmzFileClientFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IAmzS3FileClient Create()
        {
            return _serviceProvider.GetRequiredService<IAmzS3FileClient>();
        }
    }
}
