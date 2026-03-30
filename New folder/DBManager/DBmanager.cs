using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Odbc;
using System.Net;
using System.Reflection;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using InterSystems.Data.CacheClient;
using MCCMLogger;
using System.Linq;


namespace WcfLogInService
{
    public class DBmanager
    {
        protected DbHandler DialerDB = null;
        protected DbHandler AgentSPDB = null;
        protected DbHandler RealTimeDB = null;
        protected DbHandler DialerAdminDB = null;
        protected DbHandler ReportsSystemDB = null;
        protected CasheDbHandler AACC_DB = null;

        private static string DialerDB_ConnStr = "";
        private static string AgentSPDB_ConnStr = ""; 
        private static string RealTimeDB_ConnStr = "";
        private static string DialerAdminDB_ConnStr = "";
        private static string ReportsSystemDB_ConnStr = "";
        private static string AACC_DB_ConnStr = "";

        private string auditUserName = "";
        public const string DefTableName = "Result";
        public string ipAddress = "";
        public DBmanager(string AuditName = "DataManager")
        {
            this.auditUserName = AuditName;


            try
            {
                SetConnectionString("Dialer");
                DialerDB = new DbHandler(DialerDB_ConnStr,"DialerDB");
                SetConnectionString("AgentSP");
                AgentSPDB = new DbHandler(AgentSPDB_ConnStr, "AgentSPDB");
                SetConnectionString("RealTime");
                RealTimeDB = new DbHandler(RealTimeDB_ConnStr, "RealTimeDB");

                SetConnectionString("DialerAdmin");
                DialerAdminDB = new DbHandler(DialerAdminDB_ConnStr, "AdminDB");

                SetConnectionString("ReportsSystem");
                ReportsSystemDB = new DbHandler(ReportsSystemDB_ConnStr, "ReportsDB");

                SetConnectionString("AACC");
                AACC_DB = new CasheDbHandler(AACC_DB_ConnStr, "AACC_DB");

            }
            catch (Exception ex)
            {
                MccMLogger.Error("  Failed to initialize DB Manager", ex);
            }
        }

        private string SetConnectionString (string Name)
        {
            string RT = "";
            switch (Name)
            {
                case "Dialer":
                    if (!string.IsNullOrWhiteSpace(DialerDB_ConnStr))
                        return DialerDB_ConnStr;
                    DialerDB_ConnStr = GetConnectionString("Dialer");
                    break;
             case "AgentSP":
                    if (!string.IsNullOrWhiteSpace(AgentSPDB_ConnStr))
                        return AgentSPDB_ConnStr;
                    AgentSPDB_ConnStr = GetConnectionString("AgentSP");
                    break;
                case "RealTime":
                    if(!string.IsNullOrWhiteSpace(RealTimeDB_ConnStr))
                        return RealTimeDB_ConnStr;
                    RealTimeDB_ConnStr = GetConnectionString("RealTime");
                    break;
                case "DialerAdmin":
                    if (!string.IsNullOrWhiteSpace(DialerAdminDB_ConnStr))
                        return DialerAdminDB_ConnStr;
                    DialerAdminDB_ConnStr = GetConnectionString("DialerAdmin");
                    break;
                case "ReportsSystem":
                    if (!string.IsNullOrWhiteSpace(ReportsSystemDB_ConnStr))
                        return ReportsSystemDB_ConnStr;
                    ReportsSystemDB_ConnStr = GetConnectionString("ReportsSystem");
                    break;
                case "AACC":
                    if (!string.IsNullOrWhiteSpace(AACC_DB_ConnStr))
                        return AACC_DB_ConnStr;
                    AACC_DB_ConnStr = GetConnectionString("AACC");
                    break;
            }
            return RT;
        }

        #region general
        
        /* Procedure: GetSystemParams
         * Input: none
         * Stored procedure: Dialer Admin DB -> [dbo].[sp_DM_GetSystemParams]
         * Return datatable: All campaign fields, if campaign is not deleted
         */
        public ReturnData GetSystemParams()
        {
            MccMLogger.Debug("  >DM::Get system parameters");
            //if(!CheckConnectionObject(DialerDB))
            //{
            //    return BuildError("DB handler is null");
            //}
            //bool result = false;
            DataTable dt = new DataTable(DefTableName);
            
            string SqlCmd = @"[dbo].[sp_DM_GetSystemParams]";
            SqlCommand cmd = new SqlCommand(SqlCmd);
            cmd.CommandType = CommandType.StoredProcedure;
            
            return DialerDB.ExecQuery(cmd);
        }
        
        #endregion general
        
        #region Campaigns
         
        /* Procedure: SetCampaignStatus
         * Input: int campaign ID
         *        int Status
         * Stored procedure: Dialer DB -> [dbo].[sp_DM_SetCampaignStatus]
         * Return datatable: NONE
         */
        public ReturnData SetCampaignStatus(int campaignID, int status, string User = "")
        {
            MccMLogger.Debug("  >DM::Set Campaign Status. CampaignID #" + campaignID.ToString() + " Status = " + status.ToString());
            if (!CheckConnectionObject(DialerDB))
            {
                return BuildError("DB handler is null");
            }
            
            string SqlCmd = @"[dbo].[sp_DM_SetCampaignStatus]";
            SqlCommand cmd = new SqlCommand(SqlCmd);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Campaign_Code", SqlDbType.Int).Value = campaignID;
            cmd.Parameters.Add("@Status", SqlDbType.Int).Value = status;
            cmd.Parameters.Add("@last_Update_User", SqlDbType.NVarChar).Value = GetUser(User);

            ReturnData rt = DialerDB.ExecNonQuery(cmd);
            LogAuditRecord("Set campaign status", "CampaignID=" + campaignID.ToString() + ";Status=" + status.ToString());
            return rt;
        }
         
        /* Procedure: GetCampaigns
         * Input: none
         * Stored procedure: Dialer DB -> [dbo].[sp_DM_GetCampaigns]
         * Return datatable: All campaigns  fields, if campaign is not deleted regarding campaign type
         */
        public ReturnData GetCampaignsInfo(int type)
        {
            MccMLogger.Debug("  >DM::Get Campaigns info. Type = " + type.ToString());
            if (!CheckConnectionObject(DialerDB))
            {
                return BuildError("DB handler is null");
            }
            
            string SqlCmd = @"[dbo].[sp_DM_GetCampaigns]";
            SqlCommand cmd = new SqlCommand(SqlCmd);
            cmd.Parameters.Add("@Type", SqlDbType.Int).Value = type;
            cmd.CommandType = CommandType.StoredProcedure;
        
            return DialerDB.ExecQuery(cmd);
        }
         
        /* Procedure: GetCampaignInfo
         * Input: int campaign ID
         * Stored procedure: [dbo].[sp_DM_GetCampaignInfo]
         * Return datatable: All campaign fields
         */
        public ReturnData GetCampaignInfo(int campaignID)
        {
            MccMLogger.Debug("  >DM::GetCampaign info. CampaignID #" + campaignID.ToString());
            if (!CheckConnectionObject(DialerDB))
            {
                return BuildError("DB handler is null");
            }
            
            string SqlCmd = @"[dbo].[sp_DM_GetCampaignInfo]";
            SqlCommand cmd = new SqlCommand(SqlCmd);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Campaign_Code", SqlDbType.Int).Value = campaignID;
        
            return DialerDB.ExecQuery(cmd);
        }
         
        /* Procedure: GetCampaignStatus
         * Input: int campaign ID
         * Stored procedure: [dbo].[sp_DM_GetCampaignStatus]
         * Return datatable: Campaign's status
         */
        public ReturnData GetCampaignStatus(int campaignID)
        {
            MccMLogger.Debug("  >DM::Get Campaign status. CampaignID #" + campaignID.ToString());
            if (!CheckConnectionObject(DialerDB))
            {
                return BuildError("DB handler is null");
            }
            
            //int effectedRows = 0;
            string SqlCmd = @"[dbo].[sp_DM_GetCampaignStatus]";
            SqlCommand cmd = new SqlCommand(SqlCmd);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Campaign_Code", SqlDbType.Int).Value = campaignID;
        
            return DialerDB.ExecQuery(cmd);
        }
        
        public ReturnData test(SqlCommand cmd)
        {
            cmd = new SqlCommand("SELECT GETDATE() AS [eee]");
            cmd.CommandType = CommandType.Text;
            return DialerDB.ExecQuery(cmd);
        }

        #endregion Campaigns

        #region CallProxy
        public ReturnData UpdateMomaInCalls_DNIS(int recId, string dialingExtension)
        {
            MccMLogger.Debug("  >DM::UpdateMomaInCalls_DNIS. recordID = " + recId.ToString() + ". DNIS = " + dialingExtension);
            if (!CheckConnectionObject(DialerDB))
            {
                return BuildError("DB handler is null");
            }
            string SqlCmd = @"[dbo].[sp_DM_UpdateDCRecord_5]";
            SqlCommand cmd = new SqlCommand(SqlCmd);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@RecID", SqlDbType.Int).Value = recId;
            cmd.Parameters.Add("@DNIS", SqlDbType.Text).Value = dialingExtension;

            ReturnData rt = DialerDB.ExecNonQuery(cmd);
            return rt;
        }
        public ReturnData UpdateDCR_SetStatus_CP(int RecordID, int TryID, int callStatus, DateTime nextAttemptDate, int priority, string User)
        {

            MccMLogger.Debug("  >DM::UpdateDCR_SetStatus. recordID = " + RecordID.ToString() + ". Try ID # " + TryID.ToString() + ". Call status = " + callStatus.ToString() + "  Next attempt = " + nextAttemptDate.ToString("DD-MM-YYYY HH:MM:ss"));
            string SqlCmd = @"[dbo].[sp_DM_UpdateDCR_SetStatus_2]";
            if (!CheckConnectionObject(DialerDB))
            {
                return BuildError("DB handler is null");
            }
            //switch(CampType)
            //{
            //    case CampaignType.Preview:
            //        SqlCmd =  @"[dbo].[sp_DM_UpdateDCR_SetStatus_2]";
            //        break;
            //    case CampaignType.Callback:
            //        SqlCmd =  @"[dbo].[sp_DM_UpdateDCR_SetStatus_2_CB]";
            //        break;
            //    default:
            //        SqlCmd =  @"[dbo].[sp_DM_UpdateDCR_SetStatus_2]";
            //        break;
            //}

            SqlCommand cmd = new SqlCommand(SqlCmd);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@RecID", SqlDbType.Int).Value = RecordID;
            cmd.Parameters.Add("@TryID", SqlDbType.Int).Value = TryID;
            cmd.Parameters.Add("@CallStatus", SqlDbType.Int).Value = callStatus;
            cmd.Parameters.Add("@NextAttemptDate", SqlDbType.DateTime).Value = nextAttemptDate;
            cmd.Parameters.Add("@Priority", SqlDbType.Int).Value = priority;
            cmd.Parameters.Add("@User", SqlDbType.NVarChar).Value = User;

            ReturnData rt = DialerDB.ExecQuery(cmd);
            return rt;
            /*[dbo].[sp_DM_UpdateDCR_SetStatus_2]
	                @RecID		int,
	                @TryID		int,
	                @CallStatus	int,
	                @NextAttemptDate	datetime,
	                @Priority	int,
	                @User	nvarchar (36)*/
        }
        public ReturnData UpdateDCR_SMS(int recId)
        {
            MccMLogger.Debug("  >DM::UpdateDCR_SMS. RecID #" + recId);
            if (!CheckConnectionObject(DialerDB))
            {
                return BuildError("DB handler is null");
            }

            string SqlCmd = @"[dbo].[sp_DM_UpdateSMS]";
            SqlCommand cmd = new SqlCommand(SqlCmd);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@recId", SqlDbType.Int).Value = recId;
            ReturnData rt = DialerDB.ExecNonQuery(cmd);
            return rt;
        }

        public ReturnData UpdateDCR_release_CP(int RecordID, string User)
        {
            MccMLogger.Debug("  >DM::UpdateDCR_release. recordID = " + RecordID.ToString());
            string SqlCmd = @"[dbo].[sp_DM_UpdateDCR_Release]";
            if (!CheckConnectionObject(DialerDB))
            {
                return BuildError("DB handler is null");
            }
            //switch(CampType)
            //{
            //    case CampaignType.Preview:
            //        SqlCmd =  @"[dbo].[sp_DM_UpdateDCR_Release]";
            //        break;
            //    case CampaignType.Callback:
            //        SqlCmd =  @"[dbo].[sp_DM_UpdateDCR_Release_CB]";
            //        break;
            //    default:
            //        SqlCmd =  @"[dbo].[sp_DM_UpdateDCR_Release]";
            //        break;
            //}

            SqlCommand cmd = new SqlCommand(SqlCmd);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@RecID", SqlDbType.Int).Value = RecordID;
            cmd.Parameters.Add("@User", SqlDbType.NVarChar).Value = User;

            ReturnData rt = DialerDB.ExecNonQuery(cmd);
            return rt;
            /*[dbo].[sp_DM_UpdateDCR_Release]
	                @RecID		int,
	                @User	nvarchar (36)*/
        }


