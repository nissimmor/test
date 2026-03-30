using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using LoginForm;
using System.Configuration;
using MCCMLogger;
using System.Data.SqlClient;
using System.Data;

namespace WcfLogInService
{
   
    [ServiceBehavior(
            ConcurrencyMode = ConcurrencyMode.Single,
            InstanceContextMode = InstanceContextMode.PerCall)]

    public class LoLiService : ILoLiService
    {
        public string GetData(string value)
        {
            try
            {
                //ILoLiServiceCallback callback = OperationContext.Current.GetCallbackChannel<ILoLiServiceCallback>();
                //callback.OnDataReceived(string.Format("You entered: {0}", value));
                return string.Format("You entered: {0}", value);
            }
            catch (Exception ex)
            {
                MccMLogger.Info(ex.Message);
            }
            return "error";
        }

        public string AgentLoLi(string agentID, string computerName, string extNumber, string userID,string stat)
        {
            try
            {
                string webuser = ConfigurationManager.AppSettings["webuser"];
                string webpass = ConfigurationManager.AppSettings["webpass"];
                string AACCIP = ConfigurationManager.AppSettings["AACCIP"];

                MccMLogger.Info(DateTime.Now.ToString());
                //AdressChg adressChg = new AdressChg("4321", "sip:68311@mac.org.il"); //4321
                //string agentID, string extNumber, string userID,string wepass,string webuser,string ipacc
                 AdressChg adressChg = new AdressChg(agentID, extNumber,userID,webpass,webuser, AACCIP); //432
                bool sw_ok=adressChg.NewAdress();
                if (sw_ok)
                {
                    UpdtUserName(agentID, computerName, extNumber, userID, stat);
                }
                else
                    agentID = "0";
                MccMLogger.Info(DateTime.Now.ToString());

                MccMLogger.Info("             *****************       exit             ***************** ");
                MccMLogger.Info("      agentID= " + agentID+ "   ");

                //ILoLiServiceCallback callback = OperationContext.Current.GetCallbackChannel<ILoLiServiceCallback>();
                //callback.OnDataReceived(string.Format(" method: You entered: {0}", agentID));
                //return string.Format(" method: You entered: {0}", agentID);
                
            }
            catch (Exception ex)
            {
                MccMLogger.Error("Failed to start CP service !!! \n\n", ex);

                MccMLogger.Info("Press the Enter key to terminate service.");
                //Console.ReadLine();
            }
            return agentID;
        }
        public string AgentFree(string agentID, string computerName, string extNumber, string userID, string stat)
        {
            try
            {
                UpdtUserName(agentID, computerName, extNumber, userID, stat);

                MccMLogger.Info(DateTime.Now.ToString());

                MccMLogger.Info("             AgentFree *****************       exit             ***************** ");
                MccMLogger.Info(" ");


            }
            catch (Exception ex)
            {
                MccMLogger.Error("Failed to  Agen tFree !!! \n\n", ex);

            }
            return agentID;
        }

