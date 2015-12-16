using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace MSMQService
{
    [ServiceContract]
    public interface IService1
    {
        [OperationContract(IsOneWay = true)]
        void SendLockMessage(string msg, DateTime sendDate);

        [OperationContract(IsOneWay = true)]
        void SendToOutputWindow(string msg, DateTime sendDate);
    }
}