        #endregion CallProxy
        #region RealTimeDB

        /* Procedure: GetCampaigns
         * Input: none
         * Stored procedure: Dialer DB -> [dbo].[sp_DM_GetAgentsRTInfo] on DB114
         * Return datatable: All campaign fields, if campaign is not deleted
         */
        public ReturnData GetAgentsRTStatus()
        {
            MccMLogger.Debug("  >DM::GetAgentsRTStatus info");
            if (!CheckConnectionObject(RealTimeDB))
            {
                return BuildError("DB handler is null");
            }
            
            string SqlCmd = @"[dbo].[sp_DM_GetAgentsRTInfo]";
            SqlCommand cmd = new SqlCommand(SqlCmd);
            cmd.CommandType = CommandType.StoredProcedure;
        
            return RealTimeDB.ExecQuery(cmd);
        }
        
        #endregion RealTimeDB
        
        #region AdminDB
        
        public void LogAuditRecord(string activity, string data)
        {
            string ErrMsg = "";
            SqlCommand cmd = new SqlCommand("sp_InsertAuditRecord");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@SessionID", SqlDbType.NVarChar).Value = Guid.NewGuid().ToString();    // TO CHECK
            cmd.Parameters.Add("@IPAddress", SqlDbType.NVarChar).Value = this.ipAddress;   
            cmd.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = this.auditUserName;   
            cmd.Parameters.Add("@URLAccessed", SqlDbType.NVarChar).Value = activity;  
            cmd.Parameters.Add("@Data", SqlDbType.NVarChar).Value = data;  
        
            DialerAdminDB.ExecNonQuery(cmd, out ErrMsg);
        }

        #endregion AdminDB
        
        #region DialingRecord
         
        /* Procedure: UpdateDCRecord
         * Update entire record at: dbo.DialerCustomers
         * Input: int RecordID, 
         *     string PhoneNumbr
         *     DateTime CallDate
         *     int CallStatus
         *     int TryID = number of current #Try
         *     string Comments. if == "" then ignore
         *     int RecordStatus
         *     DateTime NextAttemptDate. if== Datetime.MinValue then ignore
         *     string User = "". if empty then == AuditName
         * Stored procedure: Dialer DB -> [dbo].[UpdateDCRecord_1] via _UpdateDialingRecord
         * Return datatable: NONE
         */
        public ReturnData UpdateDCRecord(int RecordID, string PhoneNumbr, DateTime CallDate, int CallStatus, int TryID, string Comments, int RecordStatus, int recordStatuseReason, DateTime NextAttemptDate, string User = "")
        {
            MccMLogger.Debug("  >DM::UpdateDCRecord. recordID = " + RecordID.ToString() + " TryID = " + TryID.ToString() + " Status = " + CallStatus.ToString() + " (All)");
            return _UpdateDialingRecord(RecordID, PhoneNumbr, CallDate, CallStatus, TryID, Comments, RecordStatus,recordStatuseReason, NextAttemptDate, User);
        }
         
        /* Procedure: UpdateDCRecord
         * Update call fields only  at: dbo.DialerCustomers
         * Input: int RecordID, 
         *     string PhoneNumbr
         *     DateTime CallDate
         *     int CallStatus
         *     int TryID = number of current #Try
         *     int RecordStatus
         *     string User . if empty then == AuditName
         * Stored procedure: Dialer DB -> [dbo].[UpdateDCRecord_1] via _UpdateDialingRecord
         * Return datatable: NONE
         */
        public ReturnData UpdateDCRecord(int RecordID, string PhoneNumbr, DateTime CallDate, int CallStatus, int TryID, int RecordStatus, int recordStatuseReason, string User = "")
        {
            MccMLogger.Debug("  >DM::UpdateDCRecord. recordID = " + RecordID.ToString() + " (call status)");
            return _UpdateDialingRecord(RecordID, PhoneNumbr, CallDate, CallStatus, TryID, "", RecordStatus, recordStatuseReason, DateTime.MinValue, User);
        }
         
        /* Procedure: UpdateDCRecord
         * Update call fields + next attempt date  at: dbo.DialerCustomers
         * Input: int RecordID, 
         *     string PhoneNumbr
         *     DateTime CallDate
         *     int CallStatus
         *     int TryID = number of current #Try
         *     int RecordStatus
         *     DateTime NextAttemptDate. if== Datetime.MinValue then ignore
         *     string User . if empty then == AuditName
         * Stored procedure: Dialer DB -> [dbo].[sp_DM_UpdateDCRecord_1] via _UpdateDialingRecord
         * Return datatable: NONE
         */
        public ReturnData UpdateDCRecord(int RecordID, string PhoneNumbr, DateTime CallDate, int CallStatus, int TryID, int RecordStatus, int recordStatuseReason, DateTime nextAttemptdate, string User = "")
        {
            MccMLogger.Debug("  >DM::UpdateDCRecord. recordID = " + RecordID.ToString() + " (call status + next attempt)");
            return _UpdateDialingRecord(RecordID, PhoneNumbr, CallDate, CallStatus, TryID, "", RecordStatus, recordStatuseReason, nextAttemptdate, User);
        }
         
        /* Procedure: UpdateDCRecord
         * Updatecall fields + comment  at: dbo.DialerCustomers
         * Input: int RecordID, 
         *     string PhoneNumbr
         *     DateTime CallDate
         *     int CallStatus
         *     int TryID = number of current #Try
         *     int RecordStatus
         *     string Comments. if == "" then ignore
         *     string User  if empty then == AuditName
         * Stored procedure: Dialer DB -> [dbo].[sp_DM_UpdateDCRecord_1] via _UpdateDialingRecord
         * Return datatable: NONE
         */
        public ReturnData UpdateDCRecord(int RecordID, string PhoneNumbr, DateTime CallDate, int CallStatus, int TryID, int RecordStatus,  int RecordStatusReason, string Comment, string User = "")
        {
            MccMLogger.Debug("  >DM::UpdateDCRecord. recordID = " + RecordID.ToString() + " (call status + comment)");
            return _UpdateDialingRecord(RecordID, PhoneNumbr, CallDate, CallStatus, TryID, Comment, RecordStatus, RecordStatusReason, DateTime.MinValue, User);
        }
         
        /* Procedure: UpdateDCRec_NextAttempt
         * Update: call next attempt date field at: dbo.DialerCustomers
         * Input: int RecordID, 
         *     DateTime NextAttemptDate
         *     string User .if empty then == AuditName
         * Stored procedure: Dialer DB -> [dbo].[sp_DM_UpdateDCRecord_2] 
         * Return datatable: NONE
         */
        public ReturnData UpdateDCRec_NextAttempt(int RecordID, DateTime NextAttemptDate, int priority, string User = "")
        {
            MccMLogger.Debug("  >DM::UpdateDCRec_NextAttempt. recordID = " + RecordID.ToString() + ". nextAttemptDate = " + NextAttemptDate.ToShortDateString() + " " + NextAttemptDate.ToShortTimeString());
            if (!CheckConnectionObject(DialerDB))
            {
                return BuildError("DB handler is null");
            }
            string SqlCmd = @"[dbo].[sp_DM_UpdateDCRecord_2]";
            SqlCommand cmd = new SqlCommand(SqlCmd);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@RecID", SqlDbType.Int).Value = RecordID;
            cmd.Parameters.Add("@NextAttemptDate", SqlDbType.DateTime).Value = NextAttemptDate;
            cmd.Parameters.Add("@priority", SqlDbType.Int).Value = priority;
            cmd.Parameters.Add("@User", SqlDbType.NVarChar).Value = GetUser(User);
            
            ReturnData rt = DialerDB.ExecNonQuery(cmd);
            return rt;
            /*dbo].[sp_DM_UpdateDCRecord_2]
            Update next attempt date
            @RecID		int,
            @NextAttemptDate Datetime,
            @Priority int,
            @User nvarchar (36)*/
        }
         
        /* Procedure: UpdateDCRec_Status
         * Update: record status  at: dbo.DialerCustomers
         * Input: int RecordID, 
         *     int RecordStatus
         *     int RecordStatusReason
         *     string User .if empty then == AuditName
         * Stored procedure: Dialer DB -> [dbo].[sp_DM_UpdateDCRecord_3] 
         * Return datatable: NONE
         */
        public ReturnData UpdateDCRec_Status(int RecordID, int RecordStatus, int RecordStatusReason, string User = "")
        {
            MccMLogger.Debug("  >DM::UpdateDCRec_Status. recordID = " + RecordID.ToString() + ". Status = " + RecordStatus);
            if (!CheckConnectionObject(DialerDB))
            {
                return BuildError("DB handler is null");
            }
            
            string SqlCmd = @"[dbo].[UpdateDCRecord_3]";
            SqlCommand cmd = new SqlCommand(SqlCmd);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@RecID", SqlDbType.Int).Value = RecordID;
            cmd.Parameters.Add("@RecordStatus", SqlDbType.Int).Value = RecordStatus;
            cmd.Parameters.Add("@RecordStatusReason", SqlDbType.Int).Value = RecordStatus;
            cmd.Parameters.Add("@User", SqlDbType.NVarChar).Value = GetUser(User);
            
            ReturnData rt = DialerDB.ExecNonQuery(cmd);
            return rt;
            /*[dbo].[UpdateDCRecord_3]
            update record status
            @RecID		int,
            @RecordStatus int,
            @RecordStatusReason int,
            @User nvarchar (36)*/
        }
         
        /* Procedure: UpdateDCRec_Comment
         * Update:comments field  at: dbo.DialerCustomers
         * Input: int RecordID, 
         *     string Comments
         *     string User = "". if empty then == AuditName
         * Stored procedure: Dialer DB -> [dbo].[sp_DM_UpdateDCRecord_4] 
         * Return datatable: NONE
         */
        public ReturnData UpdateDCRec_Comment(int RecordID, string Comments, string User = "")
        {
            MccMLogger.Debug("  >DM::UpdateDCRec_Comment. recordID = " + RecordID.ToString());
            string SqlCmd = @"[dbo].[sp_DM_UpdateDCR_Comment]";
            if (!CheckConnectionObject(DialerDB))
            {
                return BuildError("DB handler is null");
            }
            //switch(CampType)
            //{
            //    case CampaignType.Preview:
            //        SqlCmd =  @"[dbo].[sp_DM_UpdateDCR_Comment]";
            //        break;
            //    case CampaignType.Callback:
            //        SqlCmd =  @"[dbo].[sp_DM_UpdateDCR_Comment_CB]";
            //        break;
            //    default:
            //        SqlCmd =  @"[dbo].[sp_DM_UpdateDCR_Comment]";
            //        break;
            //}
            
            SqlCommand cmd = new SqlCommand(SqlCmd);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@RecID", SqlDbType.Int).Value = RecordID;
            cmd.Parameters.Add("@Comments", SqlDbType.NVarChar).Value = Comments;
            cmd.Parameters.Add("@User", SqlDbType.NVarChar).Value = GetUser(User);
            
            ReturnData rt = DialerDB.ExecNonQuery(cmd);
            return rt;
            /*[dbo].[sp_DM_UpdateDCR_Comment]
            Update comments
            @RecID		int,
            @Comments	nvarchar(max),
            @User	 nvarchar (36)*/
        }


           /* Procedure: UpdateDCR_StartCall
         * Update DialerCustomer record for start agent call 
         * Input: int RecordID, 
         *    RecID		int,
	          TryID		int,
	          PhoneNum	nvarchar(12),
	          CallStartDate	datetime,
	          AgentID	nvarchar (36)
         * Stored procedure: Dialer DB -> [dbo].[sp_DM_UpdateDCR_StartCall]
         * Return datatable: NONE
         */
        public ReturnData UpdateDCR_StartCall(int RecordID, int TryID, string PhoneNumbr, DateTime CallStartDate, string agentID)
        {
            MccMLogger.Debug("  >DM::UpdateDCR_StartCall. recordID = " + RecordID.ToString() + ". Try ID # " + TryID.ToString());
            string SqlCmd = @"[dbo].[sp_DM_UpdateDCR_StartCall]";
            if (!CheckConnectionObject(DialerDB))
            {
                return BuildError("DB handler is null");
            }
            //switch(CampType)
            //{
            //    case CampaignType.Preview:
            //        SqlCmd =  @"[dbo].[sp_DM_UpdateDCR_StartCall]";
            //        break;
            //    case CampaignType.Callback:
            //        SqlCmd =  @"[dbo].[sp_DM_UpdateDCR_StartCall_CB]";
            //        break;
            //    default:
            //        SqlCmd =  @"[dbo].[sp_DM_UpdateDCR_StartCall]";
            //        break;
            //}
            
            SqlCommand cmd = new SqlCommand(SqlCmd);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@RecID", SqlDbType.Int).Value = RecordID;
            cmd.Parameters.Add("@TryID", SqlDbType.Int).Value = TryID;
            cmd.Parameters.Add("@PhoneNum", SqlDbType.NVarChar).Value = PhoneNumbr;
            cmd.Parameters.Add("@CallDate", SqlDbType.DateTime).Value = CallStartDate;
            cmd.Parameters.Add("@AgentID", SqlDbType.NVarChar).Value = agentID;

            ReturnData rt = DialerDB.ExecNonQuery(cmd);
            return rt;

            /*
              [dbo].[sp_DM_UpdateDCR_StartCall]
	                @RecID		int,
	                @TryID		int,
	                @PhoneNum	nvarchar(12),
	                @CallDate	datetime,
	                @AgentID	nvarchar (36)*/
        }
        
