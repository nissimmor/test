using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Threading;
using System.Data;
using MCCMLogger;
using System.Configuration;

namespace WcfLogInService
{
    [ServiceBehavior(
            ConcurrencyMode = ConcurrencyMode.Single,
            InstanceContextMode = InstanceContextMode.PerCall)]
    public class LoLi_MessageService : LoLi_ServiceInbound
    {
        private static Mutex mut = new Mutex();
        private static List<LoLi_ServiceCallback> _callbackList = new List<LoLi_ServiceCallback>();
        private static Dictionary<string, LoLi_ServiceCallback> _userList = new Dictionary<string, LoLi_ServiceCallback>();
        private static Dictionary<int, Array> SysParm = new Dictionary<int, Array>();
        //private static Dictionary<string, string> CfgpPrams = new Dictionary<string, string;
        private static Dictionary<int, Array> CampaignsInfo = new Dictionary<int, Array>();
        private static Dictionary<int, int[]> campByCode = new Dictionary<int, int[]>();
        private static Dictionary<int, string[]> CodeInfo = new Dictionary<int, string[]>();
        private static Dictionary<int, Dictionary<int, string>> CampDataMap = new Dictionary<int, Dictionary<int, string>>();
        private static string[] StaticUserInfo  = null;
        private static object locker = new object();


        public static DBmanager dbManger = new DBmanager();

        // TO DELL::
        private static Dictionary<int, Dictionary<int, int>> skillInfoByCamp = new Dictionary<int, Dictionary<int, int>>();
        private static Dictionary<string, Dictionary<int, int>> AgentBySkill = new Dictionary<string, Dictionary<int, int>>();
        private static Dictionary<string, int> AgentHaveBlending = new Dictionary<string, int>();
        private static Dictionary<string, string> DicCampsByAgent = new Dictionary<string, string>();

        //  number of current users - 0 to begin with
        private static int _registeredUsers = 0;

        // Default Constructor
        public LoLi_MessageService()
        {
        }
        public string AgentLoLi(string agentID, string computerName, string extNumber, string userID, string stat)
        {
            try
            {
                string webuser = ConfigurationManager.AppSettings["webuser"];
                string webpass = ConfigurationManager.AppSettings["webpass"];
                string AACCIP = ConfigurationManager.AppSettings["AACCIP"];

                MccMLogger.Info(DateTime.Now.ToString());
                //AdressChg adressChg = new AdressChg("4321", "sip:68311@mac.org.il"); //4321
                //string agentID, string extNumber, string userID,string wepass,string webuser,string ipacc
                // -- > AdressChg adressChg = new AdressChg(agentID, extNumber,userID,webpass,webuser, AACCIP); //4321
                //adressChg.NewAdress();
                MccMLogger.Info(DateTime.Now.ToString());

                MccMLogger.Info("             *****************       exit             ***************** ");
                MccMLogger.Info(" ");

              //  LoLi_ServiceCallback callback = OperationContext.Current.GetCallbackChannel<LoLi_ServiceCallback>();
              //  callback.OnDataReceived(string.Format(" method: You entered: {0}", agentID));
                return string.Format(" method: You entered: {0}", agentID);

            }
            catch (Exception ex)
            {
                MccMLogger.Error("Failed to start CP service !!! \n\n", ex);

                Console.WriteLine("Press the Enter key to terminate service.");
                Console.ReadLine();
            }
            return agentID;
        }
        public string DoWork(string value)
        {
            return "Welcome " + value;
        }
        private string result;
        public void GetData(string value)
        {
            result = string.Format("You entered: {0}", value);

            Thread.Sleep(5000);
            OperationContext.Current.GetCallbackChannel<LoLi_ServiceCallback>().Notify(value);
        }
        public bool isConnect(string value)
        {
            string ooo = "";
            
           
            if (!dbManger.checkConnectivity(5000, "DialerDB", out ooo))
            {
                MccMLogger.Debug(string.Format("{0} error Not connected to DialerDB  .\n ",
                                  DateTime.Now.ToString("h:mm:ss.fff")));
                return false;
            }
            MccMLogger.Debug(string.Format("{0} connected to DialerDB  .\n ",
                                 DateTime.Now.ToString("h:mm:ss.fff")));
            return true;
        }
        public bool SetskillInfoByCamp(Dictionary<int, Dictionary<int, int>> dicskillInfoByCamp, Dictionary<int, Array> dicCampaignsInfo)
        {
            bool RT = false;
            try
            {
                //result = string.Format("You entered: {0}", skillInfoByCamp.Count.ToString());
                skillInfoByCamp = new Dictionary<int, Dictionary<int, int>>(dicskillInfoByCamp);
                CampaignsInfo = new Dictionary<int, Array>(dicCampaignsInfo);
                MccMLogger.Debug(string.Format("{0} skillInfoByCamp {1} CampaignsInfo {2}  .\n",
                                DateTime.Now.ToString("h:mm:ss.fff"), skillInfoByCamp.Count.ToString(), CampaignsInfo.Count.ToString()));
                RT = true;
            }
            catch (Exception ex)
            {
                MccMLogger.Debug(string.Format("{0} error  {1}   {2}   .\n",
                                                DateTime.Now.ToString("h:mm:ss.fff"), ex.Message, ex.Source));
                MccMLogger.Debug(string.Format("{0} error rskillInfoByCamp {1} CampaignsInfo {2}  .\n",
                                                DateTime.Now.ToString("h:mm:ss.fff"), skillInfoByCamp.Count.ToString(), CampaignsInfo.Count.ToString()));
            }
            return RT;
            //Thread.Sleep(5000);
            //OperationContext.Current.GetCallbackChannel<LoLi_ServiceCallback>().Notify(value);
        }
        public bool SetSysParm(Dictionary<int, Array> dicSysParm)
        {
            bool RT = false;
            try
            {
                SysParm = new Dictionary<int, Array>(dicSysParm);
                MccMLogger.Debug(string.Format("CP::SetSysParm recieves {0} params", SysParm.Count.ToString()));
                RT = true;
            }
            catch (Exception ex)
            {
                MccMLogger.Error("CP::SetSysParm", ex);
            }
            return RT;
        }
        public bool SetAgentBySkill(Dictionary<string, Dictionary<int, int>> dicAgentBySkill)
        {
            bool RT = false;
            try
            {
                //result = string.Format("You entered: {0}", dicAgentBySkill.Count.ToString());
                AgentBySkill = new Dictionary<string, Dictionary<int, int>>(dicAgentBySkill);
                //Thread.Sleep(5000);
                //OperationContext.Current.GetCallbackChannel<LoLi_ServiceCallback>().Notify(value);
                MccMLogger.Debug(string.Format("{0}  AgentBySkill {1} .\n",
                                                       DateTime.Now.ToString("h:mm:ss.fff"), AgentBySkill.Count.ToString()));
                RT = true;
            }
            catch (Exception ex)
            {
                MccMLogger.Debug(string.Format("{0} error  {1}   {2}   .\n",
                                                DateTime.Now.ToString("h:mm:ss.fff"), ex.Message, ex.Source));
                MccMLogger.Debug(string.Format("{0} error AgentBySkill {1} .\n",
                                                      DateTime.Now.ToString("h:mm:ss.fff"), AgentBySkill.Count.ToString()));
            } 
            return RT;
        }
        public bool SetAgentHaveBlending(Dictionary<string, int> dicAgentHaveBlending)
        {
            bool RT = false;
            try
            {
                //result = string.Format("You entered: {0}", dicAgentHaveBlending.Count.ToString());
                AgentHaveBlending = new Dictionary<string, int>(dicAgentHaveBlending);
                MccMLogger.Debug(string.Format("{0}  AgentHaveBlending {1} .\n",
                                                      DateTime.Now.ToString("h:mm:ss.fff"), AgentHaveBlending.Count.ToString()));
                //Thread.Sleep(5000);
                //OperationContext.Current.GetCallbackChannel<LoLi_ServiceCallback>().Notify(value);
                RT = true;
            }
            catch (Exception ex)
            {
                MccMLogger.Debug(string.Format("{0} error  {1}   {2}   .\n",
                                                DateTime.Now.ToString("h:mm:ss.fff"), ex.Message, ex.Source));
                MccMLogger.Debug(string.Format("{0} error AgentHaveBlending {1} .\n",
                                                     DateTime.Now.ToString("h:mm:ss.fff"), AgentHaveBlending.Count.ToString()));
            }
            return RT;
        }
        public bool isAlive()
        {
           
            return true;
        }
        public bool JoinTheConversation(string userName)
        {
           // bool RT = true;
            try
                {
                // Subscribe the user to the conversation
                LoLi_ServiceCallback registeredUser = OperationContext.Current.GetCallbackChannel<LoLi_ServiceCallback>();

                if (!_userList.ContainsKey(userName))
                {
                    _userList.Add(userName, registeredUser);
                    MccMLogger.Debug(string.Format("User {0}  Join conversation.", userName));
                }
                else
                {
                    _userList[userName] = registeredUser;
                    MccMLogger.Debug(string.Format("User {0} already joined conversation.", userName));
                }
                return true;
            }

            catch (Exception ex)
            {
                MccMLogger.Error(string.Format("User {0} failed to join conversation. ", userName), ex);
                return false;
            }

            //_callbackList.ForEach(
            //    delegate(LoLi_ServiceCallback callback)
            //    {
            //        try
            //        {
            //            callback.NotifyUserJoinedTheConversation(userName);
            //            //callback.Notify(userName);
            //            _registeredUsers++;
            //        }
            //        catch
            //        {
            //            _callbackList.Remove(callback);
            //        }
            //    });
        }
        public string[] ReceiveUserInfo(string userName)
        {
            if (!_userList.ContainsKey(userName))
            {
                JoinTheConversation(userName);
            }
            string[] RT = new string[3] { "0", "0", "0" };
            string[] tmp = GetStaticUserInfo();
            if (tmp != null)
                RT = tmp;
            //MccMLogger.Debug(string.Format("ReceiveUserInfo User: {0} 25={1}  19={2} 24={3} assign Campaigns: {4}",
            //                                 userName, RT[1], RT[0], RT[2], AssignCampaigns));
            return RT;
        }

