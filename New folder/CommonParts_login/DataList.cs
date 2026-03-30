using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
namespace WcfLogInService
{

    public class dtCamp
    {

        public int arrCampaign_Code = 0;
        public int arrCampaign_Name = 1;//
        public int arrCampaignTypeID = 2;//
        public int arrDialingApplicationID = 3;//
        public int arrCampaignStatusID = 4;//
        public int arrCreation_Date = 5;//
        public int arrlast_Update_Date = 6;//
        public int arrlast_Update_User = 7;//
        public int arrStart_Date = 8;//
        public int arrStart_Time_1 = 9;//
        public int arrEnd_Time_1 = 10;//
        public int arrStart_Time_2 = 11;//
        public int arrEnd_Time_2 = 12;//
        public int arrEnd_Date = 13;//
        public int arrPriority = 14;
        public int arrComments = 15;//
        public int arrApplicationID = 16;//
        public int arrSkillID = 17;
        public int arrCDN = 18;//
        public int arrWrapup = 19;
        public int arrTries_Busy = 20;//
        public int arrTimeBTW_Calls_Busy = 21;//
        public int arrTries_NRD = 22;//
        public int arrTime_BTWCalls_NRD = 23;//
        public int arrNotReadyReasonsID = 24;
        public int arrIsBleding = 25;
        public int arrTmp_DialingApplication = 27;//
        public int arrTmp_Parameter = 28;//

        private Int32 _Campaign_Code;
        private String _Campaign_Name;
        private Int32 _CampaignTypeID;
        private Int32 _DialingApplicationID;
        private Int32 _CampaignStatusID;
        private DateTime _Creation_Date;
        private DateTime _last_Update_Date;
        private String _last_Update_User;
        private DateTime _Start_Date;
        private TimeSpan _Start_Time_1;
        private TimeSpan _End_Time_1;
        private TimeSpan _Start_Time_2;
        private TimeSpan _End_Time_2;
        private DateTime _End_Date;
        private Int32 _Priority;
        private String _Comments;
        private Int32 _ApplicationID;
        private Int32 _SkillID;
        private Int32 _CDN;
        private Int32 _Wrapup;
        private Int32 _Tries_Busy;
        private Int32 _TimeBTW_Calls_Busy;
        private Int32 _Tries_NRD;
        private Int32 _Time_BTWCalls_NRD;
        private Int32 _NotReadyReasonsID;
        private Int32 _IsBleding;
        private Int32 _Tmp_DialingApplication;
        private Int32 _Tmp_Parameter;
        public dtCamp()
        {
            _Campaign_Code = 0;
            _Campaign_Name = "";
            _CampaignTypeID = 0;
            _DialingApplicationID = 0;
            _CampaignStatusID = 0;
            _Creation_Date = new DateTime();
            _last_Update_Date = new DateTime();
            _last_Update_User = "";
            _Start_Date = new DateTime();
            _Start_Time_1 = new TimeSpan(); 
            _End_Time_1 = new TimeSpan();
            _Start_Time_2 = new TimeSpan();
            _End_Time_2 = new TimeSpan();
            _End_Date = new DateTime(); 
            _Priority = 0;
            _Comments = "";
            _ApplicationID = 0;
            _SkillID = 0;
            _CDN = 0;
            _Wrapup = 0;
            _Tries_Busy = 0;
            _TimeBTW_Calls_Busy = 0;
            _Tries_NRD = 0;
            _Time_BTWCalls_NRD = 0;
            _NotReadyReasonsID = 0;
            _IsBleding = 0;
            _Tmp_DialingApplication = 0;
            _Tmp_Parameter = 0;
        }

