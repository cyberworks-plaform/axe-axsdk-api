using System.Threading.Tasks;

namespace AXService.Services.Interfaces
{
    public interface ISimpleUserService
    {
        bool ValidUserPass(string user, string pass);
    }
}
