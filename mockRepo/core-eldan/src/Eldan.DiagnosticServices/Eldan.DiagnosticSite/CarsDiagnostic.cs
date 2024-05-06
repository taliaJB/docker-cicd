using Eldan.DiagnosticSiteLib.CarsDiagnostic;
using Eldan.Logger;
namespace Eldan.DiagnosticSite
{
    public class CarsDiagnostic
    {
        public void Initilize(IConfiguration? config, ref LoggerNet logger, ref CarsDiagnosticBL BL)
        {
            logger = new LoggerNet(config, "UpdateSupplierDataController");
            logger.LoggerFilePathKey = CarsDiagnosticBL.LOGGER_FILE_PATH_KEY;
            logger.LogCreateInstance = false;
            BL = logger.CreateInstance<CarsDiagnosticBL>();
            BL.Config = config;
        }
    }
}