        public dtCamp( Int32 Campaign_Code,String Campaign_Name,Int32 CampaignTypeID,Int32 DialingApplicationID,Int32 CampaignStatusID,DateTime Creation_Date,DateTime last_Update_Date,String last_Update_User,DateTime Start_Date,TimeSpan Start_Time_1,TimeSpan End_Time_1,TimeSpan Start_Time_2,TimeSpan End_Time_2,DateTime End_Date,Int32 Priority,String Comments,Int32 ApplicationID,Int32 SkillID,Int32 CDN,Int32 Wrapup,Int32 Tries_Busy,Int32 TimeBTW_Calls_Busy,Int32 Tries_NRD,Int32 Time_BTWCalls_NRD,Int32 NotReadyReasonsID,Int32 IsBleding,Int32 Tmp_DialingApplication,Int32 Tmp_Parameter)
        {
            _Campaign_Code = Campaign_Code;
            _Campaign_Name = Campaign_Name;
            _CampaignTypeID = CampaignTypeID;
            _DialingApplicationID = DialingApplicationID;
            _CampaignStatusID = CampaignStatusID;
            _Creation_Date = Creation_Date;
            _last_Update_Date = last_Update_Date;
            _last_Update_User = last_Update_User;
            _Start_Date = Start_Date;
            _Start_Time_1 = Start_Time_1;
            _End_Time_1 = End_Time_1;
            _Start_Time_2 = Start_Time_2;
            _End_Time_2 = End_Time_2;
            _End_Date = End_Date;
            _Priority = Priority;
            _Comments = Comments;
            _ApplicationID = ApplicationID;
            _SkillID = SkillID;
            _CDN = CDN;
            _Wrapup = Wrapup;
            _Tries_Busy = Tries_Busy;
            _TimeBTW_Calls_Busy = TimeBTW_Calls_Busy;
            _Tries_NRD = Tries_NRD;
            _Time_BTWCalls_NRD = Time_BTWCalls_NRD;
            _NotReadyReasonsID = NotReadyReasonsID;
            _IsBleding = IsBleding;
            _Tmp_DialingApplication = Tmp_DialingApplication;
            _Tmp_Parameter = Tmp_Parameter;
        }
        public Int32 Campaign_Code
        {
            get { return _Campaign_Code; }
            set { _Campaign_Code = value; }
        }
        public String Campaign_Name
        {
            get { return _Campaign_Name; }
            set { _Campaign_Name = value; }
        }
        public Int32 CampaignTypeID
        {
            get { return _CampaignTypeID; }
            set { _CampaignTypeID = value; }
        }
        public Int32 DialingApplicationID
        {
            get { return _DialingApplicationID; }
            set { _DialingApplicationID = value; }
        }
        public  Int32 CampaignStatusID
        {
            get { return _CampaignStatusID; }
            set { _CampaignStatusID = value; }
        }
        public DateTime Creation_Date
        {
            get { return _Creation_Date; }
            set { _Creation_Date = value; }
        }
        public DateTime last_Update_Date
        {
            get { return _last_Update_Date; }
            set { _last_Update_Date = value; }
        }
        public String last_Update_User
        {
            get { return _last_Update_User; }
            set { _last_Update_User = value; }
        }
        public DateTime Start_Date
        {
            get { return _Start_Date; }
            set { _Start_Date = value; }
        }
        public TimeSpan Start_Time_1
        {
            get { return _Start_Time_1; }
            set { _Start_Time_1 = value; }
        }
        public TimeSpan End_Time_1
        {
            get { return _End_Time_1; }
            set { _End_Time_1 = value; }
        }
        public TimeSpan Start_Time_2
        {
            get { return _Start_Time_2; }
            set { _Start_Time_2 = value; }
        }
        public TimeSpan End_Time_2
        {
            get { return _End_Time_2; }
            set { _End_Time_2 = value; }
        }
        public DateTime End_Date
        {
            get { return _End_Date; }
            set { _End_Date = value; }
        }
        public Int32 Priority
        {
            get { return _Priority; }
            set { _Priority = value; }
        }
        public String Comments
        {
            get { return _Comments; }
            set { _Comments = value; }
        }
        public Int32 ApplicationID
        {
            get { return _ApplicationID; }
            set { _ApplicationID = value; }
        }
        public Int32  SkillID
        {
            get { return _SkillID; }
            set { _SkillID = value; }
        }
        public Int32 CDN
        {
            get { return _CDN; }
            set { _CDN = value; }
        }
        public Int32 Wrapup
        {
            get { return _Wrapup; }
            set { _Wrapup = value; }
        }
        public Int32 Tries_Busy
        {
            get { return _Tries_Busy; }
            set { _Tries_Busy = value; }
        }
        public Int32 TimeBTW_Calls_Busy
        {
            get { return _TimeBTW_Calls_Busy; }
            set { _TimeBTW_Calls_Busy = value; }
        }
        public Int32 Tries_NRD
        {
            get { return _Tries_NRD; }
            set { _Tries_NRD = value; }
        }
        public Int32 Time_BTWCalls_NRD
        {
            get { return _Time_BTWCalls_NRD; }
            set { _Time_BTWCalls_NRD = value; }
        }
        public Int32 NotReadyReasonsID
        {
            get { return _NotReadyReasonsID; }
            set { _NotReadyReasonsID = value; }
        }
        public Int32 IsBleding
        {
            get { return _IsBleding; }
            set { _IsBleding = value; }
        }
        public Int32 Tmp_DialingApplicatione
        {
            get { return _Tmp_DialingApplication; }
            set { _Tmp_DialingApplication = value; }
        }
        public Int32 Tmp_Parameter
        {
            get { return _Tmp_Parameter; }
            set { _Tmp_Parameter = value; }
        }
      

    }
   