        private string[] GetStaticUserInfo()
        {
            /*ID #0: isBlended
                *ID #1: Timeout before popup
                *ID #1: Dialer break code
            */
           
            if (StaticUserInfo == null)
            {
                lock (locker)
                {

                    try
                    {
                        string UserInfoParamsString = System.Configuration.ConfigurationManager.AppSettings["UserInfoParams"].ToString();
                        if (!string.IsNullOrWhiteSpace(UserInfoParamsString))
                        {
                            string[] tmp = UserInfoParamsString.Split(',');
                            if (tmp.Length == 3)
                            {
                                StaticUserInfo = tmp;
                                MccMLogger.Info("CP:GetStaticUserInfo  = " + UserInfoParamsString);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MccMLogger.Error("CP:GetStaticUserInfo failed", ex);
                    }
                }
            }
            return StaticUserInfo;

        }

        /*public string[] ReceiveUserInfo(string userName)
        {
            string[] RT = new string[3] { "0", "0", "0" };
            try
            {
                if (AgentBySkill.ContainsKey(userName))
                    foreach (KeyValuePair<int, int> pair in AgentBySkill[userName])
                        foreach (KeyValuePair<int, int> arr in skillInfoByCamp[pair.Key])
                        {
                            Array array = null;

                            if (CampaignsInfo.TryGetValue(arr.Key, out array))
                            {
                                //Array entry = CampaignsInfo[arr.Key];
                                //foreach (object[] entry in array)
                                //{
                                //int loc =25;
                                int itemp = int.Parse(array.GetValue(25).ToString());
                                //    if ((entry[17] != null) && (entry[17].ToString().Length > 0))
                                //    {
                                //    }
                                //}
                                if (itemp == 1)
                                {
                                    RT[1] = itemp.ToString();
                                }
                                RT[0] = array.GetValue(19).ToString();//int.Parse(array.GetValue(19).ToString());
                                RT[2] = array.GetValue(24).ToString(); //int.Parse(array.GetValue(24).ToString());
                            }
                        }

                MccMLogger.Debug(string.Format("{0}   ReceiveUserInfo 25={1}  19={2} 24={3}  .\n",
                                                                         DateTime.Now.ToString("h:mm:ss.fff"), RT[1], RT[0], RT[2]));
                MccMLogger.Debug(string.Format("{0}  skillInfoByCamp {1} AgentBySkill {2}  .\n",
                                                    DateTime.Now.ToString("h:mm:ss.fff"), skillInfoByCamp.Count.ToString(), AgentBySkill.Count.ToString()));
            }
            catch (Exception ex)
            {
                MccMLogger.Error("CP::ReceiveUserInfo", ex);
            }
            return RT;
        }*/
        public string[] ReceiveCampInfo(string userName, int campID)
        {
            Array array = null;
            string[] RT = null;
             ReturnData DB_ReturnData=null;;
            try
            {
                if (CampaignsInfo.TryGetValue(campID, out array))
                {
                    RT = new string[array.Length];
                    int Ind = 0;
                    RT[Ind++] = array.GetValue(1).ToString();
                    RT[Ind++] = array.GetValue(8).ToString();
                    RT[Ind++] = array.GetValue(9).ToString();
                    RT[Ind++] = array.GetValue(10).ToString();
                    RT[Ind++] = array.GetValue(11).ToString();
                    RT[Ind++] = array.GetValue(12).ToString();
                    RT[Ind++] = array.GetValue(13).ToString();
                    RT[Ind++] = array.GetValue(14).ToString();
                    RT[Ind++] = array.GetValue(21).ToString();
                    RT[Ind++] = array.GetValue(23).ToString();
                    RT[Ind++] = array.GetValue(20).ToString();
                    RT[Ind++] = array.GetValue(22).ToString();
                    DB_ReturnData = dbManger.GetAgentByLoginID(userName);
                    if (check_dbRD(DB_ReturnData))
                    {
                        RT[Ind++] = DB_ReturnData.DataInfo.GetField(1);
                        //DB_ReturnData.DataInfo.printData();
                    }

                }
            }
            catch (Exception ex)
            {
                MccMLogger.Error("CP::ReceiveCampInfo", ex);
             }
            return RT;
        }
        public string[] ReceiveRecord(string userName, int isBlending)
        {
            MccMLogger.Info("CP::ReceiveRecord for " + userName + ". RecID = " + isBlending.ToString());
            //const int BLANDING_FLAG = 17; not in use
            string[] aTemp = null;
            try
            {
                //int[] campid;
                //if (DicCampsByAgent.ContainsKey(userName))
                //{
                //    string[] sCampsList = DicCampsByAgent[userName].Split(',');
                //    //campid = new int[120];
                //    campid = Array.ConvertAll(sCampsList, s => int.Parse(s));
                int []campid = { isBlending }; // handle direct records popup by agent
                mut.WaitOne();
                ReturnData DB_ReturnData = dbManger.GetRecordsToCall_CP(campid, userName);
                mut.ReleaseMutex();    
                if ((check_dbRD(DB_ReturnData)) && (DB_ReturnData.StatusInfo.RowsCount == 1))
                {
                    DB_ReturnData.DataInfo.printData();
                    DataTable dt = DB_ReturnData.DataInfo.Data;
                    var arr1 = dt.Rows.Cast<DataRow>().Select(r => r.ItemArray).ToArray();
                    int Ind = 0;
                    foreach (object[] entry in arr1)
                    {
                        aTemp = new string[entry.Length];

                        foreach (object obj in entry)
                        {
                            aTemp[Ind++] = obj.ToString();
                        }


                    }
                }
            }

            catch (Exception ex)
            {
                MccMLogger.Error("CP::ReceiveRecord failed. User:" + userName, ex);
            }

            return aTemp;
        }
      
        public bool is_progressive(string AgentLoginID)
        {
            MccMLogger.Debug(" is_progressive call  >DM::is_progressive. is_progressive = " + AgentLoginID );
            bool RT = false;
            try
            {

                RT = dbManger.find_agentprogressive(AgentLoginID);
               
            }
            catch (Exception ex)
            {
                RT = false;
                MccMLogger.Debug("<-- is_progressive error " + ex.ToString());
                MccMLogger.Debug("     is_progressive " + ex.Message.ToString());
                MccMLogger.Debug("     is_progressive " + ex.Source.ToString());
                MccMLogger.Debug("     is_progressive " + ex.StackTrace.ToString() + " -->");

            }
            return RT;
        }

        public bool SetAgentState(string AgentLoginID, DateTime StateTime, string State, string OldState, string StateDetails, string Phone, string Skill, string Application, string CallID, string extDN, string agtName)        {
            MccMLogger.Debug(" SetAgentState calli  >DM::UpdateAgentState. AgentLoginID = " + AgentLoginID + " State = " + State + " details = " + StateDetails);
            bool RT = false;
            try
            {

                ReturnData DB_ReturnData = dbManger.UpdateAgentState(AgentLoginID, StateTime, State, OldState, StateDetails, Phone, Skill, Application, CallID, extDN, agtName);
                DB_ReturnData.DataInfo.printData();
                if (DB_ReturnData.StatusInfo.Status == ActivityStatus.Success)
                    RT = true;
            }
            catch (Exception ex)
            {
                MccMLogger.Debug("<-- SetComment cant  get array from DB " + ex.ToString());
                MccMLogger.Debug("    SetComment cant  get array from DB " + ex.Message.ToString());
                MccMLogger.Debug("    SetComment cant  get array from DB " + ex.Source.ToString());
                MccMLogger.Debug("    SetComment cant  get array from DB " + ex.StackTrace.ToString() + " -->");

            }
            return RT;
        }
        /*public string[] ReceiveRecord(string userName, int isBlending)
        {
        // orig
            string[] aTemp = null;
            try
            {
                int I = 0;
                int[] campid;
                if (AgentBySkill.ContainsKey(userName))
                {
                    campid = new int[120];
                    //Array array = null;
                    //int itemp1 = 0;
                    foreach (KeyValuePair<int, int> pair in AgentBySkill[userName])
                    {
                        foreach (KeyValuePair<int, int> arr in skillInfoByCamp[pair.Key])
                        {

                            if (isBlending == 1)
                                campid[I++] = arr.Key;
                            else
                                campid[I++] = arr.Key;

                            //if (CampaignsInfo.TryGetValue(arr.Key, out array))
                            //    {
                            //        //Array entry = CampaignsInfo[arr.Key];
                            //        //foreach (object[] entry in array)
                            //        //{
                            //        //int loc =25;
                            //        int itemp = int.Parse(array.GetValue(25).ToString());
                            //        itemp1 = int.Parse(array.GetValue(0).ToString());

                            //    }



                            //    if (CampaignsInfo.ContainsKey(pair.Key))
                            //        if (CampaignsInfo[pair.Key][3] == 1)
                            //        {
                            //            campid[I++] = pair.Key;
                            //        }
                            //}
                            //else
                            //    campid[I++] = pair.Key;

                        }
                    }

                    ReturnData DB_ReturnData = dbManger.GetRecordsToCall_CP(campid, userName);
                    MccMLogger.Debug(string.Format("{0}  campid {1}   .\n",
                                                   DateTime.Now.ToString("h:mm:ss.fff"), campid.Length.ToString()));
                    DB_ReturnData.DataInfo.printData();
                    if (check_dbRD(DB_ReturnData))
                    {
                        DataTable dt = DB_ReturnData.DataInfo.Data;
                        dt = DB_ReturnData.DataInfo.Data;
                        var arr1 = dt.Rows.Cast<DataRow>().Select(r => r.ItemArray).ToArray();
                        int Ind = 0;
                        foreach (object[] entry in arr1)
                        {
                            aTemp = new string[entry.Length];

                            foreach (object obj in entry)
                            {
                                aTemp[Ind++] = obj.ToString();
                            }


                        }

                    }
                }
            }

            catch (Exception ex)
            {
                MccMLogger.Debug("<--error callHabdlerHost cant  get array from DB " + ex.ToString());
                MccMLogger.Debug("    callHabdlerHost cant  get array from DB " + ex.Message.ToString());
                MccMLogger.Debug("    callHabdlerHost cant  get array from DB " + ex.Source.ToString());
                MccMLogger.Debug("    callHabdlerHost cant  get array from DB " + ex.StackTrace.ToString() + " -->");

            }

            return aTemp;
        }*/
        public Dictionary<int, string[]> ReceiveCallCode(int CampID)
        {
            int[] aCode1;
            Dictionary<int, string[]> allCampCallCode = new Dictionary<int, string[]>();
            string[] asCode;
            try
            {
                if (campByCode.TryGetValue(0, out aCode1))
                {
                    foreach (int entry in aCode1)
                        if (CodeInfo.TryGetValue(entry, out asCode))
                            allCampCallCode.Add(entry, asCode);
                }
                if (campByCode.TryGetValue(CampID, out aCode1))
                {
                    foreach (int entry in aCode1)
                        if (CodeInfo.TryGetValue(entry, out asCode))
                            allCampCallCode.Add(entry, asCode);
                }
                MccMLogger.Debug(string.Format("CP::RecieveCallsCode for campaign {0} Codes count = {1}",
                                                   CampID.ToString(), allCampCallCode.Count.ToString()));
            }
                 
            catch (Exception ex)
            {
                MccMLogger.Error("CP::RecieveCallsCode failed ", ex);
            }

            return allCampCallCode;
        }
        public bool SetComment(int recID, string Comments, string userID)
        {
            MccMLogger.Debug(string.Format("{0} SetComment   recID {1} Comments {2} userID {3} .\n",
                                                   DateTime.Now.ToString("h:mm:ss.fff"), recID.ToString(), Comments, userID));
            bool RT = false;
            try
            {
                ReturnData DB_ReturnData = dbManger.UpdateDCRec_Comment(recID, Comments, userID);
                
                DB_ReturnData.DataInfo.printData();
                if (DB_ReturnData.StatusInfo.Status == ActivityStatus.Success)
                    RT = true;
            }
            catch (Exception ex)
            {
                MccMLogger.Debug("<-- SetComment cant  get array from DB " + ex.ToString());
                MccMLogger.Debug("    SetComment cant  get array from DB " + ex.Message.ToString());
                MccMLogger.Debug("    SetComment cant  get array from DB " + ex.Source.ToString());
                MccMLogger.Debug("    SetComment cant  get array from DB " + ex.StackTrace.ToString() + " -->");

            }
            return RT;
        }
        public bool SetStatus(int recID, int tryID, int callStatus, string userID)
        {
            MccMLogger.Debug(string.Format("CP::SetStatus   recID {0} callStatus {1} userID {2} tryID {3}",
                                                   recID.ToString(), callStatus.ToString(), userID, tryID.ToString()));
            bool RT = false;
            try
            {
                ReturnData DB_ReturnData = dbManger.UpdateDCR_SetStatus(recID, tryID, callStatus, userID);
                if (DB_ReturnData.StatusInfo.Status == ActivityStatus.Success)
                    RT = true;
            }
            catch (Exception ex)
            {
                MccMLogger.Error("CP::SetStatus failed ", ex);
            }
            return RT;
        }
        public bool setRecRls(int recID, string userID)
        {
            MccMLogger.Debug(string.Format("CP::setRecRls  recID {0}  userID {1}",
                                                   recID.ToString(),  userID ));
            bool RT = false;
            try
            {
                ReturnData DB_ReturnData = dbManger.UpdateDCR_release(recID, userID);
                if (DB_ReturnData.StatusInfo.Status == ActivityStatus.Success)
                    RT = true;
            }
            catch (Exception ex)
            {
                MccMLogger.Error("CP::setRecRls failed ", ex);

            }
            return RT;
        }

        public bool setCloseSP(int recID, string userID)
        {
            MccMLogger.Debug(string.Format("CP::setCloseSP  recID {0}  userID {1}",
                                                   recID.ToString(), userID));
            bool RT = false;
            try
            {
                
                ReturnData DB_ReturnData = dbManger.UpdateDCR_CloseSP(recID, userID);
                if (DB_ReturnData.StatusInfo.Status == ActivityStatus.Success)
                    RT = true;
            }
            catch (Exception ex)
            {
                MccMLogger.Error("<CP::SetCloseSP failed ", ex);
            }
            return RT;
        }


        public bool SetStatusNew(int RecordID, int TryID, int callStatus, DateTime nextAttemptDate, int priority, string User)
        {
            MccMLogger.Debug(string.Format("CP::SetStatus Reschedule  recID {0} tryID {1} callStatus {2} nextAttemptDate {3}  priority {4} User {5}",
                                                   RecordID.ToString(), TryID.ToString(), callStatus.ToString(), nextAttemptDate.ToString(), priority.ToString(), User));
            bool RT = false;
            try
            {
                ReturnData DB_ReturnData = dbManger.UpdateDCR_SetStatus(RecordID, TryID, callStatus, nextAttemptDate, priority, User);
                if (DB_ReturnData.StatusInfo.Status == ActivityStatus.Success)
                    RT = true;
            }
            catch (Exception ex)
            {
                MccMLogger.Error("CP::SetStatus Reschedule  failed", ex);
            }
            return RT;
        }
        public bool SetReturntoQueue(int RecId, int TryID, string userID)
        {
            MccMLogger.Debug(string.Format("CP::SetReturntoQueue  recID {0} tryID {1} User {2}",
                                                   RecId.ToString(), TryID.ToString(), userID));
            bool RT = false;
            try
            {
                ReturnData DB_ReturnData = dbManger.UpdateDCR_ReturntoQueue(RecId, TryID, userID);
                if (DB_ReturnData.StatusInfo.Status == ActivityStatus.Success)
                    RT = true;
            }
            catch (Exception ex)
            {
                MccMLogger.Error("CP::SetReturntoQueue  failed RecID = " + RecId.ToString() + " ", ex);
            }
            return RT;
        }
        public bool SetNextAttempt(int recID, DateTime NextDate, int priority, string userID)
        {
            MccMLogger.Debug(string.Format("CP::SetNextAttempt  recID {0} NextDate {1} priority {2} userID {3}",
                                                recID.ToString(), NextDate.ToString(), priority.ToString(), userID));
            bool RT = false;
            try
            {
                ReturnData DB_ReturnData = dbManger.UpdateDCRec_NextAttempt(recID, NextDate, priority, userID);
                //DB_ReturnData.DataInfo.printData();
                if (DB_ReturnData.StatusInfo.Status == ActivityStatus.Success)
                    RT = true;
            }
            catch (Exception ex)
            {
                MccMLogger.Error("CP::SetNextAttempt  failed RecID = " + recID.ToString() + " "  ,  ex);
            }
            return RT;
        }
        public bool SetDCRecord(int recID, string phoneNum, DateTime startRecord, int CallStatus, int tryID, int recStatus, int recRes, string userID)
        {
            MccMLogger.Debug(string.Format("{0} SetDCRecord  recID {1} phoneNum {2} startRecord {3} CallStatus {4} .\n",
                                                   DateTime.Now.ToString("h:mm:ss.fff"), recID.ToString(), phoneNum, startRecord.ToString(), CallStatus.ToString()));
            MccMLogger.Debug(string.Format("{0} SetDCRecord  tryID {1} recStatus {2} recRes {3} userID {4} .\n",
                                                   DateTime.Now.ToString("h:mm:ss.fff"), tryID.ToString(), recStatus.ToString(), recRes.ToString(), userID));
            bool RT = false;
            try
            {
                ReturnData DB_ReturnData = dbManger.UpdateDCRecord(recID, phoneNum, startRecord, CallStatus, tryID, recStatus, recRes, userID);
                DB_ReturnData.DataInfo.printData();
                if (DB_ReturnData.StatusInfo.Status == ActivityStatus.Success)
                    RT = true;
            }
            catch (Exception ex)
            {
                MccMLogger.Debug("<-- SetComment cant  get array from DB " + ex.ToString());
                MccMLogger.Debug("    SetComment cant  get array from DB " + ex.Message.ToString());
                MccMLogger.Debug("    SetComment cant  get array from DB " + ex.Source.ToString());
                MccMLogger.Debug("    SetComment cant  get array from DB " + ex.StackTrace.ToString() + " -->");

            }
            return RT;
        }
        public bool SetCampByCode(Dictionary<int, int[]> _CampByCode)
        {
          
            bool RT = false;
            MccMLogger.Debug(string.Format("SetCampByCode: recieve data for {0} campaigns", _CampByCode.Count.ToString()));
            if (_CampByCode == null)
            {
                return false;
            }
            try
            {
                campByCode = new Dictionary<int, int[]>(_CampByCode);
                RT = true;
            }
            catch (Exception ex)
            {
                MccMLogger.Error("SetCampByCode failed.", ex);
            }
            return RT;
        }

        public bool SetCodeInfo(Dictionary<int, string[]> _CodeInfo)
        {
            bool RT = false;
            MccMLogger.Debug(string.Format("SetCampByCodeInfo: recieve data for {0} campaigns", _CodeInfo.Count.ToString()));
            if (_CodeInfo == null)
            {
                return false;
            }

            try
            {
                CodeInfo = new Dictionary<int, string[]>(_CodeInfo);
                RT = true;
            }
            catch (Exception ex)
            {
                MccMLogger.Error("SetCampByCodeInfo failed.", ex);
            }
                return RT;
        }

        public bool SetStartCall(int RecordID, int TryID, string PhoneNumbr, DateTime CallDate, string agentID)
        {
            MccMLogger.Debug(string.Format("CP::SetStartCall  RecordID {0} TryID {1} PhoneNumbr {2} CallDate {3} agentID {4}",
                                                   RecordID.ToString(), TryID.ToString(), PhoneNumbr, CallDate.ToString(), agentID));
            bool RT = false;
            try
            {
                ReturnData DB_ReturnData = dbManger.UpdateDCR_StartCall(RecordID, TryID, PhoneNumbr, CallDate, agentID);
                if (DB_ReturnData.StatusInfo.Status == ActivityStatus.Success)
                    RT = true;
            }
            catch (Exception ex)
            {
                MccMLogger.Error("CP::SetStartCall failed ", ex);
            }
            return RT;
        }
        public bool SetEndCall(int RecordID, int TryID, DateTime endDate, string User)
        {
            MccMLogger.Debug(string.Format("CP::SetEndCall  RecordID {0} TryID {1} endDate {2} User {3}",
                                                   RecordID.ToString(), TryID.ToString(), endDate.ToString(), User));
            bool RT = false;
            try
            {
                ReturnData DB_ReturnData = dbManger.UpdateDCR_EndCall(RecordID, TryID, endDate, User);
                if (DB_ReturnData.StatusInfo.Status == ActivityStatus.Success)
                    RT = true;
            }
            catch (Exception ex)
            {
                MccMLogger.Error("CP::SetEndCall failed ", ex);
            }
            return RT;
        }
        public bool SetEndCallNext(int RecordID, int TryID, DateTime endDate, int CallStatus, DateTime nextAttemptDate, int priority, string User)
        {
            MccMLogger.Debug(string.Format("{0} SetEndCallNext  RecordID {1} TryID {2} endDate {3} CallStatus {4) .\n",
                                                   DateTime.Now.ToString("h:mm:ss.fff"), RecordID.ToString(), TryID.ToString(), endDate.ToString(), CallStatus.ToString()));
            MccMLogger.Debug(string.Format("{0} SetEndCallNext  RecordID {1} nextAttemptDate {2} priority {3} User {4) .\n",
                                                   DateTime.Now.ToString("h:mm:ss.fff"), RecordID.ToString(), nextAttemptDate.ToString(), priority.ToString(), User));
            bool RT = false;
            try
            {
                ReturnData DB_ReturnData = dbManger.UpdateDCR_EndCall(RecordID, TryID, endDate, User);
                DB_ReturnData.DataInfo.printData();
                if (DB_ReturnData.StatusInfo.Status == ActivityStatus.Success)
                    RT = true;
            }
            catch (Exception ex)
            {
                MccMLogger.Debug("<-- SetComment cant  get array from DB " + ex.ToString());
                MccMLogger.Debug("    SetComment cant  get array from DB " + ex.Message.ToString());
                MccMLogger.Debug("    SetComment cant  get array from DB " + ex.Source.ToString());
                MccMLogger.Debug("    SetComment cant  get array from DB " + ex.StackTrace.ToString() + " -->");

            }
            return RT;
        }
        public Dictionary<int, string> ReceiveADName(string userName, int campID)
        {

           
            bool RT = false;
            Dictionary<int, string> lMapValue = new Dictionary<int, string>();
            MccMLogger.Debug(string.Format("CP::ReceiveADName  campID {0} userName {1} lMapValue {2}",
                                                 campID.ToString(), userName, lMapValue.Count.ToString()));
            try
            {
                if (campID > 0)
                    RT = CampDataMap.TryGetValue(campID, out lMapValue);
            }
            catch (Exception ex)
            {
                MccMLogger.Error("CP::ReceiveADName failed ", ex);
            }

            return lMapValue;
        }
        public Dictionary<int, string[]> ReceiveCampCode(int RecId, int campID, string userName)
        {
            
            int[] aCode1;
            Dictionary<int, string[]> CampCallCode = new Dictionary<int, string[]>();
            MccMLogger.Debug(string.Format("{0} ReceiveCampCode  RecId {1} userName {2} campID {3} .\n",
                                                DateTime.Now.ToString("h:mm:ss.fff"), campID.ToString(), userName, campID.ToString()));
            string[] asCode=null;
            try
            {
                
                if (campByCode.TryGetValue(campID, out aCode1))
                {
                    foreach (int entry in aCode1)
                        if (CodeInfo.TryGetValue(entry, out asCode))
                            CampCallCode.Add(entry, asCode);
                    MccMLogger.Debug(string.Format("{0} ReceiveCampCode  asCode {1} .\n",
                                               DateTime.Now.ToString("h:mm:ss.fff"), asCode.Length.ToString()));
                }
                

            }

            catch (Exception ex)
            {
                MccMLogger.Error("CP::ReceiveCampCode failed ", ex);
            }

            return CampCallCode;
        }
        public Dictionary<int, string[]> ReceivedefCode(int RecId, int campID, string userName)
        {
            int[] aCode1;
            Dictionary<int, string[]> defCallCode = new Dictionary<int, string[]>();
            MccMLogger.Debug(string.Format("{0} ReceivedefCode  RecId {1} userName {2} campID {3} .\n",
                                                DateTime.Now.ToString("h:mm:ss.fff"), campID.ToString(), userName, campID.ToString()));
            string[] asCode=null;
            try
            {
                if (campByCode.TryGetValue(0, out aCode1))
                {
                    foreach (int entry in aCode1)
                        if (CodeInfo.TryGetValue(entry, out asCode))
                            defCallCode.Add(entry, asCode);
                    MccMLogger.Debug(string.Format("{0} ReceivedefCode  asCode {1} .\n",
                                              DateTime.Now.ToString("h:mm:ss.fff"), asCode.Length.ToString()));
                }
            }

            catch (Exception ex)
            {
                MccMLogger.Debug("<-- callHabdlerHost cant  get array from DB " + ex.ToString());
                MccMLogger.Debug("    callHabdlerHost cant  get array from DB " + ex.Message.ToString());
                MccMLogger.Debug("    callHabdlerHost cant  get array from DB " + ex.Source.ToString());
                MccMLogger.Debug("    callHabdlerHost cant  get array from DB " + ex.StackTrace.ToString() + " -->");

            }

            return defCallCode;
        }
        public string[] ReceiveSysParmSP()
        {
            Array aTemp = null;
            string[] RT = new string[7] { "", "", "", "", "", "", "" };
            try
            {
                for (int i = 16; i < 23; i++)
                {
                    if (SysParm.TryGetValue(i, out aTemp))
                    {
                        RT[i - 16] = aTemp.GetValue(3).ToString();
                    }
                }
                MccMLogger.Debug(string.Format("ReceiveSysParmSP  RT {0}", RT.Length.ToString()));
            }
            catch (Exception ex)
            {
                MccMLogger.Error("CP::ReceiveSysParmSP failed ", ex);
            }

            return RT;
        }

        public string[] ReceiveRcordData(int RecId, int campID, string userName)
        {
            string[] aTemp = null;
            int rdc = 0;
            MccMLogger.Debug(string.Format("CP::ReceiveRcordData  RecId {0} userName {1} campID {2}",
                                                campID.ToString(), userName, campID.ToString()));
            try
            {
                ReturnData DB_ReturnData;
                DataTable dt;
                Dictionary<int, string> lMapValue = new Dictionary<int, string>();
                string fieldList;
                
                if (CampDataMap.TryGetValue(campID, out lMapValue))
                {
                    if (lMapValue.TryGetValue(999, out fieldList))
                    {
                        DB_ReturnData = dbManger.GetRcordData(RecId, fieldList);
                        DB_ReturnData.DataInfo.printData();
                        dt = DB_ReturnData.DataInfo.Data;
                        if (check_dbRD(DB_ReturnData))
                        {
                            DB_ReturnData.DataInfo.ToString();
                            dt = DB_ReturnData.DataInfo.Data;
                            aTemp = new string[30];
                            int Ind = 0;
                            var arr = dt.Rows.Cast<DataRow>().Select(r => r.ItemArray).ToArray();
                            rdc = arr.Length;
                            foreach (object[] entry in arr)
                            {
                                foreach (object obj in entry)
                                {
                                    aTemp[Ind++] = obj.ToString();
                                    MccMLogger.Debug(string.Format(" loc {0} = {1}",
                                               Ind.ToString(), obj.ToString()));
                                }
                            }

                        }
                    }
                }
                MccMLogger.Debug(string.Format("CP::ReceiveRcordData Ended. Items count = " + rdc.ToString()));
            }
               
            catch (Exception ex)
            {
                MccMLogger.Error("CP::ReceiveRcordData RecID =  " + RecId.ToString() +  " failed ",  ex);
            }
            return aTemp;
        }
        public string[] ReceiveFields(int RecId, int campID, string userName)
        {
            string[] aTemp = null;
            MccMLogger.Debug(string.Format("{0} ReceiveFields  RecId {1} userName {2} campID {3} .\n",
                                                DateTime.Now.ToString("h:mm:ss.fff"), campID.ToString(), userName, campID.ToString()));
            try
            {
                ReturnData DB_ReturnData;
                DataTable dt;
                Dictionary<int, string> lMapValue = new Dictionary<int, string>();
                string fieldList;
                if (CampDataMap.TryGetValue(campID, out lMapValue))
                {
                    if (lMapValue.TryGetValue(999, out fieldList))
                    {
                        DB_ReturnData = dbManger.GetRcordDataByFields(RecId, fieldList);
                        DB_ReturnData.DataInfo.printData();
                        dt = DB_ReturnData.DataInfo.Data;
                        if (check_dbRD(DB_ReturnData))
                        {
                            dt = DB_ReturnData.DataInfo.Data;
                            aTemp = new string[30];
                            int Ind = 0;
                            var arr = dt.Rows.Cast<DataRow>().Select(r => r.ItemArray).ToArray();
                            foreach (object[] entry in arr)
                            {
                                foreach (object obj in entry)
                                {
                                    aTemp[Ind++] = obj.ToString();
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MccMLogger.Debug("<-- GetRcordDataByFields cant  get array from DB " + ex.ToString());
                MccMLogger.Debug("    GetRcordDataByFields cant  get array from DB " + ex.Message.ToString());
                MccMLogger.Debug("    GetRcordDataByFields cant  get array from DB " + ex.Source.ToString());
                MccMLogger.Debug("    GetRcordDataByFields cant  get array from DB " + ex.StackTrace.ToString() + " -->");

            }
            return aTemp;
        }
        public void GetDataMapping(int campID)
        {
            try
            {
                MccMLogger.Debug(string.Format("CP::GetDataMapping for campID {0}", campID.ToString()));
                ReturnData DB_ReturnData;
                DataTable dt;
                string strMapName = "";
                string strMapHebName = "";
                Dictionary<int, string> lMapValue = new Dictionary<int, string>();
                if (campID > 0)
                {
                    DB_ReturnData = dbManger.GetAttachDataMapping(campID);
                    if (check_dbRD(DB_ReturnData))
                    {
                        DB_ReturnData.DataInfo.printData();
                        dt = DB_ReturnData.DataInfo.Data;
                        var arr = dt.Rows.Cast<DataRow>().Select(r => r.ItemArray).ToArray();
                        foreach (object[] entry in arr)
                        {
                            strMapName = strMapName + "," + entry[3].ToString();
                            strMapHebName = strMapHebName + "," + entry[1].ToString();
                            int mapValueKey = int.Parse(entry[2].ToString());
                            if (!lMapValue.ContainsKey(mapValueKey))
                                lMapValue.Add(mapValueKey, entry[1].ToString());
                            else
                                lMapValue[mapValueKey] = entry[1].ToString();
                        }
                        if (!lMapValue.ContainsKey(999))
                        {
                            lMapValue.Add(999, strMapName);
                            lMapValue.Add(998, strMapHebName);
                        }
                        else
                        {
                            lMapValue[999] = strMapName;
                            lMapValue[998] = strMapHebName;
                        }
                        if (CampDataMap.ContainsKey(campID))
                            CampDataMap[campID] = lMapValue;
                        else
                            CampDataMap.Add(campID, lMapValue);
                    }
                }
            }
            catch (Exception ex)
            {
                MccMLogger.Error("CommonParts::GetDataMapping campID= " + campID.ToString() + " error ", ex);
            }
        }
        public void GetuserDetail(string loginID, string userName, string computerName, string lineNo, string posID)
        {
            MccMLogger.Debug(string.Format("GetuserDetail  loginID {1} userName {2} computerName {3} lineNo {4} posID {5}",
                                                loginID, userName, computerName, lineNo, posID));
            ReturnData DB_ReturnData = dbManger.LoginOnlineAgent(loginID, userName, lineNo, posID, computerName, 0, 0, 0, 0);
            //DB_ReturnData.DataInfo.printData();
        }
        public void ReceiveMessage(string userName, List<string> addressList, string userMessage)
        {

            // Notify the users of a message.
            // Use an anonymous delegate and generics to do our dirty work.
            _callbackList.ForEach(
                delegate(LoLi_ServiceCallback callback)
                {
                    try
                    {
                        callback.NotifyUserOfMessage(userName, userMessage);
                    }
                    catch
                    {
                        _callbackList.Remove(callback);
                    }


                });
            // { callback.Notify(userName); });

        }
        public int LeaveTheConversation(string userName)
        {
            // Unsubscribe the user from the conversation.      
            LoLi_ServiceCallback registeredUser = OperationContext.Current.GetCallbackChannel<LoLi_ServiceCallback>();

            if (_callbackList.Contains(registeredUser))
            {
                _callbackList.Remove(registeredUser);
                _registeredUsers--;
            }

            // Notify everyone that user has arrived.
            // Use an anonymous delegate and generics to do our dirty work.
            _callbackList.ForEach(
                delegate(LoLi_ServiceCallback callback)
                {
                    try
                    {
                        callback.NotifyUserLeftTheConversation(userName);
                    }
                    catch
                    {
                        _callbackList.Remove(callback);
                    }
                });
            // { callback.Notify(userName); });

            return _registeredUsers;
        }
        public static bool check_dbRD(ReturnData DB_ReturnData)
        {
            try
            {
                if (DB_ReturnData == null)
                {
                    MccMLogger.Debug("check_dbRD:: DB_ReturnData object is null  ");
                    return false;
                }
                if (DB_ReturnData.StatusInfo.Status != ActivityStatus.Success)
                {
                    MccMLogger.Debug("check_dbRD:: Activity failed. status = " + DB_ReturnData.StatusInfo.Status.ToString());
                    return false;
                }
                if (DB_ReturnData.StatusInfo.RowsCount < 1)
                {
                    MccMLogger.Debug("check_dbRD:: No records found");
                    return false;
                }
                MccMLogger.Debug(string.Format("CheckDataObj:: Status={0}; Rows count={1}",
                               DB_ReturnData.StatusInfo.Status.ToString(),
                               DB_ReturnData.StatusInfo.RowsCount.ToString()));
                return true;
            }
            catch (Exception ex)
            {
                MccMLogger.Error("check_dbRD:: failed! ", ex);
                return false;
            }
        }
       /* public static bool check_dbRD(ReturnData DB_ReturnData)
        {
            if (DB_ReturnData == null)
                MccMLogger.Debug("callHabdlerHost DB_ReturnData object is null  ");
            else
                if (DB_ReturnData.DataInfo == null)
                MccMLogger.Debug("callHabdlerHost DB_ReturnData.DataInfo object is null ");
            else
                    if (DB_ReturnData.StatusInfo == null)
                MccMLogger.Debug("callHabdlerHost DB_ReturnData.StatusInfo object is null ");
            else
                        if (DB_ReturnData.DataInfo.Data == null)
                MccMLogger.Debug("callHabdlerHost DB_ReturnData.DataInfo.Data object is null ");
            else
                            if (DB_ReturnData.StatusInfo.Status != ActivityStatus.Success)
                MccMLogger.Debug("callHabdlerHost DB_ReturnData..StatusInfo.Status = fail or unknown ");
            else
                                if (DB_ReturnData.StatusInfo.RowsCount < 1)
                MccMLogger.Debug("callHabdlerHost DB_ReturnData.StatusInfo.RowsCount less then 1 " + DB_ReturnData.StatusInfo.RowsCount.ToString());
            else
                                    if (DB_ReturnData.DataInfo.Data.Rows.Count != DB_ReturnData.StatusInfo.RowsCount)
            {
                MccMLogger.Debug("<-- callHabdlerHost DB_ReturnData.StatusInfo.RowsCount <> DB_ReturnData.StatusInfo.RowsCount  ");
                MccMLogger.Debug(DB_ReturnData.StatusInfo.RowsCount.ToString() + " <> " + DB_ReturnData.StatusInfo.RowsCount.ToString() + "  --> ");
            }
            else
            {
                MccMLogger.Debug(string.Format("CheckDataObj:: Status={0}; Rows count={1}",
                            DB_ReturnData.StatusInfo.Status.ToString(),
                             DB_ReturnData.StatusInfo.RowsCount.ToString()));
                return true;
            }

            return false;
        }*/

        /*public static bool check_dbRD(ReturnData DB_ReturnData)
        {
            if (DB_ReturnData == null)
                MccMLogger.Debug("callHabdlerHost DB_ReturnData object is null  ");
            else
                if (DB_ReturnData.DataInfo == null)
                    MccMLogger.Debug("callHabdlerHost DB_ReturnData.DataInfo object is null ");
                else
                    if (DB_ReturnData.StatusInfo == null)
                        MccMLogger.Debug("callHabdlerHost DB_ReturnData.StatusInfo object is null ");
                    else
                        if (DB_ReturnData.DataInfo.Data == null)
                            MccMLogger.Debug("callHabdlerHost DB_ReturnData.DataInfo.Data object is null ");
                        else
                            if (DB_ReturnData.StatusInfo.Status != ActivityStatus.Success)
                                MccMLogger.Debug("callHabdlerHost DB_ReturnData..StatusInfo.Status = fail or unknown ");
                            else
                                if (DB_ReturnData.StatusInfo.RowsCount < 1)
                                    MccMLogger.Debug("callHabdlerHost DB_ReturnData.StatusInfo.RowsCount less then 1 " + DB_ReturnData.StatusInfo.RowsCount.ToString());
                                else
                                    if (DB_ReturnData.DataInfo.Data.Rows.Count != DB_ReturnData.StatusInfo.RowsCount)
                                    {
                                        MccMLogger.Debug("<-- callHabdlerHost DB_ReturnData.StatusInfo.RowsCount <> DB_ReturnData.StatusInfo.RowsCount  ");
                                        MccMLogger.Debug(DB_ReturnData.StatusInfo.RowsCount.ToString() + " <> " + DB_ReturnData.StatusInfo.RowsCount.ToString() + "  --> ");
                                    }
                                    else
                                    {
                                        MccMLogger.Debug("HAS ROWS = " + DB_ReturnData.StatusInfo.RowsCount.ToString());
                                        MccMLogger.Debug("Confirm = " + DB_ReturnData.DataInfo.Data.Rows.Count.ToString());
                                        MccMLogger.Debug("status = " + DB_ReturnData.StatusInfo.Status.ToString());
                                        return true;
                                    }

            return false;
        }*/

        public bool SetCampaignsList(Dictionary<int, Array> dicCampaignsInfo)
        {

            CommonParts.ChReplacement chr = new CommonParts.ChReplacement();
            try
            {
                dicCampaignsInfo = chr.SetCampaignsList();
                if (dicCampaignsInfo != null)
                {
                    CampaignsInfo = dicCampaignsInfo;
                    MccMLogger.Debug(string.Format("CommonParts::SetCampaignsList get {0} campaigns", dicCampaignsInfo.Count));
                }
                return true;
            }
            catch (Exception ex)
            {
                MccMLogger.Error("CommonParts::SetCampaignsList Failed", ex);
                return false;
            }


            /*
            if (dicCampaignsInfo == null)
            {
                MccMLogger.Debug("CommonParts::SetCampaignsList .No Active Campaigns Found");
                //dicCampaignsInfo.Clear();
                return true;
            }

            try
            {
                CampaignsInfo = new Dictionary<int, Array>(dicCampaignsInfo);
                MccMLogger.Debug(string.Format("CommonParts::SetCampaignsList get {0} campaigns", dicCampaignsInfo.Count));
                return true;
            }
            catch (Exception ex)
            {
                MccMLogger.Error("CommonParts::SetCampaignsList Failed", ex);
                return false;
            }
            */
        }

        public string[] GetLoggedAgents()
        {
            if (_userList == null)
                return new string[1] { "empty" };
            if (_userList.Count == 0)
                return new string[1] { "empty" };

            string[] AgentsList = _userList.Keys.ToArray();
            return AgentsList;
        }

        public DataTable ReceivePrivateRecords(string userName)
        {
            try
            {
                ReturnData DB_ReturnData = dbManger.GetPrivateRecords(userName);
                if ((DB_ReturnData.StatusInfo.Status == ActivityStatus.Success) && (DB_ReturnData.StatusInfo.RowsCount > 0))
                {
                    MccMLogger.Debug("CP::ReceivePrivateRecords  User = " + userName + " Private records = " + DB_ReturnData.StatusInfo.RowsCount.ToString());
                    return DB_ReturnData.DataInfo.Data;
                }
                else
                {
                    MccMLogger.Debug("CP::ReceivePrivateRecords  User = " + userName + " - Data not found");
                    return null;

                }
            }
            catch (Exception ex)
            {
                MccMLogger.Error("CP::ReceivePrivateRecords  User = " + userName + " failed " , ex);
                return null;
            }
        }

        //public bool SetCfgParams(Dictionary<string, string> DicCfgParams)
        //{
        //    if(DicCfgParams != null)
        //       CfgpPrams = DicCfgParams;
        //    return true;
        //}

        //public bool SetCampaignsByAgent(Dictionary<string, string> dicCampsByAgent)
        //{
        //    if (dicCampsByAgent != null)
        //        DicCampsByAgent = dicCampsByAgent;
        //    return true;
        //}
    }
    public class CP_MessageCallBack : LoLi_ServiceCallback
    {
        #region CPServiceCallback Methods
        private SynchronizationContext _uiSyncContext = null;
        CP_MessageCallBack()
        {
            _uiSyncContext = SynchronizationContext.Current;

        }

        public void NotifyUserJoinedTheConversation(string arg_Name)
        {
            // The UI thread won't be handling the callback, but it is the only one allowed to update the controls.  
            // So, we will dispatch the UI update back to the UI sync context.
            SendOrPostCallback callback =
                delegate(object state)
                {
                    string msg_user = state.ToString();
                    msg_user = msg_user.ToUpper();
                    // this.WriteMessage(String.Format("[{0}] has joined the conversation.", msg_user));
                };

            _uiSyncContext.Post(callback, arg_Name);
        }
        public void NotifyUserOfMessage(string arg_Name, string arg_Message)
        {
            // The UI thread won't be handling the callback, but it is the only one allowed to update the controls.  
            // So, we will dispatch the UI update back to the UI sync context.
            SendOrPostCallback callback =
                delegate(object state)
                {
                    // this.WriteMessage(String.Format("[{0}]: {1}", arg_Name.ToUpper(), arg_Message));
                };

            _uiSyncContext.Post(callback, arg_Name);
        }

        public void NotifyUserLeftTheConversation(string arg_Name)
        {
           
            SendOrPostCallback callback =
                delegate(object state)
                {
                    string msg_user = state.ToString();
                    msg_user = msg_user.ToUpper();
                    MccMLogger.Debug("CommonParts::NotifyUserLeftTheConversation " + arg_Name); ;
                };

            _uiSyncContext.Post(callback, arg_Name);
        }
        public void Notify(string value)
        {
            MccMLogger.Debug("CommonParts::Notify " + value); ;
        }

        #endregion
    }

}