        /* Procedure: UpdateDCR_ReturntoQueue
         * Update DialerCustomer record for return to queue if agent didnt dial (atfer timeout)
         * Input: RecID		int,
                  TryID int            
	              User	nvarchar (36)
         * Stored procedure: Dialer DB -> [dbo].[sp_DM_UpdateDCR_ReturnToQ]
         * Return datatable: NONE
         */

        public ReturnData UpdateDCR_ReturntoQueue(int RecordID, int TryID, string User )
        {
            MccMLogger.Debug("  >DM::UpdateDCR_ReturntoQueue. recordID = " + RecordID.ToString() + ".User #" + User);
            string SqlCmd = @"[dbo].[sp_DM_UpdateDCR_ReturnToQ]";
            if (!CheckConnectionObject(DialerDB))
            {
                return BuildError("DB handler is null");
            }

            //switch(CampType)
            //{
            //    case CampaignType.Preview:
            //        SqlCmd =  @"[dbo].[sp_DM_UpdateDCR_ReturnToQ]";
            //        break;
            //    case CampaignType.Callback:
            //        SqlCmd =  @"[dbo].[sp_DM_UpdateDCR_ReturnToQ_CB]";
            //        break;
            //    default:
            //        SqlCmd =  @"[dbo].[sp_DM_UpdateDCR_ReturnToQ]";
            //        break;
            //}

            SqlCommand cmd = new SqlCommand(SqlCmd);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@RecID", SqlDbType.Int).Value = RecordID;
            cmd.Parameters.Add("@TryID", SqlDbType.Int).Value = TryID;
            cmd.Parameters.Add("@User", SqlDbType.NVarChar).Value = User;

            ReturnData rt = DialerDB.ExecNonQuery(cmd);
            return rt;
            
            /*[dbo].[sp_DM_UpdateDCR_ReturnToQ]
	                @RecID		int,
                    @TryID		int,
	                @User	nvarchar (36)*/
        }

        /* Procedure: UpdateDCR_EndCall
         * Update DialerCustomer record for end agent call on close state
         * Input:  RecID		int,
	               TryID		int,
	               EndDate	datetime,
	               CallStatus	int,
	               NextAttemptDate	datetime,
	               Priority	int,
	               User	nvarchar (36)
         * Stored procedure: Dialer DB -> [dbo].[sp_DM_UpdateDCR_EndCall_1]
         * Return datatable: NONE
         */
       
        public ReturnData UpdateDCR_EndCall(int RecordID, int TryID, DateTime endDate, string User ) 
        {
            MccMLogger.Debug("  >DM::UpdateDCR_EndCall. recordID = " + RecordID.ToString() + ". Try ID # " + TryID.ToString());
            string SqlCmd = @"[dbo].[sp_DM_UpdateDCR_EndCall_2]";
            if (!CheckConnectionObject(DialerDB))
            {
                return BuildError("DB handler is null");
            }
            //switch(CampType)
            //{
            //    case CampaignType.Preview:
            //        SqlCmd =  @"[dbo].[sp_DM_UpdateDCR_EndCall_2]";
            //        break;
            //    case CampaignType.Callback:
            //        SqlCmd =  @"[dbo].[sp_DM_UpdateDCR_EndCall_2_CB]";
            //        break;
            //    default:
            //        SqlCmd =  @"[dbo].[sp_DM_UpdateDCR_EndCall_2]";
            //        break;
            //}
            
            SqlCommand cmd = new SqlCommand(SqlCmd);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@RecID", SqlDbType.Int).Value = RecordID;
            cmd.Parameters.Add("@TryID", SqlDbType.Int).Value = TryID;
            cmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = endDate;
            cmd.Parameters.Add("@User", SqlDbType.NVarChar).Value = User;

            ReturnData rt = DialerDB.ExecNonQuery(cmd);
            return rt;
            /* [dbo].[sp_DM_UpdateDCR_EndCall_2]
	                @RecID		int,
	                @TryID		int,
	                @EndDate	datetime,
	                @User	nvarchar (36)*/
        }
        public ReturnData _UpdateDCR_EndCall(int RecordID, int TryID, DateTime endDate, int callStatus, DateTime nextAttemptDate, int priority, string User = "") 
        {

            MccMLogger.Debug("  >DM::_UpdateDCR_EndCall. recordID = " + RecordID.ToString() + ". Try ID # " + TryID.ToString() + ". Call status = " + callStatus.ToString() + " (open)");
            if (!CheckConnectionObject(DialerDB))
            {
                return BuildError("DB handler is null");
            }
            string SqlCmd = @"[dbo].[sp_DM_UpdateDCR_EndCall_1]";
            SqlCommand cmd = new SqlCommand(SqlCmd);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@RecID", SqlDbType.Int).Value = RecordID;
            cmd.Parameters.Add("@TryID", SqlDbType.Int).Value = TryID;
            cmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = endDate;
            cmd.Parameters.Add("@CallStatus", SqlDbType.Int).Value = callStatus;
            cmd.Parameters.Add("@NextAttemptDate", SqlDbType.DateTime).Value = nextAttemptDate;
            cmd.Parameters.Add("@Priority", SqlDbType.Int).Value = priority;
            cmd.Parameters.Add("@User", SqlDbType.NVarChar).Value = GetUser(User);

            ReturnData rt = DialerDB.ExecNonQuery(cmd);
            return rt;
            /*[dbo].[sp_DM_UpdateDCR_EndCall_1]
	                @RecID		int,
	                @TryID		int,
	                @EndDate	datetime,
	                @CallStatus	int,
	                @NextAttemptDate	datetime,
	                @Priority	int,
	                @User	nvarchar (36)*/
        }

        public ReturnData UpdateDCR_SetStatus(int RecordID, int TryID, int callStatus, string User ) 
        {

            MccMLogger.Debug("  >DM::UpdateDCR_SetStatus. recordID = " + RecordID.ToString() + ". Try ID # " + TryID.ToString() + ". Call status = " + callStatus.ToString()) ;
            string SqlCmd = @"[dbo].[sp_DM_UpdateDCR_SetStatus_1]";
            if (!CheckConnectionObject(DialerDB))
            {
                return BuildError("DB handler is null");
            }
            //switch(CampType)
            //{
            //    case CampaignType.Preview:
            //        SqlCmd =  @"[dbo].[sp_DM_UpdateDCR_SetStatus_1]";
            //        break;
            //    case CampaignType.Callback:
            //        SqlCmd =  @"[dbo].[sp_DM_UpdateDCR_SetStatus_1_CB]";
            //        break;
            //    default:
            //        SqlCmd =  @"[dbo].[sp_DM_UpdateDCR_SetStatus_1]";
            //        break;
            //}
            SqlCommand cmd = new SqlCommand(SqlCmd);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@RecID", SqlDbType.Int).Value = RecordID;
            cmd.Parameters.Add("@TryID", SqlDbType.Int).Value = TryID;
            cmd.Parameters.Add("@CallStatus", SqlDbType.Int).Value = callStatus;
            cmd.Parameters.Add("@User", SqlDbType.NVarChar).Value = User;

            ReturnData rt = DialerDB.ExecNonQuery(cmd);
            return rt;
            /*[dbo].[sp_DM_UpdateDCR_SetStatus_1]
	                @RecID		int,
	                @TryID		int,
	                @CallStatus	int,
                    @User	nvarchar (36)*/
        }

        public ReturnData UpdateDCR_SetStatus(int RecordID, int TryID, int callStatus, DateTime nextAttemptDate, int priority, string User ) 
        {

            MccMLogger.Debug("  >DM::UpdateDCR_SetStatus. recordID = " + RecordID.ToString() + ". Try ID # " + TryID.ToString() + ". Call status = " + callStatus.ToString() + "  Next attempt = " + nextAttemptDate.ToString("dd - MMM - yyyy HH:MM:ss")) ;
            string SqlCmd = @"[dbo].[sp_DM_UpdateDCR_SetStatus_2]";
            if (!CheckConnectionObject(DialerDB))
            {
                return BuildError("DB handler is null");
            }
            //switch(CampType)
            //{
            //    case CampaignType.Preview:
            //        SqlCmd =  @"[dbo].[sp_DM_UpdateDCR_SetStatus_2]";
            //        break;
            //    case CampaignType.Callback:
            //        SqlCmd =  @"[dbo].[sp_DM_UpdateDCR_SetStatus_2_CB]";
            //        break;
            //    default:
            //        SqlCmd =  @"[dbo].[sp_DM_UpdateDCR_SetStatus_2]";
            //        break;
            //}
            
            SqlCommand cmd = new SqlCommand(SqlCmd);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@RecID", SqlDbType.Int).Value = RecordID;
            cmd.Parameters.Add("@TryID", SqlDbType.Int).Value = TryID;
            cmd.Parameters.Add("@CallStatus", SqlDbType.Int).Value = callStatus;
            cmd.Parameters.Add("@NextAttemptDate", SqlDbType.DateTime).Value = nextAttemptDate;
            cmd.Parameters.Add("@Priority", SqlDbType.Int).Value = priority;
            cmd.Parameters.Add("@User", SqlDbType.NVarChar).Value = User;

            ReturnData rt = DialerDB.ExecNonQuery(cmd);
            return rt;
            /*[dbo].[sp_DM_UpdateDCR_SetStatus_2]
	                @RecID		int,
	                @TryID		int,
	                @CallStatus	int,
	                @NextAttemptDate	datetime,
	                @Priority	int,
	                @User	nvarchar (36)*/
        }
        
        public ReturnData UpdateDCR_release(int RecordID, string User ) 
        {
            MccMLogger.Debug("  >DM::UpdateDCR_release. recordID = " + RecordID.ToString()) ;
            string SqlCmd = @"[dbo].[sp_DM_UpdateDCR_Release]";
            if (!CheckConnectionObject(DialerDB))
            {
                return BuildError("DB handler is null");
            }
            //switch(CampType)
            //{
            //    case CampaignType.Preview:
            //        SqlCmd =  @"[dbo].[sp_DM_UpdateDCR_Release]";
            //        break;
            //    case CampaignType.Callback:
            //        SqlCmd =  @"[dbo].[sp_DM_UpdateDCR_Release_CB]";
            //        break;
            //    default:
            //        SqlCmd =  @"[dbo].[sp_DM_UpdateDCR_Release]";
            //        break;
            //}
            
            SqlCommand cmd = new SqlCommand(SqlCmd);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@RecID", SqlDbType.Int).Value = RecordID;
            cmd.Parameters.Add("@User", SqlDbType.NVarChar).Value = User;

            ReturnData rt = DialerDB.ExecNonQuery(cmd);
            return rt;
            /*[dbo].[sp_DM_UpdateDCR_Release]
	                @RecID		int,
	                @User	nvarchar (36)*/
        }


