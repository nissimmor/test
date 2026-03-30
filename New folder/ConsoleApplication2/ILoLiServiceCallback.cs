using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WcfLogInService
{
    [ServiceContract(CallbackContract = typeof(ILoLiServiceCallback))]
    public interface ILoLiServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void OnDataReceived(string data);
    }

}
