using Eldan.LoggerBase;
using Eldan.TypeExtensions;
using Microsoft.Extensions.Configuration;

namespace Eldan.DiagnosticSiteLib.CarsDiagnostic
{
    public class CarsDiagnosticBL : IServiceLoggerBL
    {
        public const string LOGGER_FILE_PATH_KEY = "loggerFilePathCarsDiagnostic";

        ServiceLoggerBase _logger;
        public ServiceLoggerBase ServiceLogger { set => _logger = value; }

        private IConfiguration? _config;

        public IConfiguration? Config
        {
            get { return _config; }
            set { _config = value; }
        }

        public void UpdateEdiData(CarsDiagnosticReference.EdiDiagnosticData ediDiagnosticData)
        {
            _logger.Write("UpdateEdiData");

            using (CarsDiagnosticReference.CarsDiagnosticServiceClient proxy = GetProxy())
            {
                proxy.UpdateEdiData(ediDiagnosticData, _logger.LoggerSessionID);
            }
        }

        public void UpdatePointerData(CarsDiagnosticReference.PointerDiagnosticData pointerDiagnosticData)
        {
            _logger.Write("UpdatePointerData");

            using (CarsDiagnosticReference.CarsDiagnosticServiceClient proxy = GetProxy())
            {
                proxy.UpdatePointerData(pointerDiagnosticData, _logger.LoggerSessionID);
            }
        }

        public void UpdateIturanData(CarsDiagnosticReference.IturanDiagnosticData ituranDiagnosticData)
        {
            _logger.Write("UpdateIturanData");

            using (CarsDiagnosticReference.CarsDiagnosticServiceClient proxy = GetProxy())
            {
                proxy.UpdateIturanData(ituranDiagnosticData, _logger.LoggerSessionID);
            }
        }

        public void UpdateInetData(CarsDiagnosticReference.InetDiagnosticData inetDiagnosticData)
        {
            _logger.Write("UpdateInetData");

            using (CarsDiagnosticReference.CarsDiagnosticServiceClient proxy = GetProxy())
            {
                proxy.UpdateInetData(inetDiagnosticData, _logger.LoggerSessionID);
            }
        }

        private CarsDiagnosticReference.CarsDiagnosticServiceClient GetProxy()
        {
            CarsDiagnosticReference.CarsDiagnosticServiceClient proxy = new CarsDiagnosticReference.CarsDiagnosticServiceClient();
            proxy.Endpoint.Address = new System.ServiceModel.EndpointAddress(_config["CarsDiagnosticServiceAddress"]);
            return proxy;
        }
    }
}
