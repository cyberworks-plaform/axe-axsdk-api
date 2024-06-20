using System.Threading;
using System.Threading.Tasks;

namespace AXService.Services.Interfaces
{
    public interface IWorker
    {
        Task DoWork(CancellationToken cancellationToken);
    }
}
