using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace WcfLogInService
{
    public interface LoLi_ServiceCallback
    {

        [OperationContract(IsOneWay = true)]
        void Notify(string value);

        [OperationContract(IsOneWay = true)]
        void NotifyUserJoinedTheConversation(string userName);

        [OperationContract(IsOneWay = true)]
        void NotifyUserOfMessage(string userName, String userMessage);

        [OperationContract(IsOneWay = true)]
        void NotifyUserLeftTheConversation(string userName);

      //  [OperationContract(IsOneWay = true)]
      //  void OnDataReceived(string data);

    }
}
