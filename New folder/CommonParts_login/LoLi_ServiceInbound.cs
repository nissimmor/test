
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace WcfLogInService
{
    [ServiceContract(
     Name = "LoLi_MessageService",
      Namespace = "WcfLogInService",
        CallbackContract = typeof(LoLi_ServiceCallback),
      SessionMode = SessionMode.Required)]
    public interface LoLi_ServiceInbound
    {

        [OperationContract]
        string AgentLoLi(string agentID, string computerName, string extNumber, string userID, string stat);

        [OperationContract]
        string DoWork(string value);
        [OperationContract]
        bool isAlive();
        [OperationContract]
        bool SetComment(int recID, string Comments, string userID);
        [OperationContract]
        bool SetStatus(int recID, int recStatus, int recRes, string userID);
        [OperationContract]
        bool SetNextAttempt(int recID, DateTime NextDate, int priority, string userID);
        [OperationContract]
        bool SetDCRecord(int recID, string phoneNum, DateTime startRecord, int CallStatus, int tryID, int recStatus, int recRes, string userID);
        [OperationContract]
        bool SetCampByCode(Dictionary<int, int[]> _CampByCode);
        [OperationContract]
        string[] ReceiveCampInfo(string userName, int campID);
        [OperationContract]
        bool SetCodeInfo(Dictionary<int, string[]> _CodeInfo);
        [OperationContract]
        bool SetStartCall(int RecordID, int TryID, string PhoneNumbr, DateTime CallDate, string agentID);
        [OperationContract]
        bool SetEndCall(int RecordID, int TryID, DateTime endDate, string User);
        [OperationContract]
        bool SetEndCallNext(int RecordID, int TryID, DateTime endDate, int CallStatus, DateTime nextAttemptDate, int priority, string User);
        [OperationContract]
        bool SetReturntoQueue(int RecId, int TryID, string agentID);
        [OperationContract]
        bool SetStatusNew(int RecordID, int TryID, int callStatus, DateTime nextAttemptDate, int priority, string User);
        [OperationContract]
        bool SetskillInfoByCamp(Dictionary<int, Dictionary<int, int>> dicskillInfoByCamp, Dictionary<int, Array> dicCampaignsInfo);
       [OperationContract]
        bool SetCampaignsList(Dictionary<int, Array> dicCampaignsInfo);
        [OperationContract]
        string[] GetLoggedAgents();
        [OperationContract]
        bool SetAgentState(string AgentLoginID, DateTime StateTime, string State, string OldState, string StateDetails, string Phone, string Skill, string Application, string CallID, string extDN, string agtName);
        [OperationContract]
        bool is_progressive(string AgentLoginID);
        [OperationContract]
        bool SetSysParm(Dictionary<int, Array> dicSysParm);
        [OperationContract]
        Dictionary<int, string> ReceiveADName(string userName, int isBlending);
        [OperationContract]
        bool setRecRls(int recID, string userID);
        [OperationContract]
        bool setCloseSP(int recID, string userID);
        [OperationContract]
        Dictionary<int, string[]> ReceiveCampCode(int RecId, int campID, string userName);
        [OperationContract]
        Dictionary<int, string[]> ReceiveCallCode(int CampID);
        [OperationContract]
        string[] ReceiveSysParmSP();
        [OperationContract]
        Dictionary<int, string[]> ReceivedefCode(int RecId, int campID, string userName);
        [OperationContract]
        string[] ReceiveRcordData(int RecId, int campID, string userName);
        [OperationContract]
        System.Data.DataTable  ReceivePrivateRecords(string userName);
        [OperationContract]
        string[] ReceiveRecord(string userName, int isBlending);
        [OperationContract]
        string[] ReceiveFields(int RecId, int campID, string userName);
        [OperationContract]
        bool SetAgentHaveBlending(Dictionary<string, int> dicAgentHaveBlending);
        [OperationContract(IsOneWay = true)]
        void GetDataMapping(int value);
        [OperationContract(IsOneWay = true)]
        void GetData(string value);
        [OperationContract]
        bool isConnect(string value);
        //[OperationContract]
        //bool SetCampaignsByAgent(Dictionary<string, string> agentCampaignAssignment);
        //[OperationContract]
        //bool SetCfgParams(Dictionary<string, string> CfgParams);

        #region Softphone Members

        [OperationContract]
        bool JoinTheConversation(string userName);
        [OperationContract]
        string[] ReceiveUserInfo(string userName);
        [OperationContract(IsOneWay = true)]
        void ReceiveMessage(string userName, List<string> addressList, string userMessage);
        [OperationContract]
        int LeaveTheConversation(string userName);
        [OperationContract(IsOneWay = true)]
        void GetuserDetail(string loginID, string userName, string computerName, string lineNo, string posID);

        #endregion
    }
}