        public string UpdtUserName(string loginID, string computerName, string extension, string userName, string state)
        {
            string resultl = "a";
            MccMLogger.Debug("CH::sp_agt_updtComp - start");
            string sqlProc = "";
            try
            {
                //DBmanager dbManger = new DBmanager();
                SqlDataReader reader = null;
                sqlProc = @"[dbo].[sp_agt_updtComp]";
                MccMLogger.Debug("  > DM ::" + sqlProc);
                SqlCommand command = new SqlCommand(sqlProc);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@computerName", SqlDbType.NVarChar).Value = computerName;
                command.Parameters.Add("@extension", SqlDbType.NVarChar).Value = extension;
                command.Parameters.Add("@userName", SqlDbType.NVarChar).Value = userName;
                command.Parameters.Add("@loginID", SqlDbType.NVarChar).Value = loginID;
                command.Parameters.Add("@state", SqlDbType.NVarChar).Value = state;
                SqlConnection connection = new SqlConnection();
                connection.ConnectionString = ConfigurationManager.ConnectionStrings["AgentSP"].ToString();
                command.Connection = connection;
                //command.CommandType = CommandType.Text;
                connection.Open();
                reader = command.ExecuteReader();


                //  dbManger.GetAgentInfo(computerName, userID, out reader, sqlProc);
                if (reader.HasRows)
                {
                    
                    MccMLogger.Debug("CH:: sp_agt_updtComp - has " + reader.RecordsAffected + " row");

                }
                else
                    MccMLogger.Debug("CH:: sp_agt_updtComp - has no row");

            }
            catch (Exception ex)
            {
                MccMLogger.Error("CH:: sp_agt_getDetails  error ", ex);
                return resultl;
            }

            //RT = DictintEqualsOrderM(dicskillInfoByCamp, skillInfoByCamp);

            return resultl;
        }
        public string AgentDB( string computerName, string userName)
        {
            string resultl = "a";
            MccMLogger.Debug("CH::GetAgentInfo - start");
            string sqlProc = "";
            try
            {
                //DBmanager dbManger = new DBmanager();
                SqlDataReader reader = null;
                sqlProc = @"[dbo].[sp_agt_getDetails]";
                MccMLogger.Debug("  > DM ::" + sqlProc);
                SqlCommand command = new SqlCommand(sqlProc);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@computerName", SqlDbType.NVarChar).Value = computerName;
                command.Parameters.Add("@userName", SqlDbType.NVarChar).Value = userName;
                SqlConnection connection = new SqlConnection();
                connection.ConnectionString = ConfigurationManager.ConnectionStrings["AgentSP"].ToString(); 
                command.Connection = connection;
                //command.CommandType = CommandType.Text;
                connection.Open();
                reader = command.ExecuteReader();
                

              //  dbManger.GetAgentInfo(computerName, userID, out reader, sqlProc);
                 if (reader.HasRows)
                 {
                    string ca = "";
                    string c = "";
                    string a = "";
                    while (reader.Read())
                    {
                        //todo(reader);
                         ca = reader.GetString(0);
                        //check to c  
                         c = reader.GetString(1);
                         a = reader.GetString(2);
                        break;
                    }
                     resultl = ca + ";" + c + ";" + a + ";";
                     MccMLogger.Debug("CH:: sp_agt_getDetails - has " + reader.RecordsAffected + " row");

                 }
                 else
                     MccMLogger.Debug("CH:: sp_agt_getDetails - has no row");

            }
            catch (Exception ex)
            {
                MccMLogger.Error("CH:: sp_agt_getDetails  error ", ex);
                return resultl;
            }
           
            //RT = DictintEqualsOrderM(dicskillInfoByCamp, skillInfoByCamp);
            
            return resultl;
        }
        public static bool GetRecordInfo()
        {
            MccMLogger.Debug("CH::GetRecordInfo - start");
            string sqlProc = "";
            bool rt = false;
            try
            {
                DBmanager dbManger = new DBmanager();
                SqlDataReader reader = null;
                /*ReturnData rt = dbManger.GetRecordsToCall(1, out reader);
                if (rt.StatusInfo.Status != ActivityStatus.Success)
                {
                    TccLogger.Error("CH:: GetRecordsToCall Failed to get active . error -  " + rt.StatusInfo);
                    return false;
                }
                */

                sqlProc = @"[dbo].[sp_Moma_GetAgentExt]";
                dbManger.GetAgentExtension(1, out reader, sqlProc);
                if (reader.HasRows)
                {
                    //todo(reader);
                    MccMLogger.Debug("CH:: GetRecordsToCall - has " + reader.RecordsAffected + " row");
                    rt = true;
                }
                else
                    MccMLogger.Debug("CH:: GetRecordsToCall - has no row");


                //ReturnData DB_ReturnData = dbManger.GetCampaignsInfo(CampType);
                /*DataTable dt = rt.DataInfo.Data;


                //0 _campPriority
                //1_campSkillID
                //2 _campWrapup
                //3_campIsBleding
                //4_NotReadyReasonsID
                 var a = "";
                var b = "";
                if ((check_dbRD(rt)) && (rt.StatusInfo.RowsCount > 0))
                {
                    var arr = dt.Rows.Cast<DataRow>().Select(r => r.ItemArray).ToArray();
                    foreach (object[] entry in arr)
                    {
                        //int itemp = int.Parse(entry[loc].ToString());
                      // a = entry[loc].ToString();
                        //else
                         //   CampaignsInfo.Add(itemp, entry);
                    }
                }*/
            }
            catch (Exception ex)
            {
                MccMLogger.Error("CH:: GetRecordsToCall  error ", ex);
                rt= false;
                return rt;
            }
            //RT = DictintEqualsOrderM(dicskillInfoByCamp, skillInfoByCamp);
            return rt;

        }
        public string AddAgent(string agentID, string computerName, string extNumber, string userID, string stat)
        {
            try
            {
                string webuser = ConfigurationManager.AppSettings["webuser"];
                string webpass = ConfigurationManager.AppSettings["webpass"];
                string AACCIP = ConfigurationManager.AppSettings["AACCIP"];

                MccMLogger.Info(DateTime.Now.ToString());
                //AdressChg adressChg = new AdressChg("4321", "sip:68311@mac.org.il"); //4321
                //string agentID, string extNumber, string userID,string wepass,string webuser,string ipacc
                AdressChg adressChg = new AdressChg(agentID, extNumber, userID, webpass, webuser, AACCIP); //432
                bool sw_ok = adressChg.NewAdress();
                if (sw_ok)
                {
                    UpdtUserName(agentID, computerName, extNumber, userID, stat);
                }
                MccMLogger.Info(DateTime.Now.ToString());

                MccMLogger.Info("             *****************       exit             ***************** ");
                MccMLogger.Info(" ");

                //ILoLiServiceCallback callback = OperationContext.Current.GetCallbackChannel<ILoLiServiceCallback>();
                //callback.OnDataReceived(string.Format(" method: You entered: {0}", agentID));
                //return string.Format(" method: You entered: {0}", agentID);

            }
            catch (Exception ex)
            {
                MccMLogger.Error("Failed to start CP service !!! \n\n", ex);

                MccMLogger.Info("Press the Enter key to terminate service.");
               // Console.ReadLine();
            }
            return agentID;
        }
        public bool  addUserAD(string userID, int action)
        {
            ADChgUser adUserChg = new ADChgUser();
            string AACCIP68 = "172.26.30.68";
            string AACCIP67 = "172.26.30.67";
            bool status = false;

            if (action==0)
            {
                // del user from ad at AACC
                status = adUserChg.DeleteUN(userID, AACCIP68);
                status = adUserChg.DeleteUN(userID, AACCIP67);
            }

            else
            {
                // add user to AD at aacc
                try
                {
                    status = adUserChg.AddUN(userID, AACCIP68);
                }
                catch (Exception ex)
                {
                    MccMLogger.Info("User Error" + ex.StackTrace);
                }
                try
                {
                    status = adUserChg.AddUN(userID, AACCIP68);
                }
                catch (Exception ex)
                {
                    MccMLogger.Info("User Error" + ex.StackTrace);
                }
                try
                {
                    status = adUserChg.AddUN(userID, AACCIP67);
                }
                catch (Exception ex)
                {
                    MccMLogger.Info("User Error" + ex.StackTrace);
                }
                try
                {
                    status = adUserChg.AddUN(userID, AACCIP67);
                }
                catch (Exception ex)
                {
                    MccMLogger.Info("User Error"+ex.StackTrace);
                }
            }
            return status; 

        }

