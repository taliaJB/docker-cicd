using Eldan.DiagnosticSiteLib.CarsDiagnostic;
using Eldan.Logger;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Eldan.DiagnosticSite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdatePointerDataController : ControllerBase
    {
        private LoggerNet _logger;
        private CarsDiagnosticBL _BL;

        public UpdatePointerDataController(IConfiguration? config)
        {
            new CarsDiagnostic().Initilize(config, ref _logger, ref _BL);
        }

        [HttpPost(Name = "UpdatePointerData")]
        public void Post(CarsDiagnosticReference.PointerDiagnosticData pointerDiagnosticData)
        {
            _logger.DoAction(_BL.UpdatePointerData, pointerDiagnosticData);
        }
    }
}
