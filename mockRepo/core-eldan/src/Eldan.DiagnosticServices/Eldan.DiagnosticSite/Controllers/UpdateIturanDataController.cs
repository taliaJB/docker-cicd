using Eldan.DiagnosticSiteLib.CarsDiagnostic;
using Eldan.Logger;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Eldan.DiagnosticSite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateIturanDataController : ControllerBase
    {
        private LoggerNet _logger;
        private CarsDiagnosticBL _BL;

        public UpdateIturanDataController(IConfiguration? config)
        {
            new CarsDiagnostic().Initilize(config, ref _logger, ref _BL);
        }

        [HttpPost(Name = "UpdateIturanData")]
        public void Post(CarsDiagnosticReference.IturanDiagnosticData ituranDiagnosticData)
        {
            _logger.DoAction(_BL.UpdateIturanData, ituranDiagnosticData);
        }
    }
}
