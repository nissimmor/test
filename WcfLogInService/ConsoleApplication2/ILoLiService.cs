using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace WcfLogInService
{
    [ServiceContract(
        Name = "LoLiService",
        Namespace = "WcfLogInService",
        CallbackContract = typeof(ILoLiServiceCallback),
        SessionMode = SessionMode.Required)]
    public interface ILoLiService
    {
        [OperationContract]
        string GetData(string value);
        [OperationContract]
        string AgentLoLi(string agentID, string computerName, string extNumber, string userID, string stat);
        [OperationContract]
        string AgentFree(string agentID, string computerName, string extNumber, string userID, string stat);
        [OperationContract]
        string Mala_EWT_Proxy(string ApplicationName, string Ucid, string Ani, string Dnis, string TZ);
        [OperationContract]
        string Mala_InsertNewProxy(string Ani, string Dnis, string Ucid, string Phone, int CampaingCode, string CustID, string ApplicationName, string TZ, int ProxyStatus, string username);
        [OperationContract]
        string TB_CloseCB_mac(string phoneNumber, string TZ, string username);
        [OperationContract]
        string AgentDB(string computerName, string userID);        
        [OperationContract]
        string AddAgent(string agentID, string computerName, string extNumber, string userID, string stat);
        [OperationContract]
        bool addUserAD(string userID, int action);
        [OperationContract]
        string getOtherPhone(string userID);
        [OperationContract]
        int setOtherPhone(string userID, int isOtherPhone,string phone);
        [OperationContract]
        string getOutPhone(string userID);
        [OperationContract]
        int setOutPhone(string userID, int isOutDial, string bphone,string extension);


    }
}
