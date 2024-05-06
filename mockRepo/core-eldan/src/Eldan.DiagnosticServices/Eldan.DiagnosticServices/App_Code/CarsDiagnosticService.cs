using Eldan.DiagnosticServicesLib.CarsDiagnostic;
using Eldan.LoggerFrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Eldan.ServicesSchedulerLib;

// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service" in code, svc and config file together.
public class CarsDiagnosticService : ICarsDiagnosticService, IScheduler
{
    private LoggerFrm _logger;
    private CarsDiagnosticBL _BL;

    public void UpdateEdiData(EdiDiagnosticData ediDiagnosticData, string LoggerSessionID)
    {
        SetLogger();
        _logger.LoggerSessionID = LoggerSessionID;
        _logger.DoAction(_BL.UpdateEdiData, ediDiagnosticData);
    }

    public void UpdatePointerData(PointerDiagnosticData pointerDiagnosticData, string LoggerSessionID)
    {
        SetLogger();
        _logger.LoggerSessionID = LoggerSessionID;
        _logger.DoAction(_BL.UpdatePointerData, pointerDiagnosticData);
    }

    public void UpdateIturanData(IturanDiagnosticData ituranDiagnosticData, string LoggerSessionID)
    {
        SetLogger();
        _logger.LoggerSessionID = LoggerSessionID;
        _logger.DoAction(_BL.UpdateIturanData, ituranDiagnosticData);
    }

    public void UpdateInetData(InetDiagnosticData inetDiagnosticData, string LoggerSessionID)
    {
        SetLogger();
        _logger.LoggerSessionID = LoggerSessionID;
        _logger.DoAction(_BL.UpdateInetData, inetDiagnosticData);
    }

    public void UpdateSupplierData(List<int> carsNumber)
    {
        SetLogger();
        _logger.DoAction(_BL.UpdateSupplierData, carsNumber);
    }

    public void UpdateSuppliersData()
    {
        SetLogger(CarsDiagnosticBL.LOGGER_FILE_PATH_SCHEDUALE_KEY);
        _logger.DoAction(_BL.UpdateSuppliersData);
    }

    public CarData GetCarData(int carNumber)
    {
        SetLogger();
        return _logger.DoFunc(_BL.GetCarData, carNumber);
    }

    public void CalibrateSupplierCar(int carNumber, int KM)
    {
        SetLogger();
        _logger.DoAction(_BL.CalibrateSupplierCar, carNumber, KM);
    }

    public void Flush()
    {
        throw new NotImplementedException();
    }

    public void Refresh(string taskId)
    {
        UpdateSuppliersData();
    }

    private void SetLogger()
    {
        SetLogger(CarsDiagnosticBL.LOGGER_FILE_PATH_ONLINE_KEY);
    }

    private void SetLogger(string key)
    {
        _logger = new LoggerFrm(CarsDiagnosticBL.SERVICE_NAME);
        _logger.LoggerFilePathKey = key;
        _logger.LogCreateInstance = false;
        _logger.BindEvents(new CarsDiagnosticBLEvents());
        _BL = _logger.CreateInstance<CarsDiagnosticBL>();
    }
}
