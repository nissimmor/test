using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Drawing;
using System.ServiceModel;
using System.ServiceModel.Channels;


using LoginForm.CCMAlogin;
using LoginForm;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using MCCMLogger;
using System.Runtime.Serialization;

namespace LoginForm
{
    public class AdressChg
    {
        private string AgentIn;
        private string AACCIP;
        private string CommToken;
        private string CCMAURL;
        private string UserName;
        private string Password;
        private string voiceURI;
        //private string domain_cct;

        AuthenticationService authenticate;
        string ssoToken = string.Empty;
        string error = string.Empty;

        private OICCMA.UserDetails local_user = new OICCMA.UserDetails();
        public AdressChg()
        {
            
            voiceURI = "sip:0025@mac.org.il";
            AgentIn =  "0025"; // "0088"; //
            AACCIP = "172.26.30.66";// "172.26.34.68";
            UserName = "nissim";// "webadmin";
            Password = "Q1w2e3r4!";// "AdminWeb1";
        //CommToken = TheToken;
        //CCMAURL = TheURL;


    }
        public AdressChg(bool start)
        {

            voiceURI = "sip:0025@mac.org.il";
            AgentIn = "0025"; // "0088"; //
            AACCIP = "172.26.30.66";// "172.26.34.68";
            UserName = "nissim";// "webadmin";
            Password = "Q1w2e3r4!";// "AdminWeb1";
                                   //CommToken = TheToken;
                                   //CCMAURL = TheURL;
                                    
            LoginAuth();


        }

        // public AdressChg(string AgentID, string AACC_IP,  string TheURL)