        public ReturnData UpdateDCR_CloseSP(int RecordID, string User ) 
        {

            MccMLogger.Debug("  >DM::UpdateDCR_CloseSP. recordID = " + RecordID.ToString()) ;
            string SqlCmd = @"[dbo].[sp_DM_UpdateDCR_CloseSP]";
            if (!CheckConnectionObject(DialerDB))
            {
                return BuildError("DB handler is null");
            }
            SqlCommand cmd = new SqlCommand(SqlCmd);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@RecID", SqlDbType.Int).Value = RecordID;
            cmd.Parameters.Add("@User", SqlDbType.NVarChar).Value = User;
            ReturnData rt = DialerDB.ExecNonQuery(cmd);
            return rt;
            /*[dbo].[p_DM_UpdateDCR_CloseSP]]
	                @RecID		int,
	                @User	nvarchar (36)*/
        }



            
        private ReturnData _UpdateDialingRecord(int RecordID, string PhoneNumbr, DateTime CallDate, int CallStatus, int TryID, string Comments, int RecordStatus, int RecordStatusReason, DateTime NextAttemptDate, string User)
        {
            if (!CheckConnectionObject(DialerDB))
            {
                return BuildError("DB handler is null");
            }
            if (string.IsNullOrWhiteSpace(User))
            {
                User = auditUserName; 
            }
            if (NextAttemptDate == DateTime.MinValue)
            {
                NextAttemptDate = DateTime.Now.AddDays(-2);
            }
            
            string SqlCmd = @"[dbo].[sp_DM_UpdateDCRecord_1]";
            SqlCommand cmd = new SqlCommand(SqlCmd);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@RecID", SqlDbType.Int).Value = RecordID;
            cmd.Parameters.Add("@PhoneNum", SqlDbType.NVarChar).Value = PhoneNumbr;
            cmd.Parameters.Add("@CallDate", SqlDbType.DateTime).Value = CallDate;
            cmd.Parameters.Add("@CallStatus", SqlDbType.Int).Value = CallStatus;
            cmd.Parameters.Add("@TryID", SqlDbType.Int).Value = TryID;
            cmd.Parameters.Add("@Comments", SqlDbType.NVarChar).Value = Comments;
            cmd.Parameters.Add("@RecordStatus", SqlDbType.Int).Value = RecordStatus;
            cmd.Parameters.Add("@RecordStatusReason", SqlDbType.Int).Value = RecordStatusReason;
            cmd.Parameters.Add("@NextAttemptDate", SqlDbType.DateTime).Value = NextAttemptDate;
            cmd.Parameters.Add("@User", SqlDbType.NVarChar).Value = GetUser(User);
            
            ReturnData rt = DialerDB.ExecNonQuery(cmd);
            return rt;
            /*[dbo].[UpdateDCRecord_1]
            @RecID		int,
            @PhoneNum	nvarchar(12),
            @CallDate	datetime,
            @CallStatus	int,
            @TryID		int,
            @Comments nvarchar(max),
            @RecordStatus int,
            @RecordStatusReason int,
            @NextAttemptDate Datetime,         
            @User nvarchar (36)*/
        }

        #endregion DialingRecord

        #region AgentActivity
        public bool find_agentprogressive(string AgentLoginID)
        {
            int _rc=0;
            MccMLogger.Debug("  >DM::is_progressive. AgentLoginID = " + AgentLoginID);
            if (!CheckConnectionObject(DialerDB))
            {
                return false;
            }
          
            string cmndfunc = " select dbo.fn_isProgressive(@AgentID)";
            string errMsg = "";
            _rc = DialerDB.Executefunction(cmndfunc, AgentLoginID, errMsg);
            return (_rc==1);
        }
        public ReturnData UpdateAgentState(string AgentLoginID, DateTime StateTime, string State, string OldState, string StateDetails, string Phone, string Skill, string Application, string CallID,string extDN, string agtName)
        {

            MccMLogger.Debug("  >DM::UpdateAgentState. AgentLoginID = " + AgentLoginID + " State = " + State + " details = " + StateDetails);
            if (!CheckConnectionObject(DialerDB))
            {
                return BuildError("DB handler is null");
            }
            string SqlCmd = @"[dbo].[sp_rt_UpdateSPState]";
            SqlCommand cmd = new SqlCommand(SqlCmd);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@startEventTime", SqlDbType.DateTime).Value = StateTime;
            cmd.Parameters.Add("@agentID", SqlDbType.NVarChar).Value = AgentLoginID;
            cmd.Parameters.Add("@extDN", SqlDbType.NVarChar).Value = extDN; 
             cmd.Parameters.Add("@agtName", SqlDbType.NVarChar).Value = agtName;
            cmd.Parameters.Add("@eventType", SqlDbType.NVarChar).Value = State;
            cmd.Parameters.Add("@subEventType", SqlDbType.NVarChar).Value = StateDetails;
            cmd.Parameters.Add("@callID", SqlDbType.NVarChar).Value = CallID;
            cmd.Parameters.Add("@phoneDN", SqlDbType.NVarChar).Value = Phone;
            cmd.Parameters.Add("@application", SqlDbType.NVarChar).Value = Application;
            cmd.Parameters.Add("@skill", SqlDbType.NVarChar).Value = Skill;
            cmd.Parameters.Add("@lastEventType", SqlDbType.NVarChar).Value = OldState;
            //cmd.Parameters.Add("@startEventTime", SqlDbType.DateTime).Value = StateTime;
            ReturnData rt = DialerDB.ExecNonQuery(cmd);
            return rt;
            /* [dbo].[sp_rt_UpdateSPState]
	            @AgentLoginID			varchar(50)
	            ,@StateTime				datetime
	            ,@State					varchar(50)
	            ,@OldState				varchar(50)
	            ,@StateDetails			varchar(100)
	            ,@Phone					varchar(50)
	            ,@Skill					varchar(50)
	            ,@Application			varchar(50)
	            ,@CallID				int
      @startEventTime		datetime,
	        @agentID		nvarchar(32),
	        @extDN			nvarchar(10),
	        @agtName		nvarchar(50),
	        @eventType		nvarchar(10),
            @subEventType	nvarchar(10),
            @callID			nvarchar(50),
            @phoneDN		nvarchar (32),
            @application	nvarchar (32),
			@skill			nvarchar (32),
			@lastEventType	nvarchar (32),
			--@endEventTime	Time

             */
        }
        #endregion AgentActivity

        #region DialingProccess

        /* Procedure: GetRecordsToCall_CP
         * get one dialing record from : dbo.DialerCustomers
         * Input: int[]  campaignID, 
         *  ----   int  NumberOfRecords
         *  string User 
         * Stored procedure: Dialer DB -> [dbo].[sp_CP_GetRecordsToCall] via dbo.fnSelectRecords
         * Return datatable: Record of dbo.DialerCustomers table
         */
        public ReturnData GetRecordsToCall_CP(int[] campaignID, /*int  NumberOfRecords,*/ string User = "")
        {
            string CampaignList = string.Join(",", campaignID);
            MccMLogger.Debug("  >DM::GetRecordsToCall_CP. RecID = " + CampaignList);
            if (!CheckConnectionObject(DialerDB))
            {
                return BuildError("DB handler is null");
            }
            
            string SqlCmd = @"[dbo].[sp_CP_GetRecordsToCall]";
            SqlCommand cmd = new SqlCommand(SqlCmd);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Campaign_Code", SqlDbType.VarChar).Value = CampaignList;
            //cmd.Parameters.Add("@NumberOfRecords", SqlDbType.Int).Value = NumberOfRecords;
            cmd.Parameters.Add("@User", SqlDbType.NVarChar).Value = GetUser(User);
            
            ReturnData rt = DialerDB.ExecQuery(cmd);
            return rt;
            /*[dbo].[sp_CP_GetRecordsToCall]
            @Campaign_Code		int,
            @NumberOfRecords	int,
            @User				nvarchar(36)*/
        }

        public ReturnData GetRecordsToCall_ca(int[] campaignID, /*int  NumberOfRecords,*/ string User = "")
        {
            string CampaignList = string.Join(",", campaignID);
            string SqlCmd = @"[dbo].[sp_Moma_GetRecordsToCall]";
            MccMLogger.Debug("  >DM::GetRecordsToCall. campaignID = " + CampaignList);
            if (!CheckConnectionObject(DialerDB))
            {
                return BuildError("DB handler is null");
            }
            //switch(CampType)
            //{
            //    case CampaignType.Preview:
            //        SqlCmd = @"[dbo].[sp_CP_GetRecordsToCall]";
            //        break;
            //    case CampaignType.Callback:
            //        SqlCmd = @"[dbo].[sp_CB_GetRecordsToCall]"; 
            //        break;
            //    default:
            //        SqlCmd = @"[dbo].[sp_CP_GetRecordsToCall]";
            //        break;
            //}
            SqlCommand cmd = new SqlCommand(SqlCmd);
            /*
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Campaign_Code", SqlDbType.VarChar).Value = CampaignList;
            //cmd.Parameters.Add("@NumberOfRecords", SqlDbType.Int).Value = NumberOfRecords;
            cmd.Parameters.Add("@User", SqlDbType.NVarChar).Value = GetUser(User);
            */
            ReturnData rt = DialerDB.ExecQuery(cmd);
            return rt;
            /* [dbo].[sp_CP_GetRecordsToCall]
            @Campaign_Code		int,
            @NumberOfRecords	int,
            @User				nvarchar(36)*/
        }
        public ReturnData GetPrivateRecords(string User)
        {
            string SqlCmd = @"[dbo].[sp_DM_GetPrivateRecords]";
            MccMLogger.Debug("  >DM::GetRecordsToCall. User = " + User);
            if (!CheckConnectionObject(DialerDB))
            {
                return BuildError("DB handler is null");
            }
            //switch(CampType)
            //{
            //    case CampaignType.Preview:
            //        SqlCmd = @"[dbo].[sp_CP_GetRecordsToCall]";
            //        break;
            //    case CampaignType.Callback:
            //        SqlCmd = @"[dbo].[sp_CB_GetRecordsToCall]"; 
            //        break;
            //    default:
            //        SqlCmd = @"[dbo].[sp_CP_GetRecordsToCall]";
            //        break;
            //}
            SqlCommand cmd = new SqlCommand(SqlCmd);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@User", SqlDbType.NVarChar).Value = GetUser(User);

            ReturnData rt = DialerDB.ExecQuery(cmd);
            return rt;
        }



        /* Procedure: GetecordDataByFields
         * get data of specefic dialing record from : dbo.DialerCustomers
         * Input: int RecId, 
         * string[] fields list
         * Stored procedure: sql query
         * Return datatable: partial Record of dbo.DialerCustomers table
         */
        public ReturnData GetRcordDataByFields(int RecId, string fieldList)
        {
            string fromStatement;
            MccMLogger.Debug("  >DM::GetecordDataByFields. recordID = " + RecId.ToString());
            if (!CheckConnectionObject(DialerDB))
            {
                return BuildError("DB handler is null");
            }
           //switch(CampType)
           // {
           //     case CampaignType.Preview:
           //         fromStatement = " FROM [dbo].[DialerCustomers] ";
           //         break;
           //     case CampaignType.Callback:
           //         fromStatement = " FROM [dbo].[CallbackCustomers] ";
           //         break;
           //     default:
           //         fromStatement = " FROM [dbo].[DialerCustomers] ";
           //         break;
           // }
            //string SqlCmd = @"SELECT [RecId],[Campaign_Code],[RecNum],[Priority],[Comments_1],[Retries] ";

            fromStatement = " FROM [dbo].[DialerCustomers] ";
            string SqlCmd = @"SELECT [RecId] ";
            if(!string.IsNullOrWhiteSpace(fieldList))
            {
                //SqlCmd += string.Join(",", fieldList);
                SqlCmd += fieldList;
            }
            SqlCmd += Environment.NewLine + fromStatement ;//" FROM [dbo].[DialerCustomers] ";
            SqlCmd += Environment.NewLine + " WHERE [RecId] = '" + RecId.ToString()+ "' ";


            SqlCommand cmd = new SqlCommand(SqlCmd);
            cmd.CommandType = CommandType.Text;

            
            ReturnData rt = DialerDB.ExecQuery(cmd);
            return rt;
            
        }

        public ReturnData GetRcordData(int RecId, string fieldList)
        {
            MccMLogger.Debug("  >DM::GetRcordData. recordID = " + RecId.ToString() + ". customer fields = " + fieldList);
            string fromStatement;
            if (!CheckConnectionObject(DialerDB))
            {
                return BuildError("DB handler is null");
            }
            fromStatement = " FROM [dbo].[DialerCustomers] ";

            //switch(CampType)
            //{
            //    case CampaignType.Preview:
            //        fromStatement = " FROM [dbo].[DialerCustomers] ";
            //        break;
            //    case CampaignType.Callback:
            //        fromStatement = " FROM [dbo].[CallbackCustomers] ";
            //        break;
            //    default:
            //        fromStatement = " FROM [dbo].[DialerCustomers] ";
            //        break;
            //}
            string SqlCmd = @"SELECT [RecId], [Comments_1], " + 
                            "[PhoneNum_1], [PhoneType_1],[PhoneNum_2],[PhoneType_2],[PhoneNum_3],[PhoneType_3], " +
                            "[PhoneNum_4],[PhoneType_4],[PhoneNum_5],[PhoneType_5],[PhoneNum_6],[PhoneType_6],[PhoneNum_7],[PhoneType_7], " +
                            "[PhoneNum_8],[PhoneType_8], [PhoneNum_9],[PhoneType_9],[PhoneNum_10],[PhoneType_10] ";
            if(!string.IsNullOrWhiteSpace(fieldList))
            {
                SqlCmd += fieldList;
            }
            SqlCmd += Environment.NewLine + fromStatement;
            SqlCmd += Environment.NewLine + " WHERE [RecId] = '" + RecId.ToString()+ "' ";

            SqlCommand cmd = new SqlCommand(SqlCmd);
            cmd.CommandType = CommandType.Text;
            
            ReturnData rt = DialerDB.ExecQuery(cmd);
             // check ret value
            if (rt.StatusInfo.RowsCount != 1) // no data
            {
            if (!string.IsNullOrWhiteSpace(rt.StatusInfo.Description))
                return BuildError(rt.StatusInfo.Description);
                return BuildError("Data not found");
            }
            ReturnData new_rt = getRcordData_Parsing( rt);
            return new_rt;
        }
  
