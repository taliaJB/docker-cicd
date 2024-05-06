using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eldan.DiagnosticServicesLib.CarsDiagnostic
{
    public abstract class CommonData
    {
        internal enum EnmErrorType
        {
            HTTPError = 1,
            SupplierError = 2,
        }

        internal bool IsSuccess { get; set; } = true;

        internal string ErrorMessage { get; set; }

        internal EnmErrorType? ErrorType { get; set; }

        internal void Copy(CommonData commonData)
        {
            IsSuccess = commonData.IsSuccess;
            ErrorMessage = commonData.ErrorMessage;
            ErrorType = commonData.ErrorType;
        }
    }
}
