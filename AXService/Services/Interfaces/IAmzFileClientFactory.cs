using AXService.Services.Implementations;
using System;
using System.Collections.Generic;
using System.Text;

namespace AXService.Services.Interfaces
{
    public interface IAmzFileClientFactory
    {
        IAmzS3FileClient Create();
    }
}