        private ReturnData getRcordData_Parsing(ReturnData rt)
        {
            if (rt == null)
                 return BuildError("Data object is null");

            string phoneNumList = "";
            string PhoneTypeList = "";
            string attachedDtataList = "";
            string comment = rt.DataInfo.GetField("Comments_1");

            for (int i = 2; i < 22; i += 2 )
            {
                string _phone = rt.DataInfo.GetField(i);
                string _type = rt.DataInfo.GetField(i+1);
                if(!string.IsNullOrWhiteSpace(_phone))
                {
                    if (string.IsNullOrWhiteSpace(phoneNumList)) // first element
                    {
                        phoneNumList += _phone;
                        PhoneTypeList += _type;
                    }
                    else
                    {
                        phoneNumList += "," + _phone;
                        PhoneTypeList += "," +_type;
                    }
                }
            }
            int columnCount = rt.DataInfo.Data.Columns.Count;
            for (int i = 22; i < columnCount; i ++)
            {
                string _data = rt.DataInfo.GetField(i);
                if (string.IsNullOrWhiteSpace(attachedDtataList)) // first element
                {
                    attachedDtataList += _data;
                }
                else
                {
                    attachedDtataList +=  "," +_data;
                }
            }
            // generate data table
            DataTable dt = new DataTable("Result");
            dt.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("Phoneslist", typeof(string)),
                new DataColumn("PhoneTypeList", typeof(string)),
                new DataColumn("DataValuesList", typeof(string)),
                new DataColumn("Comments", typeof(string))
            });
            dt.Rows.Add(new object[]
            {
                phoneNumList,
                PhoneTypeList,
                attachedDtataList,
                comment
            });

