using AXService.Dtos;
using AXService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BGServiceAX.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class CyberWorkController : Controller
    {
        private readonly ICyberWorkService _cyberWorkService;
        public CyberWorkController(ICyberWorkService cyberWorkService)
        {
            _cyberWorkService = cyberWorkService;
        }

        //[HttpPost]
        //[Route("process-ocr-recognize")]
        //public async Task<IActionResult> Index(FileBase64Request request)
        //{
        //    var result = await _cyberWorkService.GetOcrRegion(request.Base64);
        //    return Content(result);
        //}
    }
}