        public class Agent
        {
            private string _agentID; 
            private eAgentStat _agentStat; 
            private eCallStat  _agentACStat; 
            private ePriorety _agentPriority;
            private int _agentRecID;

            public Agent(string agentID, eAgentStat agentStat, eCallStat  agentACStat,ePriorety agentPriority,int agentRecID)
            {
                _agentID = agentID;
                _agentStat = agentStat;
                _agentACStat = agentACStat;
                _agentPriority = agentPriority;
                _agentRecID = agentRecID;
            }
            public string agentID
            {
                get { return _agentID; }
                set { _agentID = value; }
            }
            public eAgentStat agentStat
            {
                get { return _agentStat; }
                set { _agentStat = value; }
            }
            public eCallStat agentACStat
            {
                get { return _agentACStat; }
                set { _agentACStat = value; }
            }
            public ePriorety agentPriority
            {
                get { return _agentPriority; }
                set { _agentPriority = value; }
            }
            public int agentRecID
            {
                get { return _agentRecID; }
                set { _agentRecID = value; }
            }
        }
        public   class CampOLD
        {
            private string _campID;
            private eDialer _campType;
            private eCampStat _campStatus;
            private ePriorety _campPriority;
            private string _campAppSkill;
            private List<Agent> _campAgentlist;
            private eCampStat _campOldStatus;
            private string  _camp_Wrapup;
            private string _camp_Tries_Busy;
            private string _camp_TimeBTWCalls_Busy;
            private string _camp_Tries_NRD;
            private string _camp_TimeBTWCalls_NRD;
            private int _campRecID;



           
            public string campID
            {
                get { return _campID; }
                set { _campID = value; }
            }
            public eDialer campType
            {
                get { return _campType; }
                set { _campType = value; }
            }
            public eCampStat campStatus
            {
                get { return _campStatus; }
                set { _campStatus = value; }
            }
            public string campAppSkill
            {
                get {return _campAppSkill;}
                set {_campAppSkill = value;}
            }
            public ePriorety campPriority
            {
                get { return _campPriority; }
                set { _campPriority = value; }
            }
            public List<Agent> campAgentlist
            {
                //get { return _campAgentlist; }
                //set { _campAgentlist = value; }
                get { return nn.updateList(_campAgentlist); }
                set { _campAgentlist = value.ToList(); }
            }
            public eCampStat campOldStatus
            {
                get { return _campOldStatus; }
                set { _campOldStatus = value; }
            }
            public string camp_Wrapup
            {
                get { return _camp_Wrapup; }
                set { _camp_Wrapup = value; }
            }
            public string camp_Tries_Busy
            {
                get {return _camp_Tries_Busy;}
                set {_camp_Tries_Busy = value;}
            }
            public string camp_TimeBTWCalls_Busy
            {
                get {return _camp_TimeBTWCalls_Busy;}
                set {_camp_TimeBTWCalls_Busy = value;}
            }
            public string camp_Tries_NRD
            {
                get {return _camp_Tries_NRD;}
                set {_camp_Tries_NRD = value;}
            }
            public string camp_TimeBTWCalls_NRD
            {
                get {return _camp_TimeBTWCalls_NRD;}
                set {_camp_TimeBTWCalls_NRD = value;}
            }
            public int campRecID
            {
                get { return _campRecID; }
                set { _campRecID = value; }
            }

        }
      public   class Camp
        {
            private int _campPriority;
            private int _campSkillID;
            private int _campWrapup;
            private int _campIsBleding;
               



