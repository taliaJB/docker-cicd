using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Eldan.DiagnosticServicesLib;
using Eldan.DiagnosticServicesLib.CarsDiagnostic;

[ServiceContract]
public interface ICarsDiagnosticService
{

    [OperationContract]
    void UpdateSuppliersData();

    [OperationContract]
    void UpdateSupplierData(List<int> carsNumber);

    [OperationContract(IsOneWay = true)]
    void UpdateEdiData(EdiDiagnosticData ediDiagnosticData, string LoggerSessionID);

    [OperationContract(IsOneWay = true)]
    void UpdatePointerData(PointerDiagnosticData pointerDiagnosticData, string LoggerSessionID);

    [OperationContract(IsOneWay = true)]
    void UpdateIturanData(IturanDiagnosticData ituranDiagnosticData, string LoggerSessionID);

    [OperationContract(IsOneWay = true)]
    void UpdateInetData(InetDiagnosticData inetDiagnosticData, string LoggerSessionID);

    [OperationContract]
    CarData GetCarData(int carNumber);

    [OperationContract]
    void CalibrateSupplierCar(int carNumber, int KM);
}