        public AdressChg(string agentID, string extNumber, string userID,string wepass,string  webuser,string ipacc)
        {

            MccMLogger.Info("start adChange \n\n");
            //string thevoiceURI 
            voiceURI = "sip:"+ extNumber+"@mac.org.il";
            AACCIP = ipacc;// "172.26.34.68";
            UserName = webuser;// "webadmin";
            Password = wepass;// "AdminWeb1";
            AgentIn = agentID;
            CCMAURL = "https://" + AACCIP + "/WebServices/OpenInterfaces/soap.svc";
            //AACCIP = AACC_IP;
            //CommToken = ssoToken;
            //CCMAURL = TheURL;
            //voiceURI = thevoiceURI;
            /*MccMLogger.Info(DateTime.Now.ToString());
            LoginAuth();
            MccMLogger.Info(DateTime.Now.ToString());
            //UpdateAgentDetails();
            LoadAgentDetails();
            MccMLogger.Info(DateTime.Now.ToString()); */
        }
        public bool NewAdress()
        {


            //AgentIn = AgentID;
            //AACCIP = AACC_IP;
            CommToken = ssoToken;
            //CCMAURL = TheURL;
            LoginAuth();
            
            LoadAgentDetails();
            return true;
        }
        public bool AddAgent()
        {


            //AgentIn = AgentID;
            //AACCIP = AACC_IP;
            CommToken = ssoToken;
            //CCMAURL = TheURL;
            LoginAuth();

            AddAgentDetails();
            return true;
        }
        private void LoginAuth()
        {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.DefaultConnectionLimit = 9999;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
                //Get certificate
                X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                store.Open(OpenFlags.ReadOnly);
                X509Certificate2Collection collection = store.Certificates.Find(X509FindType.FindBySubjectName, "MaccabiOCA", false);
                //Somewhere();
                // Skip validation of SSL/TLS certificate
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                authenticate = new AuthenticationService();
                authenticate.Url = "https://" + AACCIP + "/WebServices/Authentication/Service.asmx";
                try
                {
                    if (!authenticate.AuthenticateUser(UserName, Password, out ssoToken, out error))
                    {
                        // MessageBox.Show("Error logging in user " + UserName + ". Error: " + error);
                        return;
                    }
                    else
                    {
                        CCMAURL = "https://" + AACCIP + "/WebServices/OpenInterfaces/soap.svc";
                        //this.Cursor = Cursors.WaitCursor;
                        //Agent myAgent = new Agent(ssoToken, "https://" + AACCIP + "/WebServices/OpenInterfaces/soap.svc");
                        //this.Cursor = Cursors.Default;

                        //if (myAgent.ShowDialog() == DialogResult.OK)
                        //{
                        //    ssoToken = string.Empty; //logout
                        //}
                        CommToken = ssoToken;
                  //  MccMLogger.Info("info LoginAuth "+ ssoToken);
                }
                }
                catch (Exception Ex)
                {
                    MccMLogger.Error("error LoginAuth \n\n", Ex);
                    //  MessageBox.Show("An exception was raised while attempting to authenticate user. Details: " + Ex.Message);
                }
            
        }
       /* public void GetAgentList()
        {
            string errorMessage = null;
            OICCMA.AgentFullDetails[] Agents;

            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            basicHttpBinding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
            basicHttpBinding.MaxReceivedMessageSize = int.MaxValue;
            EndpointAddress endpointAddress = new EndpointAddress(CCMAURL);
            ChannelFactory<OICCMA.soap> oiChannelFactory = new ChannelFactory<OICCMA.soap>(basicHttpBinding, endpointAddress);
            OICCMA.soap oiService = oiChannelFactory.CreateChannel();

            try
            {
                using (OperationContextScope scope = new OperationContextScope((IContextChannel)oiService))
                {
                    HttpRequestMessageProperty requestProperty = new HttpRequestMessageProperty();
                    requestProperty.Headers["NTSSOCookie"] = CommToken;
                    OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestProperty;

                    oiService.getAgentsList(out Agents, out errorMessage, AACCIP);

                    if (errorMessage != null && errorMessage != "")
                    {
                        MessageBox.Show("Error text was received from the getAllAgents method. Details: " + errorMessage);
                    }
                   // lBoxAgents.Items.Clear();
                    for (int i = 0; i < Agents.Length; i++)
                    {
                        //lBoxAgents.Items.Add(Agents[i].firstName + " " + Agents[i].lastName + "," + Agents[i].phoneSetLoginID);
                    }
                    //lBoxAgents.SelectedIndex = 0;
                    //SelectedAgentLoginID = Agents[0].phoneSetLoginID;
                }

            }
            catch (Exception Ex)
            {
                MessageBox.Show("An exception was caught while executing the GetAllAgents method. Details : " + Ex.Message);
            }
            oiChannelFactory.Close();
           
        }*/
        public void LoadAgentDetails()
        {
            string errorMessage = string.Empty;
            CCMAURL = "https://" + AACCIP + "/WebServices/OpenInterfaces/soap.svc";
            //OICCMA.ContactType[] contactTypeList;
            //OICCMA.Skillset[] skillsetList;
            OICCMA.UserDetails user;

            //OICCMA.ContactTypeAndRelatedSkillsets[] contactSkillsetList;
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            basicHttpBinding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
            basicHttpBinding.CloseTimeout = TimeSpan.FromMinutes(5);
            basicHttpBinding.MaxReceivedMessageSize = int.MaxValue;
            EndpointAddress endpointAddress = new EndpointAddress(CCMAURL);
            ChannelFactory<OICCMA.soap> oiChannelFactory = new ChannelFactory<OICCMA.soap>(basicHttpBinding, endpointAddress);
            OICCMA.soap oiService = oiChannelFactory.CreateChannel();

            using (OperationContextScope scope = new OperationContextScope((IContextChannel)oiService))
            {
                HttpRequestMessageProperty requestProperty = new HttpRequestMessageProperty();
                requestProperty.Headers["NTSSOCookie"] = CommToken;
                OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestProperty;

                try
                {
                    oiService.getUserDetails(out user, out errorMessage, AgentIn, AACCIP);


                    local_user = user;

                    //if (errorMessage != string.Empty)
                    //    MessageBox.Show("Web service getUserDetails returned an error " + errorMessage);

                    //if (user.voiceURI != "")
                    //    ckbSIPAgent.Checked = true;

                    //if (ckbSIPAgent.Checked)
                    //{
                    //    txtPersonnalDN.Text = user.voiceURI;
                    //    txtACDQueue.Text = user.iMURI;
                    //}
                    //else
                    //{
                    //    txtPersonnalDN.Text = user.personalDN;
                    //    txtACDQueue.Text = user.defaultACDQueue;
                    //}                    

                    //if (user.loggedIn)
                    //    txtLoginStatus.Text = "Logged In";
                    //else
                    //   txtLoginStatus.Text = "Logged Out";
                    //user.loggedIn = false;  // sip:68307@mac.org.il
                    user.voiceURI = voiceURI;
                    
                    oiService.updateUser( out  errorMessage, AACCIP, AgentIn, user);

                    MccMLogger.Info(" LoadAgentDetails " + AgentIn + "  "+user+" \n\n");
                    //if (errorMessage != string.Empty)
                    //   MessageBox.Show("Web service getUserDetails returned an error " + errorMessage);





                }
                catch (Exception Ex)
                {
                    //CommToken = null;
                    MccMLogger.Error("error LoadAgentDetails \n\n", Ex);
                    //MessageBox.Show("Error running GetUserdetails. Details: " + Ex.Message);
                    Cursor.Current = Cursors.Default;
                }
            }

            oiChannelFactory.Close();
           
        }
        public void AddAgentDetails()
        {
            string errorMessage = string.Empty;

            //OICCMA.ContactType[] contactTypeList;
            //OICCMA.Skillset[] skillsetList;
            OICCMA.UserDetails user = null;
            OICCMA.CCTUserDetails cctUser = new OICCMA.CCTUserDetails();
            CCMAURL = "https://" + AACCIP + "/WebServices/OpenInterfaces/soap.svc";

            //OICCMA.ContactTypeAndRelatedSkillsets[] contactSkillsetList;
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            basicHttpBinding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
            basicHttpBinding.CloseTimeout = TimeSpan.FromMinutes(5);
            basicHttpBinding.MaxReceivedMessageSize = int.MaxValue;
            EndpointAddress endpointAddress = new EndpointAddress(CCMAURL);
            ChannelFactory<OICCMA.soap> oiChannelFactory = new ChannelFactory<OICCMA.soap>(basicHttpBinding, endpointAddress);
            OICCMA.soap oiService = oiChannelFactory.CreateChannel();

            using (OperationContextScope scope = new OperationContextScope((IContextChannel)oiService))
            {
                HttpRequestMessageProperty requestProperty = new HttpRequestMessageProperty();
                requestProperty.Headers["NTSSOCookie"] = CommToken;
                OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestProperty;

                try
                {
                    //oiService.getUserDetails(out user, out errorMessage, AgentIn, AACCIP);
                 /*
                    cctUser.CCTLogin = AgentIn;
                    cctUser.CCTServerName = AACCIP;
                    */
                    int _agent = 0;
                    int.TryParse(AgentIn, out _agent);
                    oiService.getUserDetails(out user, out errorMessage, "0077", AACCIP);
                    user.loginID = AgentIn;
                    user.lastName = "last";
                    user.firstName = "first";
                    user.userName = "nissim";
                    user.password = "Q1w2e3r4!";
                    //user.assignedContactsandSkillsets[]=OICCMA.ContactTypeAndRelatedSkillsets.
                    user.userID = "NewUser";
                    user.userType = OICCMA.AGENT_TYPE.AGENT;
                    user.voiceURI = voiceURI;
                    
                    user.cctAgentEnabled = true;
                    user.agentCTIEnabled = true;
                    //user.assignedContactsandSkillsets[] = new OICCMA.a
                    //user.assignedContactsandSkillsets []= OICCMA.ContactType
                    oiService.addUser(out _agent, out errorMessage, AACCIP, user);
                    // oiService.addAgent(out _agent, out errorMessage, AACCIP, "user" + AgentIn, "aa", cctUser);
                    local_user = user;

                    if (errorMessage != string.Empty)
                        MessageBox.Show("Web service getUserDetails returned an error " + errorMessage);

                    MessageBox.Show("Web service local agenrid " + _agent); 
                    //if (user.voiceURI != "")
                    //    ckbSIPAgent.Checked = true;

                    //if (ckbSIPAgent.Checked)
                    //{
                    //    txtPersonnalDN.Text = user.voiceURI;
                    //    txtACDQueue.Text = user.iMURI;
                    //}
                    //else
                    //{
                    //    txtPersonnalDN.Text = user.personalDN;
                    //    txtACDQueue.Text = user.defaultACDQueue;
                    //}                    

                    //if (user.loggedIn)
                    //    txtLoginStatus.Text = "Logged In";
                    //else
                    //   txtLoginStatus.Text = "Logged Out";
                    //user.loggedIn = false;  // sip:68307@mac.org.il
                    user.voiceURI = voiceURI;

                    oiService.updateUser(out errorMessage, AACCIP, AgentIn, user);


                    if (errorMessage != string.Empty)
                        MessageBox.Show("Web service getUserDetails returned an error " + errorMessage);





                }
                catch (Exception Ex)
                {
                    //CommToken = null;
                    MessageBox.Show("Error running GetUserdetails. Details: " + Ex.Message);
                    Cursor.Current = Cursors.Default;
                }
            }

            oiChannelFactory.Close();

        }
        public void UpdateAgentDetails()
        {
            string errorMessage = string.Empty;

            //OICCMA.ContactType[] contactTypeList;
            //OICCMA.Skillset[] skillsetList;
            OICCMA.UserDetails user;
            CCMAURL = "https://" + AACCIP + "/WebServices/OpenInterfaces/soap.svc";
            //OICCMA.ContactTypeAndRelatedSkillsets[] contactSkillsetList;
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            basicHttpBinding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
            basicHttpBinding.CloseTimeout = TimeSpan.FromMinutes(5);
            basicHttpBinding.MaxReceivedMessageSize = int.MaxValue;
            EndpointAddress endpointAddress = new EndpointAddress(CCMAURL);
            ChannelFactory<OICCMA.soap> oiChannelFactory = new ChannelFactory<OICCMA.soap>(basicHttpBinding, endpointAddress);
            OICCMA.soap oiService = oiChannelFactory.CreateChannel();

            using (OperationContextScope scope = new OperationContextScope((IContextChannel)oiService))
            {
                HttpRequestMessageProperty requestProperty = new HttpRequestMessageProperty();
                requestProperty.Headers["NTSSOCookie"] = CommToken;
                OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestProperty;

                try
                {
                    /*
                    oiService.getUserDetails(out user, out errorMessage, AgentIn, AACCIP);
                    

                    local_user = user;

                    if (errorMessage != string.Empty)
                        MessageBox.Show("Web service getUserDetails returned an error " + errorMessage);

                    //if (user.voiceURI != "")
                    //    ckbSIPAgent.Checked = true;

                    //if (ckbSIPAgent.Checked)
                    //{
                    //    txtPersonnalDN.Text = user.voiceURI;
                    //    txtACDQueue.Text = user.iMURI;
                    //}
                    //else
                    //{
                    //    txtPersonnalDN.Text = user.personalDN;
                    //    txtACDQueue.Text = user.defaultACDQueue;
                    //}                    

                    //if (user.loggedIn)
                    //    txtLoginStatus.Text = "Logged In";
                    //else
                    //   txtLoginStatus.Text = "Logged Out";
                    //user.loggedIn = false;  // sip:68307@mac.org.il
                    */
                    user= new OICCMA.UserDetails(); 
                    user.voiceURI = voiceURI;

                    
                   oiService.updateUser(out errorMessage, AACCIP, AgentIn, user);


                    if (errorMessage != string.Empty)
                        MessageBox.Show("Web service getUserDetails returned an error " + errorMessage);





                }
                catch (Exception Ex)
                {
                    //CommToken = null;
                    MessageBox.Show("Error running GetUserdetails. Details: " + Ex.Message);
                    Cursor.Current = Cursors.Default;
                }
            }

            oiChannelFactory.Close();

        }

    }
}
