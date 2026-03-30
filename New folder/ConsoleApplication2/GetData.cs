using MCCMLogger;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WcfLogInService;

namespace WcfLogInService
{
    class GetData
    {

        public static bool GetRecordInfo()
        {
            MccMLogger.Debug("CH::GetRecordInfo - start");
            string sqlProc = "";
            bool RT = false;
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
                    RT = true;
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
                RT = false;
                return RT;
            }
            //RT = DictintEqualsOrderM(dicskillInfoByCamp, skillInfoByCamp);
            RT = true;
            return RT;

        }
    }
}