            public Camp(int campPriority, int campSkillID, int campWrapup,int campIsBleding)

            {
                _campPriority = campPriority;
                _campSkillID = campSkillID;
                _campIsBleding = campIsBleding;
                _campWrapup = camp_Wrapup;
                
            }

            public int campPriority
            {
                get { return _campPriority; }
                set { _campPriority = value; }
            }
           public int campSkillID
            {
                get { return _campSkillID; }
                set { _campSkillID = value; }
            }
           public int campIsBlending
            {
                get { return _campIsBleding; }
                set { _campIsBleding = value; }
            }
           public int camp_Wrapup
            {
                get { return _campWrapup; }
                set { _campWrapup = value; }
            }

        }

        public class objDate
        {
            private string _dataName;
            private DataTable _dataTable;
        }
           
        public class currdialer
        {
            private eDialer _currDial;
            private int _licAgent;
            private int _licCamp;
            private int _licApp;
            private int _licSkill;
            private int _licLine;
            
            public currdialer(eDialer currDial,int licAgent,int licCamp,int licApp,int licSkill,int licLine)
            {
                _currDial = currDial;
                _licAgent = licAgent;
                _licCamp= licCamp;
                _licApp = licApp;
                _licSkill = licSkill;
                _licLine = licLine;
            }
            public eDialer currDial
            {
                 get; set; 
                //get{ return currDial; }
                //set { currDial = value; }
            }
            public int licAgent
            {
                 get; set; 
                //get{ return licAgent; }
                //set { licAgent = value; }
            }
            public int licCamp
            {
                 get; set; 
                //get { return licCamp; }
                //set { licCamp = value; }
            }
            public int licApp
            {
                 get; set; 
                //get{ return licApp; }
                //set { licApp = value; }
            }
            public int licSkill
            {
                 get; set; 
                //get{ return licSkill; }
                //set { licSkill = value; }
            }
            public int licLine
            {
                get; set; 
                //get{ return licLine; }
                //set { licLine = value; }
            }
            

        }
        public  class Customer
        {
          
            
      
            private string  _custRecID;
            private int _custRecNum;
            private ePriorety _custPriority;
            private string[] _custPhoneNum;// = new string [10];
            private string[] _custPhoneType;// = new string[10];
            private string[] _custADField;// = new string[20];
            private string[] _custADName;// = new string[20];
            private string[] _custComment;// = new string[5];
            private Dictionary<int, string> _custDialerAc;
            private DateTime _custNextAttempt;
            private string _custRetries;

            public Customer(string custRecID, int custRecNum, ePriorety custPriority, string[] custPhoneNum, string[] custPhoneType, string[] custADField, string[] custADName, string[] custComment, Dictionary<int, string> custDialerAc, DateTime custNextAttempt, string custRetries)
            {
                _custRecID = custRecID;
                _custRecNum = custRecNum;
                _custPriority = custPriority;
                _custPhoneNum = new string [custPhoneNum.Length];
                custPhoneNum.CopyTo(_custPhoneNum,0);
                _custPhoneType = new string [custPhoneType.Length];
                custPhoneType.CopyTo(_custPhoneType, 0);
                _custADField = new string [custADField.Length];
                custADField.CopyTo(_custADField, 0);
                _custADName = new string[custADName.Length];
                custADName.CopyTo(_custADName, 0);
                _custComment = new string [custComment.Length];
                custComment.CopyTo(_custComment, 0);
                
                _custDialerAc=custDialerAc;
                _custNextAttempt = custNextAttempt;
                _custRetries = custRetries;
                
                               
            }
            public string  custRecID
            {
                get { return _custRecID; }
                set { _custRecID = value; }
            }
            public int custRecNum
            {
                get { return _custRecNum; }
                set { _custRecNum = value; }
            }
            public ePriorety custPriority
            {
                get { return _custPriority; }
                set { _custPriority = value; }
            }
            public string[] custPhoneNum
            {
                get { return updateArr(_custPhoneNum); }
                set { value.CopyTo(_custPhoneNum, 0); }
            }
            public string[] custPhoneType
            {
                get { return updateArr(_custPhoneType); }
                set { value.CopyTo(_custPhoneType, 0); }
            }
            public string[] custADField
            {
                get { return updateArr(_custADField); }
                set { value.CopyTo(_custADField, 0); }
            }
            public string[] custADName
            {
                get { return updateArr(_custADName); }
                set { value.CopyTo(_custADName, 0); }
            }
            public string[] custComment
            {
                get { return updateArr(_custComment); }
                set { value.CopyTo(_custComment, 0); }
            }
            public Dictionary<int, string> custDialerAc
            {
                get {return _custDialerAc; }
                set { _custDialerAc=value; }
            }
            public DateTime custNextAttempt
            {
                get { return _custNextAttempt; }
                set { _custNextAttempt = value; }
            }
            public string custRetries
            {
                get { return _custRetries; }
                set { _custRecID = value; }
            }

