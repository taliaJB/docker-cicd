using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace Eldan.ServicesSchedulerLib
{
    [ServiceContract]
    public interface IScheduler
    {
        [OperationContract]
        void Refresh(string taskId);

        [OperationContract]
        void Flush();
    }
}
