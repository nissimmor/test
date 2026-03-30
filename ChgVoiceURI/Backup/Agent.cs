using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace LoginForm
{
    public partial class Agent : Form
    {
        private string TokenID;
        private string ccmaURL;
        private string AACCIP;
        private string SelectedAgentLoginID;

        public Agent(string Token, string URL)
        {
            InitializeComponent();

            TokenID = Token;
            ccmaURL = URL;

            AACCIP = string.Empty;
            SelectedAgentLoginID = string.Empty;

            TokenText.Text = "Athentication token: " + TokenID;           
            GetServerList();
            IPText.Text = "Agents below from AACC IP: " + AACCIP;
            GetAgentList();
        }

        private void Agent_Load(object sender, EventArgs e)
        {

        }

        public void GetServerList()
        {
            string errorMessage = string.Empty; 
            OICCMA.CCMSServer[] ccmsServers;

            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            basicHttpBinding.MaxReceivedMessageSize = int.MaxValue;
            EndpointAddress endpointAddress = new EndpointAddress(ccmaURL);
            ChannelFactory<OICCMA.soap> oiChannelFactory = new ChannelFactory<OICCMA.soap>(basicHttpBinding, endpointAddress);
            OICCMA.soap oiService = oiChannelFactory.CreateChannel();

            using (OperationContextScope scope = new OperationContextScope((IContextChannel)oiService))
            {
                HttpRequestMessageProperty requestProperty = new HttpRequestMessageProperty();
                requestProperty.Headers["NTSSOCookie"] = TokenID;
                OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestProperty;

                try
                {
                    int count = oiService.getServerList(out ccmsServers, out errorMessage);
                    if (errorMessage != "")
                        MessageBox.Show("Error message was populated with " + errorMessage);
                    else
                    {
                        foreach (OICCMA.CCMSServer srvr in ccmsServers)
                        {
                            CboServers.Items.Add(srvr.HostIpAddress + ", " + srvr.Name + ", " + srvr.DisplayName);
                        }
                        CboServers.SelectedIndex = 0;
                        AACCIP = CboServers.Text.Split(',')[0];
                    }
                }
                catch (Exception Ex)
                {
                    MessageBox.Show("Error retrieving server list. Details: " + Ex.Message);
                }
                oiChannelFactory.Close();
            }
        }

        public void GetAgentList()
        {
            string errorMessage = null;
            OICCMA.AgentFullDetails[] Agents;

            this.Cursor = Cursors.WaitCursor;

            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            basicHttpBinding.MaxReceivedMessageSize = int.MaxValue;
            EndpointAddress endpointAddress = new EndpointAddress(ccmaURL);
            ChannelFactory<OICCMA.soap> oiChannelFactory = new ChannelFactory<OICCMA.soap>(basicHttpBinding, endpointAddress);
            OICCMA.soap oiService = oiChannelFactory.CreateChannel();

            try
            {
                using (OperationContextScope scope = new OperationContextScope((IContextChannel)oiService))
                {
                    HttpRequestMessageProperty requestProperty = new HttpRequestMessageProperty();
                    requestProperty.Headers["NTSSOCookie"] = TokenID;
                    OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestProperty;

                    oiService.getAgentsList(out Agents, out errorMessage, AACCIP);

                    if (errorMessage != null && errorMessage != "")
                    {
                        MessageBox.Show("Error text was received from the getAllAgents method. Details: " + errorMessage);
                    }
                    lBoxAgents.Items.Clear();
                    for (int i = 0; i < Agents.Length; i++)
                    {
                        lBoxAgents.Items.Add(Agents[i].firstName + " " + Agents[i].lastName + "," + Agents[i].phoneSetLoginID);
                    }
                    lBoxAgents.SelectedIndex = 0;
                    SelectedAgentLoginID = Agents[0].phoneSetLoginID;
                }

            }
            catch (Exception Ex)
            {
                MessageBox.Show("An exception was caught while executing the GetAllAgents method. Details : " + Ex.Message);
            }
            oiChannelFactory.Close();
            this.Cursor = Cursors.Default;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            return;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AgentInfo myAgentInfo = new AgentInfo(SelectedAgentLoginID, AACCIP, TokenID, ccmaURL);
            if (myAgentInfo.ShowDialog() == DialogResult.OK)
            {

            }
        }

        private void CboServers_SelectedIndexChanged(object sender, EventArgs e)
        {
            AACCIP = CboServers.Text.Split(',')[0];
            GetAgentList();
        }

        private void lBoxAgents_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedAgentLoginID = lBoxAgents.Text.Split(',')[1];
        }


    }
}
