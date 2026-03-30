using WcfLogInService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MCCMLogger;

namespace CommonParts
{
    public class ChReplacement
    {
        public Dictionary<int, Array> SetCampaignsList()
        {
            Dictionary<int, Array> CampaignsInfo = GetCampaignsInfo();
            return CampaignsInfo;
            /*
            Dictionary<int, Array> _dicCampaignsInfo = new Dictionary<int, Array>();
            if (CampaignsInfo == null)
            {
                return null;
            }

            try
            {
                Dictionary<int, object[]> _odicCampaignsInfo = new Dictionary<int, object[]>();
                Array array = null;
                foreach (KeyValuePair<int, Array> arr in _dicCampaignsInfo)
                {


                    if (_dicCampaignsInfo.TryGetValue(arr.Key, out array))
                    {
                        object[] oarray = array.OfType<object>().Select(o => o.ToString()).ToArray();
                        _odicCampaignsInfo.Add(arr.Key, oarray);
                    }
                }
                
            }
            catch (Exception ex)
            {
                MccMLogger.Error("CHR::SetCampaignsList", ex);
                return false;
            }


            return true;
            */
        }

        private Dictionary<int, Array> GetCampaignsInfo()
        {
            bool RT = false;
            int loc = 0;
            Dictionary<int, Array> CampaignsInfo = new Dictionary<int, Array>();
            try
            {
                DBmanager dbManger = new DBmanager();
                ReturnData rt = dbManger.SetActiveCampaigns();
                if (rt.StatusInfo.Status != ActivityStatus.Success)
                {
                    MccMLogger.Error("CHR::GetCampaignsInfo Failed to set active campaigns. error -  " + rt.StatusInfo);
                    return null;
                }
                ReturnData DB_ReturnData = dbManger.GetCampaignsInfo(((int)eDialer.CallPerview));
                if ((check_dbRD(DB_ReturnData)) && (DB_ReturnData.StatusInfo.RowsCount > 0))
                {
                    var arr = DB_ReturnData.DataInfo.Data.Rows.Cast<System.Data.DataRow>().Select(r => r.ItemArray).ToArray();
                    foreach (object[] entry in arr)
                    {
                        int itemp = int.Parse(entry[loc].ToString());
                        if ((CampaignsInfo.Count != 0) && (CampaignsInfo.ContainsKey(itemp)))
                            CampaignsInfo[itemp] = entry;
                        else
                            CampaignsInfo.Add(itemp, entry);
                    }
                }
                return CampaignsInfo;
            }
            catch (Exception ex)
            {
                MccMLogger.Error("CHR::GetCampaignsInfo  error ", ex);
                return null;
            }
            return null;
        }

        public bool check_dbRD(ReturnData DB_ReturnData)
        {
            if (DB_ReturnData == null)
            {
                MccMLogger.Debug("CHR::check_dbRD DB_ReturnData object is null  ");
                return false;
            }
            if (DB_ReturnData.StatusInfo.Status != ActivityStatus.Success)
            {
                MccMLogger.Debug("CHR::check_dbRD Activity failed. status = " + DB_ReturnData.StatusInfo.Status.ToString());
                return false;
            }
            if (DB_ReturnData.StatusInfo.RowsCount < 1)
            {
                MccMLogger.Debug("CHR::check_dbRD No records found");
                return false;
            }
            MccMLogger.Debug(string.Format("CHR::CheckDataObj:: Status={0}; Rows count={1}",
                           DB_ReturnData.StatusInfo.Status.ToString(),
                           DB_ReturnData.StatusInfo.RowsCount.ToString()));
            return true;
        }
    }
}