            StatusObj status = new StatusObj(ActivityStatus.Success,DateTime.Now,1,"");
            ReturnData rt_new = new ReturnData(status, new DataObj(dt));
            return rt_new;
        
        }
         
        /* Procedure: GetCallStatuses_CP
         * get list of Aavailable statuses for a call  from : dbo.CallStatuses, to: Softphone
         
         * Input: int  campaignID, 
         * Stored procedure: Dialer DB -> [dbo].[sp_DM_GetCallStatusList]
         * Return datatable: Record of dbo.[CallStatuses]  table that mutch condition
         */
        public ReturnData GetCallStatuses_CP(int campaignID)
        {
            
            MccMLogger.Debug("  >DM::GetCallStatuses_CP. campaignID = " + campaignID.ToString());
            if (!CheckConnectionObject(DialerDB))
            {
                return BuildError("DB handler is null");
            }
            
            string SqlCmd = @"[dbo].[sp_DM_GetCallStatusList]";
            SqlCommand cmd = new SqlCommand(SqlCmd);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@CampaignID", SqlDbType.Int).Value = campaignID;
          
            ReturnData rt = DialerDB.ExecQuery(cmd);
            return rt;
            /* [dbo].[sp_DM_GetCallStatusList]
	            @CampaignID	int*/
        }

        /* Procedure:GetWShDataMapping
         * get fields: Pop, Check for WS activation before making call
         * Input: int  campaignID, 
         * Stored procedure: Dialer DB -> [dbo].[sp_DM_GetWSDataMapping]
         * Return datatable: mapping Records from [dbo].[CustomFieldsMappings] table that mutch campaign and contain attach data only (custom field 1-20)
         */
        public ReturnData GetWShDataMapping(int campaignID)
        {
            
            MccMLogger.Debug("  >DM::GetWShDataMapping. campaignID = " + campaignID.ToString());
            if (!CheckConnectionObject(DialerDB))
            {
                return BuildError("DB handler is null");
            }
            
            string SqlCmd = @"[dbo].[sp_DM_GetWSDataMapping]";
            SqlCommand cmd = new SqlCommand(SqlCmd);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Campaign_Code", SqlDbType.Int).Value = campaignID;
          
            ReturnData rt = DialerDB.ExecQuery(cmd);
            return rt;
            /* [dbo].[sp_DM_GetWSDataMapping]
	                @Campaign_Code int*/
        }

         /* Procedure: GetAttachDataMapping
         * get list of attached data field (display order, display id, source data field)l  from : dbo.CustomFieldsMappings, to: Softphone
         
         * Input: int  campaignID, 
         * Stored procedure: Dialer DB -> [dbo].[sp_DM_GetAttachDataMapping]
         * Return datatable: mapping Records from [dbo].[CustomFieldsMappings] table that mutch campaign and contain attach data only (custom field 1-20)
         */
        public ReturnData GetAttachDataMapping(int campaignID)
        {
            
            MccMLogger.Debug("  >DM::GetAttachDataMapping. campaignID = " + campaignID.ToString());
            if (!CheckConnectionObject(DialerDB))
            {
                return BuildError("DB handler is null");
            }
            
            string SqlCmd = @"[dbo].[sp_DM_GetAttachDataMapping]";
            SqlCommand cmd = new SqlCommand(SqlCmd);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Campaign_Code", SqlDbType.Int).Value = campaignID;
          
            ReturnData rt = DialerDB.ExecQuery(cmd);
            return rt;
            /* [dbo].[sp_DM_GetAttachDataMapping]
	                @Campaign_Code int*/
        }
        #endregion DialingProccess
        
        #region Evenst_online
        
        /* Procedure: LogEvent
        * Update entire record at: dbo.DialerCustomers
        * Input: @TimeStamp datetime
        EventSource nvarchar(32)
        SourceType int
        EventTypeID int
        EventData nvarchar(50)
        Call_Internal_CallID nvarchar(36)
        Call_External_CallID nvarchar(36)
        Call_Direction int
        Call_CallType int
        Call_CLI nvarchar(12)
        Call_DNIS nvarchar(12)
        Call_Application nvarchar(100)
        Call_Skill nvarchar(100)
        Call_Data1 nvarchar(250)
        Call_Data2 nvarchar(250)
        Call_Data3 nvarchar(max)
        * Stored procedure: Dialer DB -> [dbo].[sp_DM_LogEvent] 
        * Return datatable: NONE
        */
        public ReturnData LogEvent(DateTime TimeStamp, string EventSource, int SourceType, int EventTypeID, string EventData, string Call_Internal_CallID, string Call_External_CallID, int Call_Direction, int Call_CallType, string Call_CLI, string Call_DNIS, string Call_Application, string Call_Skill, string Call_Data1, string Call_Data2, string Call_Data3)
        {
            MccMLogger.Debug("  >DM::LogEvent. Source = " + EventSource + ". Event = " + EventTypeID.ToString());
            if (!CheckConnectionObject(DialerDB))
            {
                return BuildError("DB handler is null");
            }
            
            string SqlCmd = @"[dbo].[sp_DM_LogEvent]";
            SqlCommand cmd = new SqlCommand(SqlCmd);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@TimeStamp", SqlDbType.DateTime).Value = TimeStamp;
            cmd.Parameters.Add("@EventSource", SqlDbType.NVarChar).Value = EventSource;
            cmd.Parameters.Add("@SourceType", SqlDbType.Int).Value = SourceType;
            cmd.Parameters.Add("@EventTypeID", SqlDbType.Int).Value = EventTypeID;
            cmd.Parameters.Add("@EventData", SqlDbType.NVarChar).Value = EventData;
            cmd.Parameters.Add("@Call_Internal_CallID", SqlDbType.NVarChar).Value = Call_Internal_CallID;
            cmd.Parameters.Add("@Call_External_CallID", SqlDbType.NVarChar).Value = Call_External_CallID;
            cmd.Parameters.Add("@Call_Direction", SqlDbType.Int).Value = Call_Direction;
            cmd.Parameters.Add("@Call_CallType", SqlDbType.Int).Value = Call_CallType;
            cmd.Parameters.Add("@Call_CLI", SqlDbType.NVarChar).Value = Call_CLI;
            cmd.Parameters.Add("@Call_DNIS", SqlDbType.NVarChar).Value = Call_DNIS;
            cmd.Parameters.Add("@Call_Application", SqlDbType.NVarChar).Value = Call_Application;
            cmd.Parameters.Add("@Call_Skill", SqlDbType.NVarChar).Value = Call_Skill;
            cmd.Parameters.Add("@Call_Data1", SqlDbType.NVarChar).Value = Call_Data1;
            cmd.Parameters.Add("@Call_Data2", SqlDbType.NVarChar).Value = Call_Data2 ;
            cmd.Parameters.Add("@Call_Data3", SqlDbType.NVarChar).Value = Call_Data3;
            
            return DialerDB.ExecNonQuery(cmd);
            /* sp_DM_LogEvent
            @TimeStamp datetime
            ,@EventSource nvarchar(32)
            ,@SourceType int
            ,@EventTypeID int
            ,@EventData nvarchar(50)
            ,@Call_Internal_CallID nvarchar(36)
            ,@Call_External_CallID nvarchar(36)
            ,@Call_Direction int
            ,@Call_CallType int
            ,@Call_CLI nvarchar(12)
            ,@Call_DNIS nvarchar(12)
            ,@Call_Application nvarchar(100)
            ,@Call_Skill nvarchar(100)
            ,@Call_Data1 nvarchar(250)
            ,@Call_Data2 nvarchar(250)
            ,@Call_Data3 nvarchar(max)*/
        }
        
        /* Procedure: LogEvent
        * Update entire record at: dbo.DialerCustomers
        * Input: @TimeStamp datetime
        EventSource nvarchar(32)
        SourceType int
        EventTypeID int
        EventData nvarchar(50)
        Call_Data1 nvarchar(250)
        
        * Stored procedure: Dialer DB -> [dbo].[sp_DM_LogEvent] 
        * Return datatable: NONE
        */
        public ReturnData LogEvent(DateTime TimeStamp, string EventSource, int SourceType, int EventTypeID, string EventData, string Call_Data1)
        {
            return LogEvent(TimeStamp, EventSource, SourceType, EventTypeID, EventData, "0", "0", 0, 0, "", "", "", "", Call_Data1, "", "");
        }
        
        /* Procedure: LoginOnlineAgent
        * Create new entry at: [OnlineAgents] table
        * Input: @AgentLoginID	int,
        @AgentID		nvarchar(20),
        @AgentName		nvarchar(100),
        @ExtensionDN	nvarchar(10),
        @PossitionDN	nvarchar(10),
        @LoginTime		datetime,
        @Station		nvarchar(50),
        @DepartmentID	int,
        @ExtensionState	int,
        @AgentStatus	int,
        @AgentSubStatus	int
        
        * Stored procedure: Dialer DB -> [dbo].[sp_DM_LoginOnlineAgent] 
        * Return datatable: NONE
        */
        public ReturnData LoginOnlineAgent(string AgentLoginID,  string AgentName, string ExtensionDN, string PossitionDN, string Station, int DepartmentID, int ExtensionState, int AgentStatus, int @AgentSubStatus)
        {
            MccMLogger.Debug("  >DM::LoginOnlineAgent. Source = " + AgentLoginID + " (" + AgentName + ")");
            if (!CheckConnectionObject(DialerDB))
            {
                return BuildError("DB handler is null");
            }
            
            string SqlCmd = @"[dbo].[sp_DM_LoginOnlineAgent]";
            SqlCommand cmd = new SqlCommand(SqlCmd);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@AgentLoginID", SqlDbType.NVarChar).Value = AgentLoginID;
            cmd.Parameters.Add("@AgentName", SqlDbType.NVarChar).Value = AgentName;
            cmd.Parameters.Add("@ExtensionDN", SqlDbType.NVarChar).Value = ExtensionDN;
            cmd.Parameters.Add("@PossitionDN", SqlDbType.NVarChar).Value = PossitionDN;
            cmd.Parameters.Add("@LoginTime", SqlDbType.DateTime).Value = DateTime.Now;
            cmd.Parameters.Add("@Station", SqlDbType.NVarChar).Value = Station;
            cmd.Parameters.Add("@DepartmentID", SqlDbType.Int).Value = DepartmentID;
            cmd.Parameters.Add("@ExtensionState", SqlDbType.Int).Value = ExtensionState;
            cmd.Parameters.Add("@AgentStatus", SqlDbType.Int).Value = AgentStatus;
            cmd.Parameters.Add("@AgentSubStatus", SqlDbType.Int).Value = AgentSubStatus;
            
            return DialerDB.ExecNonQuery(cmd);
            /* [dbo].[sp_DM_LoginOnlineAgent]
	                @AgentLoginID	nvarchar(20),
	                @AgentName		nvarchar(100),
	                @ExtensionDN	nvarchar(10),
	                @PossitionDN	nvarchar(10),
	                @LoginTime		datetime,
	                @Station		nvarchar(50),
	                @DepartmentID	int,
	                @ExtensionState	int,
	                @AgentStatus	int,
	                @AgentSubStatus	int*/
        }

        /* Update entry at: [OnlineAgents] table
        * Input:@AgentLoginID			nvarchar(20),
		    @ExtensionState			int,
		    @AgentStatus			int,
		    @AgentSubStatus			int,
		
		    @ACDCall_CallStartTime	datetime,
		    @ACDCall_Direction		int,
		    @ACDCall_CallType		int ,
		    @ACDCall_PhoneNumber	nvarchar(12),
		    @ACDCall_Application	nvarchar(100),
		    @ACDCall_Skill			nvarchar(100),
		    @ACDCall_Data			nvarchar(100),
		    @ACDCall_Internal_CallID nvarchar(36) ,
		    @ACDCall_External_CallID nvarchar(36) ,
		    @DataField_1			nvarchar(max)
        
        * Stored procedure: Dialer DB -> [dbo].[sp_DM_UpdateOnlineAgent_StatrtCall]
        * Return datatable: NONE
        */
        public ReturnData UpdateOnlineAgentStart_Call(string AgentLoginID, int ExtensionState, int AgentStatus, int AgentSubStatus, int ACDCall_Direction,int ACDCall_CallType, string ACDCall_PhoneNumber, string ACDCall_Application, string ACDCall_Skill, string ACDCall_Data,string ACDCall_Internal_CallID, string ACDCall_External_CallID,string  DataField_1 = ""  )
        {
            MccMLogger.Debug("  >DM::UpdateOnlineAgentStart_Call. Source = " + AgentLoginID + ". Phone number = " + ACDCall_PhoneNumber);
            if (!CheckConnectionObject(DialerDB))
            {
                return BuildError("DB handler is null");
            }
            
            string SqlCmd = @"[dbo].[sp_DM_UpdateOnlineAgent_StatrtCall]";
            SqlCommand cmd = new SqlCommand(SqlCmd);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@AgentLoginID", SqlDbType.NVarChar).Value = AgentLoginID;
            cmd.Parameters.Add("@ExtensionState", SqlDbType.Int).Value = ExtensionState;
            cmd.Parameters.Add("@AgentStatus", SqlDbType.Int).Value = AgentStatus;
            cmd.Parameters.Add("@AgentSubStatus", SqlDbType.Int).Value = AgentSubStatus;

            cmd.Parameters.Add("@ACDCall_CallStartTime", SqlDbType.DateTime).Value = DateTime.Now;
            cmd.Parameters.Add("@ACDCall_Direction", SqlDbType.Int).Value = ACDCall_Direction;
            cmd.Parameters.Add("@ACDCall_CallType", SqlDbType.Int).Value = ACDCall_CallType;
            cmd.Parameters.Add("@ACDCall_PhoneNumber", SqlDbType.NVarChar).Value = ACDCall_PhoneNumber;
            cmd.Parameters.Add("@ACDCall_Application", SqlDbType.NVarChar).Value = ACDCall_Application;
            cmd.Parameters.Add("@ACDCall_Skill", SqlDbType.NVarChar).Value = ACDCall_Skill;
            cmd.Parameters.Add("@ACDCall_Data", SqlDbType.NVarChar).Value = ACDCall_Data;
            cmd.Parameters.Add("@ACDCall_Internal_CallID", SqlDbType.NVarChar).Value = ACDCall_Internal_CallID;
            cmd.Parameters.Add("@ACDCall_External_CallID", SqlDbType.NVarChar).Value = ACDCall_External_CallID;
            cmd.Parameters.Add("@DataField_1", SqlDbType.NVarChar).Value = DataField_1;
            
            return DialerDB.ExecNonQuery(cmd);
            /* [dbo].[sp_DM_UpdateOnlineAgent_StatrtCall]
		            @AgentLoginID			nvarchar(20),
		            @ExtensionState			int,
		            @AgentStatus			int,
		            @AgentSubStatus			int,
		
		            @ACDCall_CallStartTime	datetime,
		            @ACDCall_Direction		int,
		            @ACDCall_CallType		int ,
		            @ACDCall_PhoneNumber	nvarchar(12),
		            @ACDCall_Application	nvarchar(100),
		            @ACDCall_Skill			nvarchar(100),
		            @ACDCall_Data			nvarchar(100),
		            @ACDCall_Internal_CallID nvarchar(36) ,
		            @ACDCall_External_CallID nvarchar(36) ,
		            @DataField_1			nvarchar(max) /*optional data field*/
        }


        /* Update entry at: [OnlineAgents] table
        * Input:@AgentLoginID			nvarchar(20),
		    @ExtensionState			int,
		    @AgentStatus			int,
		    @AgentSubStatus			int,
       
        * Stored procedure: Dialer DB -> [dbo].[sp_DM_UpdateOnlineAgent_EndCall]
        * Return datatable: NONE
        */
        public ReturnData UpdateOnlineAgent_EndCall(string AgentLoginID, int ExtensionState, int AgentStatus, int AgentSubStatus )
        {
            MccMLogger.Debug("  >DM::UpdateOnlineAgent_EndCall. status = " + ExtensionState.ToString() + " / " + AgentStatus.ToString());
            if (!CheckConnectionObject(DialerDB))
            {
                return BuildError("DB handler is null");
            }
            
            string SqlCmd = @"[dbo].[sp_DM_UpdateOnlineAgent_EndCall]";
            SqlCommand cmd = new SqlCommand(SqlCmd);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@AgentLoginID", SqlDbType.NVarChar).Value = AgentLoginID;
            cmd.Parameters.Add("@ExtensionState", SqlDbType.Int).Value = ExtensionState;
            cmd.Parameters.Add("@AgentStatus", SqlDbType.Int).Value = AgentStatus;
            cmd.Parameters.Add("@AgentSubStatus", SqlDbType.Int).Value = AgentSubStatus;
           
            return DialerDB.ExecNonQuery(cmd);
            /* [dbo].[sp_DM_UpdateOnlineAgent_EndCall]
		            @AgentLoginID			nvarchar(20),
		            @ExtensionState			int,
		            @AgentStatus			int,
		            @AgentSubStatus			int*/
        }

        /* Update entry at: [OnlineAgents] table
        * Input:@AgentLoginID			nvarchar(20),
		    @ExtensionState			int,
		    @AgentStatus			int,
		    @AgentSubStatus			int,
       
        * Stored procedure: Dialer DB -> [dbo].[sp_DM_UpdateOnlineAgent_Status]
        * Return datatable: NONE
        */
        public ReturnData UpdateOnlineAgent_Status(string AgentLoginID, int ExtensionState, int AgentStatus, int AgentSubStatus )
        {
            MccMLogger.Debug("  >DM::UpdateOnlineAgent_Status. status = " + ExtensionState.ToString() + " / " + AgentStatus.ToString());
            if (!CheckConnectionObject(DialerDB))
            {
                return BuildError("DB handler is null");
            }
            
            string SqlCmd = @"[dbo].[sp_DM_UpdateOnlineAgent_Status]";
            SqlCommand cmd = new SqlCommand(SqlCmd);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@AgentLoginID", SqlDbType.NVarChar).Value = AgentLoginID;
            cmd.Parameters.Add("@ExtensionState", SqlDbType.Int).Value = ExtensionState;
            cmd.Parameters.Add("@AgentStatus", SqlDbType.Int).Value = AgentStatus;
            cmd.Parameters.Add("@AgentSubStatus", SqlDbType.Int).Value = AgentSubStatus;
           
            return DialerDB.ExecNonQuery(cmd);
            /* [dbo].[sp_DM_UpdateOnlineAgent_Status]
		            @AgentLoginID			nvarchar(20),
		            @ExtensionState			int,
		            @AgentStatus			int,
		            @AgentSubStatus			int*/
        }









        
        /* Procedure: LoginOnlineResource
        * Create new entry at: [OnlineResources] table
        * Input:@DialingResource	nvarchar(32),
           @Type				int,
           @Station			nvarchar(50),
           @LoginTime			datetime,
           @Status				int,
        * Stored procedure: Dialer DB -> [dbo].[sp_DM_LoginOnlineResource] 
        * Return datatable: NONE
        */
        public ReturnData LoginOnlineResource(string DialingResource, int Type, string Station, DateTime LoginTime, int Status)
        {
            MccMLogger.Debug("  >DM::LoginOnlineResource. Source = " + DialingResource);
            if (!CheckConnectionObject(DialerDB))
            {
                return BuildError("DB handler is null");
            }
            
            string SqlCmd = @"[dbo].[sp_DM_LoginOnlineResource]";
            SqlCommand cmd = new SqlCommand(SqlCmd);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@DialingResource", SqlDbType.NVarChar).Value = DialingResource;
            cmd.Parameters.Add("@Type", SqlDbType.Int).Value = Type;
            cmd.Parameters.Add("@Station", SqlDbType.NVarChar).Value = Station;
            cmd.Parameters.Add("@LoginTime", SqlDbType.DateTime).Value = LoginTime;
            cmd.Parameters.Add("@Status", SqlDbType.Int).Value = Status;
            
            return DialerDB.ExecNonQuery(cmd);
            /* [dbo].[sp_DM_LoginOnlineResource]
            @DialingResource	nvarchar(32),
            @Type				int,
            @Station			nvarchar(50),
            @LoginTime			datetime,
            @Status				int,
            */
        }
        
        /* Procedure: UpdateOnlineResource
        * Update entry at: [OnlineResources] table. incluse call details (start call)
        * Input: @DialingResource nvarchar(32),
        @Status int,
        @Call_Direction		int,
        @Call_CLI			nvarchar(16),
        @Call_DNIS			nvarchar(16),
        @Call_Internal_CallID	nvarchar(36),
        @Call_External_CallID	nvarchar(36),
        @Call_StartTime		datetime,
        @Call_ConnectTime	datetime,
        @Call_End			datetime,
        @Call_Application	nvarchar(100),
        @Call_Data			nvarchar(100),
        @DataField_1		nvarchar(max),
        @DataField_2		nvarchar(max)
        * Stored procedure: Dialer DB -> [dbo].[sp_DM_UpdateOnlineResource] 
        * Return datatable: NONE
        */
        public ReturnData UpdateOnlineResource(string DialingResource, int Status, int Call_Direction, string Call_CLI, string Call_DNIS, string Call_Internal_CallID, string Call_External_CallID, DateTime Call_StartTime, DateTime Call_ConnectTime, DateTime Call_End, string Call_Application, string Call_Data, string DataField_1, string DataField_2)      
        {
            MccMLogger.Debug("  >DM::UpdateOnlineResource (Start Call) . Source = " + DialingResource + ". Status = " + Status.ToString());
            if (!CheckConnectionObject(DialerDB))
            {
                return BuildError("DB handler is null");
            }
            
            string SqlCmd = @"[dbo].[sp_DM_UpdateOnlineResource]";
            SqlCommand cmd = new SqlCommand(SqlCmd);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@DialingResource", SqlDbType.NVarChar).Value = DialingResource;
            cmd.Parameters.Add("@Status", SqlDbType.Int).Value = Status;
            cmd.Parameters.Add("@Call_Direction", SqlDbType.Int).Value = Call_Direction;
            cmd.Parameters.Add("@Call_CLI", SqlDbType.NVarChar).Value = Call_CLI;
            cmd.Parameters.Add("@Call_DNIS", SqlDbType.NVarChar).Value = Call_DNIS;
            cmd.Parameters.Add("@Call_Internal_CallID", SqlDbType.NVarChar).Value = Call_Internal_CallID;
            cmd.Parameters.Add("@Call_External_CallID", SqlDbType.NVarChar).Value = Call_External_CallID;
            cmd.Parameters.Add("@Call_StartTime", SqlDbType.DateTime).Value = Call_StartTime;
            cmd.Parameters.Add("@Call_ConnectTime", SqlDbType.DateTime).Value = Call_ConnectTime;
            cmd.Parameters.Add("@Call_End", SqlDbType.DateTime).Value = Call_End;
            cmd.Parameters.Add("@Call_Application", SqlDbType.NVarChar).Value = Call_Application;
            cmd.Parameters.Add("@Call_Data", SqlDbType.NVarChar).Value = Call_Data;
            cmd.Parameters.Add("@DataField_1", SqlDbType.NVarChar).Value = DataField_1;
            cmd.Parameters.Add("@DataField_2", SqlDbType.NVarChar).Value = DataField_2;
            
            return DialerDB.ExecNonQuery(cmd);
            /* [dbo].[sp_DM_UpdateOnlineResource]
            @DialingResource nvarchar(32),
            @Status             int,
            @Call_Direction		int,
            @Call_CLI			nvarchar(16),
            @Call_DNIS			nvarchar(16),
            @Call_Internal_CallID	nvarchar(36),
            @Call_External_CallID	nvarchar(36),
            @Call_StartTime		datetime,
            @Call_ConnectTime	datetime,
            @Call_End			datetime,
            @Call_Application	nvarchar(100),
            @Call_Data			nvarchar(100),
            @DataField_1		nvarchar(max),
            @DataField_2		nvarchar(max)
            */
        }
        
        /* Procedure: LoginOnlineResource
        *Update entry at: [OnlineResources] table. update call progress status
        * Input:   @DialingResource nvarchar(32),
        @Status int,
        @Call_ConnectTime	datetime,
        @Call_End			datetime,
        @Call_Data			nvarchar(100),
        @DataField_1		nvarchar(max),
        @DataField_2		nvarchar(max)
        * Stored procedure: Dialer DB -> [dbo].[sp_DM_UpdateOnlineResource2] 
        * Return datatable: NONE
        */
        public ReturnData UpdateOnlineResource_CallProgress(string DialingResource, int Status, int DateType/*0 = n\a, 1, connect time, 2 end time*/, DateTime dateValue, string Call_Data, string DataField_1, string DataField_2)      
        {
            MccMLogger.Debug("  >DM::UpdateOnlineResource. Source = " + DialingResource + ". Status = " + Status.ToString());
            if (!CheckConnectionObject(DialerDB))
            {
                return BuildError("DB handler is null");
            }
            DateTime Call_ConnectTime = DateTime.MinValue;
            DateTime Call_End = DateTime.MinValue;
            switch (DateType)
            {
                case 1:
                    Call_ConnectTime = dateValue;
                    break;
                case 2:
                    Call_End = dateValue;
                    break;
            }
            
            string SqlCmd = @"[dbo].[sp_DM_UpdateOnlineResource2]";
            SqlCommand cmd = new SqlCommand(SqlCmd);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@DialingResource", SqlDbType.NVarChar).Value = DialingResource;
            cmd.Parameters.Add("@Status", SqlDbType.Int).Value = Status;
            
            cmd.Parameters.Add("@Call_ConnectTime", SqlDbType.DateTime).Value = Call_ConnectTime;
            cmd.Parameters.Add("@Call_End", SqlDbType.DateTime).Value = Call_End;
            cmd.Parameters.Add("@Call_Data", SqlDbType.NVarChar).Value = Call_Data;
            cmd.Parameters.Add("@DataField_1", SqlDbType.NVarChar).Value = DataField_1;
            cmd.Parameters.Add("@DataField_2", SqlDbType.NVarChar).Value = DataField_2;
            
            return DialerDB.ExecNonQuery(cmd);
            /* [dbo].[sp_DM_UpdateOnlineResource2]
            @DialingResource nvarchar(32),
            @Status int,
            @Call_ConnectTime	datetime,
            @Call_End			datetime,
            @Call_Data			nvarchar(100),
            @DataField_1		nvarchar(max),
            @DataField_2		nvarchar(max)
            */
        }

        #endregion Evenst_online
         
        #region Cashe
         
        /* Procedure: GetAgentsBySkillInfo
         * Input: int skillID, 
         * Stored procedure: cashe DB -> test query.
         * Return datatable: list of agents in specefic skill
         */
        public ReturnData GetAgentsBySkill(int skillID)  
        {
            MccMLogger.Debug("  >DM::GetAgentsBySkill. skillID = " + skillID.ToString());
            if (!CheckConnectionObject(AACC_DB))
            {
                return BuildError("DB handler is null");
            }
                            
            string SqlCmd = "SELECT dbo.SkillsetByAgent.SkillsetState, dbo.SkillsetByAgent.Priority, dbo.Agent.PersonalDN," +
                            "       dbo.SkillsetByAgent.SkillsetID, dbo.Agent.SurName, dbo.Agent.GivenName,  dbo.Agent.TelsetLoginID " +
                            "FROM   dbo.SkillsetByAgent, dbo.Agent " +
                            "WHERE  dbo.SkillsetByAgent.UserID = dbo.Agent.UserID " +
                            "       AND (dbo.Agent.TelsetLoginID between  0  AND  9999999999999999) " +
                            "       AND  dbo.SkillsetByAgent.SkillsetID = " + skillID.ToString() + "  " +
                            "       ORDER BY dbo.Agent.TelsetLoginID";
            //OdbcCommand cmd = new OdbcCommand(SqlCmd);
            CacheCommand cmd = new CacheCommand(SqlCmd);

            ReturnData rt = AACC_DB.ExecQuery(cmd);
            return rt;
        }
        public ReturnData GetAgentsInfo(string[] AgentsList)
        {
            if (AgentsList == null)
            {
                return null;
            }
            MccMLogger.Debug("  >DM::GetAgentsInfo. Agents count = " + AgentsList.Length.ToString());
            if (!CheckConnectionObject(AACC_DB))
            {
                return BuildError("DB handler is null");
            }
            string sAgentsList = string.Join(",", AgentsList.Select(agent => agent ));

            string SqlCmd = "SELECT dbo.Agent.TelsetLoginID, dbo.SkillsetByAgent.SkillsetID, dbo.Agent.SurName,  " + 
                            "dbo.Agent.GivenName,  dbo.Agent.PersonalDN, dbo.SkillsetByAgent.SkillsetState, dbo.SkillsetByAgent.Priority " +
                            "FROM   dbo.SkillsetByAgent, dbo.Agent " +
                            "WHERE  dbo.SkillsetByAgent.UserID = dbo.Agent.UserID " +
                            "      AND (dbo.SkillsetByAgent.SkillsetState = 'Active') " + 
                            "       AND (dbo.Agent.TelsetLoginID between  0  AND  9999999999999999) " +
                            //"       AND  dbo.SkillsetByAgent.SkillsetID = " + skillID.ToString() + "  " +
                            "       AND dbo.Agent.TelsetLoginID in (" + sAgentsList  +  ")  " +
                            "       ORDER BY dbo.Agent.TelsetLoginID";
            //OdbcCommand cmd = new OdbcCommand(SqlCmd);
            CacheCommand cmd = new CacheCommand(SqlCmd);

            ReturnData rt = AACC_DB.ExecQuery(cmd);
            return rt;
        }
        public bool UpdateTmpAgentsTable(DataTable tbl_Src, string tbl_Dst)
        {
            if (tbl_Src == null)
                return false;
            ReturnData rt = DialerDB.ExecNonQuery(new SqlCommand("truncate table " + tbl_Dst));

            SqlBulkCopy bulkcopy = new SqlBulkCopy(DialerDB_ConnStr);
            bulkcopy.DestinationTableName = tbl_Dst;
            try
            {
                bulkcopy.WriteToServer(tbl_Src);
                return true;
            }
            catch (Exception ex)
            {
                MccMLogger.Error("UpdateTmpAgentsTable failed. ", ex);
                return false; 
            }

        }

        public ReturnData GetAgentsBySkill_simulator(int skillID)  
        {
            MccMLogger.Debug("  >DM::GetAgentsBySkill_simulator. skillID = " + skillID.ToString());
            
            DataTable dt = new DataTable("Result");
            dt.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("SkillsetState", typeof(string)),
                new DataColumn("Priority", typeof(Int16)),
                new DataColumn("PersonalDN", typeof(string)),
                new DataColumn("SkillsetID", typeof(Int32)),
                new DataColumn("SurName", typeof(string)),
                new DataColumn("GivenName", typeof(string)),
                new DataColumn("TelsetLoginID", typeof(string)),
            });
            dt.Rows.Add(new object[]
            {
                "Active",1,"", skillID.ToString(), "Agent_2" , "MccM",  "0077"
            });

            /*

            */
            StatusObj status = new StatusObj(ActivityStatus.Success,DateTime.Now,1,"imulator Mode");
            ReturnData rt = new ReturnData(status, new DataObj(dt));
            return rt;
          
        }
        
        #endregion cashe


        #region ReportsDB

        /* Procedure: GetAgentsBySkillInfo
         * Input: int skillID, 
         * Stored procedure: cashe DB -> test query.
         * Return datatable: list of agents in specefic skill
         */
        public ReturnData GetAgentByLoginID(string agentLoginID)  
        {
            MccMLogger.Debug("  >DM::GetAgentByLoginID. Agent Login ID = " + agentLoginID);
            if (!CheckConnectionObject(ReportsSystemDB))
            {
                return BuildError("DB handler is null");
            }
                            
            string SqlCmd = "SELECT  [TelsetLoginID], s.[Name] as [AgentHebName], [LastName], [FirstName],[User_Name], s.[MokedID], m.[Name] as [MokedHebName], m.[AliasName] as [MokedName] " + 
                            "FROM	[dbo].[symp_tx_sochen] s " + 
                            "JOIN	[dbo].[symp_tx_moked] m on s.[MokedID]  = m.MokedID " + 
                            "WHERE [TelsetLoginID] = '" + agentLoginID + "'";
            
            SqlCommand cmd = new SqlCommand(SqlCmd);
            
            ReturnData rt = ReportsSystemDB.ExecQuery(cmd);
            return rt;
        }
        public ReturnData GetAgentByLoginID_simulator(string agentLoginID)  
        {
            MccMLogger.Debug("  >DM::GetAgentByLoginID_simulator. Agent Login ID = " + agentLoginID);
                            
            string SqlCmd = "SELECT  [TelsetLoginID], s.[Name] as [AgentHebName], [LastName], [FirstName],[User_Name], s.[MokedID], m.[Name] as [MokedHebName], m.[AliasName] as [MokedName] " + 
                            "FROM	[dbo].[symp_tx_sochen] s " + 
                            "JOIN	[dbo].[symp_tx_moked] m on s.[MokedID]  = m.MokedID " + 
                            "WHERE [TelsetLoginID] = '" + agentLoginID + "'";
            
            DataTable dt = new DataTable("Result");
            dt.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("TelsetLoginID", typeof(string)),
                new DataColumn("AgentHebName", typeof(string)),
                new DataColumn("LastName", typeof(string)),
                new DataColumn("FirstName", typeof(string)),
                new DataColumn("User_Name", typeof(string)),
                new DataColumn("MokedID", typeof(string)),
                new DataColumn("MokedHebName", typeof(string)),
                new DataColumn("MokedName", typeof(string))
            });
            dt.Rows.Add(new object[]
            {
                agentLoginID,
                "שם נציג בעברית",
                "Mor",
                "Nissim",
                "x4963",
                "1",
                "מוקד בדיקה",
                "test center"
            });

            StatusObj status = new StatusObj(ActivityStatus.Success,DateTime.Now,1,"imulator Mode");
            ReturnData rt = new ReturnData(status, new DataObj(dt));
            return rt;
        }

        #endregion ReportsDB




            
        #region  Helper
                
        private bool CheckConnectionObject(object dbObj)
        {
            if (dbObj == null)
            {
                MccMLogger.Error("DB handler is null");
                return false;
            }
            return true;
        }
            
        private string GetConnectionString(string ConnName)
        {
            string retVal = "";
            string _tmp = "";
            try
            {
                _tmp = ConfigurationManager.ConnectionStrings[ConnName].ToString();
            }
            catch
            {};
            
            if (!string.IsNullOrEmpty(_tmp))
            {
                retVal = _tmp;
            }
            else
            {
                MccMLogger.Error("  Failed to locate connection string: " + ConnName);
            }
            return retVal;
        }
            
        private string GetComputer_LanIP()
        {
            string strHostName = System.Net.Dns.GetHostName();
                
            IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(strHostName);
                    
            foreach (IPAddress ipAddress in ipEntry.AddressList)
            {
                if (ipAddress.AddressFamily.ToString() == "InterNetwork")
                {
                    return ipAddress.ToString();
                }
            }
        
            return "-";
        }
            
        private ReturnData BuildError(string ErrMsg)
        {
            ReturnData rt = new ReturnData();
            rt.SetError(ErrMsg);
            return rt;
        }
            
        private string GetUser(string username)
        {
            if (!string.IsNullOrWhiteSpace(username))
                return username;
            else
                return auditUserName;
        }

       /* public ReturnData ExecQuery_New(SqlCommand cmd, DbHandler db)
        {
            return db.ExecQuery(cmd);
        }*/
        
        /* public ReturnData ExecQuery (SqlCommand cmd,DbHandler db )
        {
        return db.ExecQuery(cmd);
        /*
        string ErrMsg = "";
        db.ExternalOutput = "";
        SqlDataReader sdr = db.ExecuteQuery(cmd,out ErrMsg);
        if (sdr == null)
        {
        return BuildError(ErrMsg);
        }
        DateTime now = DateTime.Now;
        ActivityStatus status = ActivityStatus.Success;
        int rowsCount = 0;
        DataTable dt = null;
        if (sdr.HasRows)
        {
        dt = new DataTable(DefTableName);
        dt.Load(sdr);
        rowsCount = dt.Rows.Count;
        }
        return new ReturnData(new StatusObj(status, now, rowsCount,"") 
        ,new DataObj(dt));
            
        }*/
        /*public ReturnData ExecQuery(OdbcCommand cmd, CasheOdbcDbHandler db)
        {
            string ErrMsg = "";
            db.ExternalOutput = "";
            OdbcDataReader sdr = db.ExecuteQuery(cmd, out ErrMsg);
            if (sdr == null)
            {
                return BuildError(ErrMsg);
            }
            DateTime now = DateTime.Now;
            ActivityStatus status = ActivityStatus.Success;
            int rowsCount = 0;
            DataTable dt = null;
            if (sdr.HasRows)
            {
                dt = new DataTable(DefTableName);
                dt.Load(sdr);
                rowsCount = dt.Rows.Count;
            }
            return new ReturnData(new StatusObj(status, now, rowsCount,""),
                new DataObj(dt));
        }*/
        public bool checkConnectivity(int timeout_ms, string ConnectionToCheck,  out string ErrMsg  )
        {
            ErrMsg = "";
            try
            {
                switch (ConnectionToCheck)
                {
                    case "DialerDB": 
                        return DialerDB.CheckConnectivity(timeout_ms, out ErrMsg);
                       // break;
                    case "RealTimeDB": 
                        return RealTimeDB.CheckConnectivity(timeout_ms, out ErrMsg);
                       // break;
                    case "AdminDB": 
                        return DialerAdminDB.CheckConnectivity(timeout_ms, out ErrMsg);
                       // break;
                    case "ReportsSystemDB": 
                        return ReportsSystemDB.CheckConnectivity(timeout_ms, out ErrMsg);
                       // break;
                    case "AACC_DB": 
                        return AACC_DB.CheckConnectivity(timeout_ms, out ErrMsg);
                       // break;
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
                MccMLogger.Error("checkConnectivity::Error " + ex.Message);
                return false;
            }
            return true;
        }

        #endregion Helpers
    #region DataToCall

        public ReturnData GetRecordsToCall()
        {
            MccMLogger.Debug("  >DM::GetRecordsToCall ...");
            string SqlCmd = @"[dbo].[sp_Moma_GetRecordsToCall]";
            if (!CheckConnectionObject(DialerDB))
            {
                return BuildError("DB handler is null");
            }
            SqlCommand cmd = new SqlCommand(SqlCmd);
            cmd.CommandType = CommandType.StoredProcedure;
            ReturnData rt = DialerDB.ExecNonQuery(cmd);
            string a = "";
            string b = "";
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {

                    a = reader.GetString(1);
                    b = reader.GetString(2);

                  // c = reader.GetString(3);


                }
            }
            //  returnvalue = true;
            // ReturnData rt = DialerDB.ExecQuery(cmd);
            //ReturnData rt = DialerDB.ExecNonQuery(cmd);
            return rt;
        }

        public  void GetRecordsToCall(int NumberOfRecords, out SqlDataReader reader ,string sqlProc)
        {
            reader = null;

            string SqlCmd = sqlProc;// @"[dbo].[sp_Moma_GetRecordsToCall]";
            MccMLogger.Debug("  > DM ::" + sqlProc);// sp_Moma_GetRecordsToCall. ");
            if (!CheckConnectionObject(DialerDB))
            {
                MccMLogger.Error("  > DM ::   DB handler is null ");
                return;//BuildError("   DB handler is null");
            }
            //switch(CampType)
            //{
            //    case CampaignType.Preview:
            //        SqlCmd = @"[dbo].[sp_CP_GetRecordsToCall]";
            //        break;
            //    case CampaignType.Callback:
            //        SqlCmd = @"[dbo].[sp_CB_GetRecordsToCall]"; 
            //        break;
            //    default:
            //        SqlCmd = @"[dbo].[sp_CP_GetRecordsToCall]";
            //        break;
            //}
            SqlCommand cmd = new SqlCommand(SqlCmd);
            SqlCommand command = new SqlCommand(SqlCmd);
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = GetConnectionString("Dialer"); 
            
            command.Connection = connection;
            //command.CommandType = CommandType.Text;
            connection.Open();
            reader = command.ExecuteReader();

            //returnvalue = true;
            /*
             * 
             * OUTPUT INSERTED.[RecId],
		   INSERTED.[Campaign_Code],
		   INSERTED.[Load_Date],
		   INSERTED.[Customer_Field_1] as 'CustID',
		   INSERTED.[Customer_Field_2] as 'ApplicationName',
		   INSERTED.[Customer_Field_3] as 'VDN',
		   INSERTED.[Customer_Field_4] as 'UCID',
		   INSERTED.[Customer_Field_5] as 'StartCall',
		   INSERTED.[Customer_Field_16] as 'AAI',
		   INSERTED.[Next_Attempt_date] as 'CallTime',
		   INSERTED.PhoneNum_1 as 'CallTo',
		   C.DestPhoneNum As TransferTo,
		   INSERTED.Retries,
		   C.TimeBTW_Calls_Busy ,
		   C.Tries_Busy AS 'Allowedtries',
		   C.[Priority]
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Campaign_Code", SqlDbType.VarChar).Value = CampaignList;
            //cmd.Parameters.Add("@NumberOfRecords", SqlDbType.Int).Value = NumberOfRecords;
            cmd.Parameters.Add("@User", SqlDbType.NVarChar).Value = GetUser(User);
            */
            //ReturnData rt = DialerDB.ExecQuery(cmd);
            //eturn rt;
            /* [dbo].[sp_CP_GetRecordsToCall]
            @Campaign_Code		int,
            @NumberOfRecordrts	int,
            @User				nvarchar(36)*/
        }
        public void  GetAgentExtension(int NumberOfRecords, out SqlDataReader reader, string sqlProc)
        {
            reader = null;

            string SqlCmd = sqlProc;// @"[dbo].[sp_Moma_GetRecordsToCall]";
            MccMLogger.Debug("  > DM ::" + sqlProc);// sp_Moma_GetRecordsToCall. ");
            if (!CheckConnectionObject(DialerDB))
            {
                MccMLogger.Error("  > DM ::   DB handler is null ");
                return;//BuildError("   DB handler is null");
            }
            //switch(CampType)
            //{
            //    case CampaignType.Preview:
            //        SqlCmd = @"[dbo].[sp_CP_GetRecordsToCall]";
            //        break;
            //    case CampaignType.Callback:
            //        SqlCmd = @"[dbo].[sp_CB_GetRecordsToCall]"; 
            //        break;
            //    default:
            //        SqlCmd = @"[dbo].[sp_CP_GetRecordsToCall]";
            //        break;
            //}
            SqlCommand cmd = new SqlCommand(SqlCmd);
            SqlCommand command = new SqlCommand(SqlCmd);
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = GetConnectionString("Dialer");

            command.Connection = connection;
            //command.CommandType = CommandType.Text;
            connection.Open();
            reader = command.ExecuteReader();

            //returnvalue = true;
            /*
             * 
             * OUTPUT INSERTED.[RecId],
		   INSERTED.[Campaign_Code],
		   INSERTED.[Load_Date],
		   INSERTED.[Customer_Field_1] as 'CustID',
		   INSERTED.[Customer_Field_2] as 'ApplicationName',
		   INSERTED.[Customer_Field_3] as 'VDN',
		   INSERTED.[Customer_Field_4] as 'UCID',
		   INSERTED.[Customer_Field_5] as 'StartCall',
		   INSERTED.[Customer_Field_16] as 'AAI',
		   INSERTED.[Next_Attempt_date] as 'CallTime',
		   INSERTED.PhoneNum_1 as 'CallTo',
		   C.DestPhoneNum As TransferTo,
		   INSERTED.Retries,
		   C.TimeBTW_Calls_Busy ,
		   C.Tries_Busy AS 'Allowedtries',
		   C.[Priority]
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Campaign_Code", SqlDbType.VarChar).Value = CampaignList;
            //cmd.Parameters.Add("@NumberOfRecords", SqlDbType.Int).Value = NumberOfRecords;
            cmd.Parameters.Add("@User", SqlDbType.NVarChar).Value = GetUser(User);
            */
            //ReturnData rt = DialerDB.ExecQuery(cmd);
            //eturn rt;
            /* [dbo].[sp_CP_GetRecordsToCall]
            @Campaign_Code		int,
            @NumberOfRecordrts	int,
            @User				nvarchar(36)*/
        }
        public void GetAgentInfo(string computerName, string userID, out SqlDataReader reader, string sqlProc)
        {
            reader = null;

            string SqlCmd = sqlProc;
            MccMLogger.Debug("  > DM ::" + sqlProc);
           /* if (!CheckConnectionObject(AgentSPDB))
            {
                MccMLogger.Error("  > DM ::   DB handler is null ");
                return;//BuildError("   DB handler is null");
            }
            */
            
            SqlCommand command = new SqlCommand(SqlCmd);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@computerName", SqlDbType.NVarChar).Value = computerName;
            command.Parameters.Add("@userID", SqlDbType.NVarChar).Value = userID;

            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = GetConnectionString("AgentSP");

            command.Connection = connection;
            //command.CommandType = CommandType.Text;
            connection.Open();
            reader = command.ExecuteReader();

		   
        }
        public void setOtherPhoneDB(string userName, int isOtherPhone,string phone, out SqlDataReader reader, string sqlProc)
        {
            reader = null;
            
            string SqlCmd = sqlProc;
            MccMLogger.Debug("  > DM ::" + sqlProc);
            SqlCommand command = new SqlCommand(SqlCmd);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@userName", SqlDbType.NVarChar).Value = userName;
            command.Parameters.Add("@phone", SqlDbType.NVarChar).Value = phone;
            command.Parameters.Add("@isOtherPhone", SqlDbType.Int).Value = isOtherPhone;
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = GetConnectionString("AgentSP");

            command.Connection = connection;
            //command.CommandType = CommandType.Text;
            connection.Open();
            reader = command.ExecuteReader();
        }
        public void getOtherPhoneDB(string userName, out SqlDataReader reader, string sqlProc)
        {
            reader = null;

            string SqlCmd = sqlProc;
            MccMLogger.Debug("  > DM ::" + sqlProc);
            SqlCommand command = new SqlCommand(SqlCmd);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@userName", SqlDbType.NVarChar).Value = userName;

            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = GetConnectionString("AgentSP");

            command.Connection = connection;
            //command.CommandType = CommandType.Text;
            connection.Open();
            reader = command.ExecuteReader();
        }
        public void setOutPhoneDB(string userName, int isOutDial, string bphone, string extension , out SqlDataReader reader, string sqlProc)
        {

            reader = null;

            string SqlCmd = sqlProc;
            MccMLogger.Debug("  > DM ::" + sqlProc);
            SqlCommand command = new SqlCommand(SqlCmd);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@userName", SqlDbType.NVarChar).Value = userName;
            command.Parameters.Add("@bphone", SqlDbType.NVarChar).Value = bphone;
            command.Parameters.Add("@isOutDial", SqlDbType.Int).Value = isOutDial;
            command.Parameters.Add("@extension", SqlDbType.Int).Value = extension;
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = GetConnectionString("AgentSP");

            command.Connection = connection;
            //command.CommandType = CommandType.Text;
            connection.Open();
            reader = command.ExecuteReader();
        }
        public void getOutPhoneDB(string userName, out SqlDataReader reader, string sqlProc)
        {
            reader = null;

            string SqlCmd = sqlProc;
            MccMLogger.Debug("  > DM ::" + sqlProc);
            SqlCommand command = new SqlCommand(SqlCmd);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@userName", SqlDbType.NVarChar).Value = userName;

            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = GetConnectionString("AgentSP");

            command.Connection = connection;
            //command.CommandType = CommandType.Text;
            connection.Open();
            reader = command.ExecuteReader();
        }

        #endregion DataToCall
        #region MacSales
        public ReturnData SetActiveCampaigns()
        {
            MccMLogger.Debug("  >DM::SetActiveCampaigns ...");
            string SqlCmd = @"[dbo].[sp_DM_SetActiveCampaigns]";
            if (!CheckConnectionObject(DialerDB))
            {
                return BuildError("DB handler is null");
            }
            SqlCommand cmd = new SqlCommand(SqlCmd);
            cmd.CommandType = CommandType.StoredProcedure;
            //ReturnData rt = DialerDB.ExecNonQuery(cmd);
            ReturnData rt = DialerDB.ExecQuery(cmd);
            return rt;
        }
        public ReturnData SetAgentLinkage()
        {
            MccMLogger.Debug("  >DM::SetAgentLinkage ...");
            string SqlCmd = @"[dbo].[sp_DM_SetAgentLikage]";
            if (!CheckConnectionObject(DialerDB))
            {
                return BuildError("DB handler is null");
            }
            SqlCommand cmd = new SqlCommand(SqlCmd);
            cmd.CommandType = CommandType.StoredProcedure;
            ReturnData rt = DialerDB.ExecQuery(cmd);
            return rt;
        }
    }

        #endregion MacSales



    
}

