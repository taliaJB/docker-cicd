using Eldan.DiagnosticSiteLib.CarsDiagnostic;
using Eldan.Logger;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Eldan.DiagnosticSite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateEdiDataController : ControllerBase
    {
        private LoggerNet _logger;
        private CarsDiagnosticBL _BL;

        public UpdateEdiDataController(IConfiguration? config)
        {
            new CarsDiagnostic().Initilize(config, ref _logger, ref _BL);
        }

        [HttpPost(Name = "UpdateEdiData")]
        public void Post(CarsDiagnosticReference.EdiDiagnosticData ediDiagnosticData)
        {
            _logger.DoAction(_BL.UpdateEdiData, ediDiagnosticData);
        }
    }
}