            public string[] updateArr (string [] strOrgin)
            {
                string [] strdest =new string[strOrgin.Length];
                strOrgin.CopyTo(strdest,0);
                return strdest;
            }
            
        }

        public class DialerAC
        {
            private int _ACID;
            private string  _ACDesc;
            public DialerAC(int ACID, string ACDesc)
            {
                _ACID = ACID;
                _ACDesc = ACDesc;
                
            }
            public int ACID
            {
                get { return _ACID; }
                set { _ACID = value; }
            }
            public string ACDesc
            {
                get { return _ACDesc; }
                set { _ACDesc = value; }
            }
          }

        public enum ePriorety
            {
                high=0,
                middele,
                low,
                NumOfePrioretys
            }
        public enum eDialer
        {
            CallPerview = 1,
            CallMasive=2,
            CallBack = 4,
            CallAbanded = 8,
            CallProxy=16
          
        }
        public enum eCampStat
        {
            active=1,
            deactive=2,
            suspend=3,
            suspendbyAdmin=4,
            deleted=9 
        }
        public enum eDB
            {
                Localdb=0,
                CTIdb,
                RTdb,
                NumOfeDBs
            }
        public enum eCallStat
           {
               //  1       2                   3               4           5       6           7           8           9
               IDLE = 1, IN_COMING_ACTIVE, IN_COMING_RINGING, OUT_GOING, HOLD, DISCONNECT, OUT_INNER, CONFERENCE, TRASFERED, NumOfeCallStatus
           }
        public enum eAgentStat
           {
               //  1       2                   3               4           5       6           7           8           9
               IDLE = 1, Ready, NotReady, Busy, HOLD, DISCONNECT, OUT_INNER, CONFERENCE, TRASFERED, NumOfeCallStatus
           }
     public static class  nn
        {
         public static List<T>  updateList <T> (this List<T> source)
            {

                List<T> destList=new List<T>();

                //destList.AddRange(source);
                destList = source.ToList();
                
                //string[] strdest = new string[strOrgin.Length];
                //strOrgin.CoyTo(strdest, 0);
                return destList;
            }   
}

     /*
      *  public class CampOld(string campID, eDialer campType, eCampStat campStatus, ePriorety campPriority, string campAppSkill, List<Agent> campAgentlist, eCampStat campOldStatus, string camp_Wrapup, string camp_Tries_Busy, string camp_TimeBTWCalls_Busy, string camp_Tries_NRD, string camp_TimeBTWCalls_NRD, int campRecID)
                 {
                     _campID = campID;
                     _campType = campType;
                     _campStatus = campStatus;
                     _campPriority = campPriority;
                     _campAppSkill = campAppSkill;
                     _campAgentlist = campAgentlist.ToList();
                     _campOldStatus = campOldStatus;
                     _camp_Wrapup = camp_Wrapup;
                     _camp_Tries_Busy = camp_Tries_Busy;
                     _camp_TimeBTWCalls_Busy = camp_TimeBTWCalls_Busy;
                     _camp_Tries_NRD = camp_Tries_NRD;
                     _camp_TimeBTWCalls_NRD = camp_TimeBTWCalls_NRD;
                     _campRecID = campRecID;
                 }
       */
 }
