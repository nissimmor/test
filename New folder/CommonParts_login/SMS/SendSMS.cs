using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using MCCMLogger;
using WcfLogInService;

namespace WcfLogInService.SMS
{
    public  class SendSMS
    {
        public static DateTime _LastTimeSendSMSFailDialer;
        public static DateTime _LastTimeSendSMSOldRecords;
        public static void SendSMSEndRetries(string phone)
        {
            string smsMsg = ConfigurationManager.AppSettings["SMS_Endretries_Content"].ToString();
            SendSMSGlobal(phone, smsMsg);
        }

        public static void SendSMSOldRecords()
        {
            MccMLogger.Debug("<<<<<------SendSMSOldRecords Function start....----->>>>>>");
            string smsMsg = ConfigurationManager.AppSettings["SMS_OldRecords_Content"].ToString();
            string smsPhone = ConfigurationManager.AppSettings["SMSPhone"].ToString();
            int SMSEveryTime= Convert.ToInt32(ConfigurationManager.AppSettings["SMSEveryTime"].ToString());
            int maxTime = Convert.ToInt32(ConfigurationManager.AppSettings["MaxTime"].ToString());
            if (_LastTimeSendSMSOldRecords <= DateTime.Now.AddMinutes(-SMSEveryTime))
            {
                DBmanager dbManger = new DBmanager();
               // ReturnData rt = dbManger.CheckIfOldRecordsExists(maxTime);
                //No old records exists
                //if (rt.DataInfo.Data != null)
                //{
                    _LastTimeSendSMSOldRecords = DateTime.Now;
                    SendSMSGlobal(smsPhone, smsMsg);
                    
                //}
            }
        }

        public static void SendSMSFailDialer()
        {
            MccMLogger.Debug("<<<<<------SendSMSFailDialer Function start....----->>>>>>");
            string smsMsg = ConfigurationManager.AppSettings["SMS_FailDialer_Content"].ToString();
            string smsPhone = ConfigurationManager.AppSettings["SMSPhone"].ToString();
            int SMSEveryTime = Convert.ToInt32(ConfigurationManager.AppSettings["SMSEveryTime"].ToString());
            if (_LastTimeSendSMSFailDialer <= DateTime.Now.AddMinutes(-SMSEveryTime))
            {
                _LastTimeSendSMSFailDialer = DateTime.Now;
                SendSMSGlobal(smsPhone, smsMsg);
                
            }
        }


        private static void SendSMSGlobal(string phone, string smsMsg)
        {
            MccMLogger.Debug("<<<<<------SendSMSGlobal Function start....----->>>>>>");
            List<string> targets = new List<string>();
            var numbers = phone.Split(',').Select(Convert.ToString).ToList();
            targets.AddRange(numbers);
            System.Net.WebClient client = new System.Net.WebClient();
            client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            string urlSms = "http://smsalert1/SNSManager/msgSend.jsp?uid=nurse&amp;pass=nurse1&amp;encoding=windows-1255&";
            string smsMsgWithEncoding = HttpUtility.UrlEncode(smsMsg, Encoding.GetEncoding("windows-1255"));
            string sendSms = urlSms + "to=Cellact:*" + phone + "&msg=" + smsMsgWithEncoding;
            string htmlResult = client.DownloadString(sendSms);
            Match m = Regex.Match(htmlResult, "Message was added with ID (?<IdNo>\\d+)");
            string id = m.Groups["IdNo"].Value;
            string SMSServiceAddress = ConfigurationManager.AppSettings["SMSServiceAddress"];
            string SMSCenterUser = ConfigurationManager.AppSettings["SMSCenterUser"];
            string SMSCenterPassword = ConfigurationManager.AppSettings["SMSCenterPassword"];
            SendToSNS(SMSServiceAddress, SMSCenterUser, SMSCenterPassword, targets, smsMsgWithEncoding);
        }

        private static string SendToSNS(string serverurl, string userName, string password, List<string> targets, string textMessage)
        {
            string status = "error,sms not sent";
            string formatFromConfig = ConfigurationManager.AppSettings["StringFormatForSNS"];
            try
            {
                foreach (var target in targets)
                {              
                
                string to = "Cellact:*" + target;
                string text = serverurl + string.Format(formatFromConfig, userName, password, to, textMessage);
                WebRequest request = WebRequest.Create(text);
                WebResponse response = request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                string responseFromServer = reader.ReadToEnd();
                status = Between(responseFromServer, "<label id=\"statusMsg\">", "</label>");
                MccMLogger.Debug("<<<<<<----Send SMS to number : " + target  + " Success--->>>>>>");
                }

            }
            catch (Exception ex)
            {
                MccMLogger.Debug("<<<<<<----Send SMS Fail--->>>>>>");
            }
            return status;

        }

        public static string Between(string STR, string FirstString, string LastString)
        {
            string FinalString;
            int Pos1 = STR.IndexOf(FirstString) + FirstString.Length;
            int Pos2 = STR.IndexOf(LastString);
            FinalString = STR.Substring(Pos1, Pos2 - Pos1);
            return FinalString;
        }
    }
}