        public int setOtherPhone(string userID, int isOtherPhone,string phone)
        {
            MccMLogger.Debug("CH::setOtherPhone - start");
            string sqlProc = "";
            int rt = 0;
            try
            {
                DBmanager dbManger = new DBmanager();
                SqlDataReader reader = null;
                sqlProc = @"[dbo].[sp_agt_setOtherPhone]";
                dbManger.setOtherPhoneDB(userID, isOtherPhone,phone, out reader, sqlProc);
                if (reader.HasRows)
                {
                    //todo(reader);
                    MccMLogger.Debug("CH:: setOtherPhone - has " + reader.RecordsAffected + " row");
                    rt = 1;
                }
                else
                    MccMLogger.Debug("CH:: setOtherPhone - has no row");

                
            }
            catch (Exception ex)
            {
                MccMLogger.Error("CH:: setOtherPhone  error ", ex);
                rt = 0;
                
            }
            //RT = DictintEqualsOrderM(dicskillInfoByCamp, skillInfoByCamp);
            return rt;

            
        }
        public string getOtherPhone(string userID)
        {
            MccMLogger.Debug("CH::getOtherPhone - start");
            string sqlProc = "";
            string rt = "0";
            try
            {
                DBmanager dbManger = new DBmanager();
                SqlDataReader reader = null;
                sqlProc = @"[dbo].[sp_agt_getOtherPhone]";
                
        dbManger.getOtherPhoneDB(userID,  out reader, sqlProc);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        //todo(reader);
                        rt = reader.GetString(0);                      
                        break;
                    }
                    MccMLogger.Debug("CH:: getOtherPhone - has " + reader.RecordsAffected + " row");
                }
                else
                {
                    MccMLogger.Debug("CH:: getOtherPhone - has no row");
                    rt = "0";
                }


            }
            catch (Exception ex)
            {
                MccMLogger.Error("CH:: getOtherPhone  error ", ex);
                rt = "0";

            }
            //RT = DictintEqualsOrderM(dicskillInfoByCamp, skillInfoByCamp);
            return rt;


        }

        public int setOutPhone(string userID, int isOutDial, string bphone,string extension)
        {
            MccMLogger.Debug("CH::setOutPhone - start");
            string sqlProc = "";
            int rt = 0;
            try
            {
                DBmanager dbManger = new DBmanager();
                SqlDataReader reader = null;
                sqlProc = @"[dbo].[sp_agt_setOutPhone]";
                dbManger.setOutPhoneDB(userID, isOutDial, bphone, extension, out reader, sqlProc);
                if (reader.HasRows)
                {
                    //todo(reader);
                    MccMLogger.Debug("CH:: setOutPhone - has " + reader.RecordsAffected + " row");
                    rt = 1;
                }
                else
                    MccMLogger.Debug("CH:: setOutPhone - has no row");


            }
            catch (Exception ex)
            {
                MccMLogger.Error("CH:: setOutPhone  error ", ex);
                rt = 0;

            }
            //RT = DictintEqualsOrderM(dicskillInfoByCamp, skillInfoByCamp);
            return rt;


        }
        public string getOutPhone(string userID)
        {
            MccMLogger.Debug("CH::getOutPhone - start");
            string sqlProc = "";
            string rt = "0";
            try
            {
                DBmanager dbManger = new DBmanager();
                SqlDataReader reader = null;
                sqlProc = @"[dbo].[sp_agt_getOutPhone]";

                dbManger.getOutPhoneDB(userID, out reader, sqlProc);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        //todo(reader);
                        rt = reader.GetString(0);
                        break;
                    }
                    MccMLogger.Debug("CH:: getOutPhone - has " + reader.RecordsAffected + " row");
                }
                else
                {
                    MccMLogger.Debug("CH:: getOutPhone - has no row");
                    rt = "0";
                }


            }
            catch (Exception ex)
            {
                MccMLogger.Error("CH:: getOutPhone  error ", ex);
                rt = "0";

            }
            //RT = DictintEqualsOrderM(dicskillInfoByCamp, skillInfoByCamp);
            return rt;


        }

    }

}
