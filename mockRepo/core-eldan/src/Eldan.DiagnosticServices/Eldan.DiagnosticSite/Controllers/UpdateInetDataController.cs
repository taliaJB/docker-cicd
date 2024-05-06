using Eldan.DiagnosticSiteLib.CarsDiagnostic;
using Eldan.Logger;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Eldan.DiagnosticSite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateInetDataController : ControllerBase
    {
        private LoggerNet _logger;
        private CarsDiagnosticBL _BL;

        public UpdateInetDataController(IConfiguration? config)
        {
            new CarsDiagnostic().Initilize(config, ref _logger, ref _BL);
        }

        [HttpPost(Name = "UpdateInetData")]
        public void Post(CarsDiagnosticReference.InetDiagnosticData inetDiagnosticData)
        {
            _logger.DoAction(_BL.UpdateInetData, inetDiagnosticData);
        }
    }
}
