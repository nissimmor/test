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
    public partial class AgentInfo : Form
    {
        private string AgentIn;
        private string AACCIP;
        private string CommToken;
        private string CCMAURL;

        private OICCMA.UserDetails local_user = new OICCMA.UserDetails();

        public AgentInfo(string AgentID, string AACC_IP, string TheToken, string TheURL)
        {
            InitializeComponent();

            AgentIn = AgentID;
            AACCIP = AACC_IP;
            CommToken = TheToken;
            CCMAURL = TheURL;

            LoadAgentDetails();
        }

        public void LoadAgentDetails()
        {
            string errorMessage = string.Empty;

            this.Cursor = Cursors.WaitCursor;

            OICCMA.ContactType[] contactTypeList;
            OICCMA.Skillset[] skillsetList;
            OICCMA.UserDetails user;

            OICCMA.ContactTypeAndRelatedSkillsets[] contactSkillsetList;
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
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
                    bool result = oiService.getSkillsetsList(out skillsetList, out errorMessage, AACCIP);

                    if (errorMessage != string.Empty)
                        MessageBox.Show("Web service getSkillsetsList returned an error " + errorMessage);

                    if (!result)
                        MessageBox.Show("getSkillsetsList returned false!");

                    dgvSkillsets.Rows.Clear();

                    foreach (OICCMA.Skillset skillset in skillsetList)
                    {
                        int rowindex = dgvSkillsets.Rows.Add();

                        dgvSkillsets.Rows[rowindex].Cells[0].Value = skillset.Name;
                        dgvSkillsets.Rows[rowindex].Cells[1].Value = "Unassigned";
                        dgvSkillsets.Rows[rowindex].Cells[2].Value = skillset.ContactTypeID.ToString();
                        dgvSkillsets.Rows[rowindex].Cells[3].Value = skillset.ID.ToString();
                    }
                }
                catch (Exception Ex)
                {
                    MessageBox.Show("Error calling Get ContactTypes. Details: " + Ex.Message);
                }

                try
                {
                    int result = oiService.getContactTypes(out contactTypeList, out errorMessage, AACCIP);

                    if (errorMessage != string.Empty)
                        MessageBox.Show("Web service getContactTypes returned an error " + errorMessage);

                    cklbContactTypes.Items.Clear();

                    foreach (OICCMA.ContactType conType in contactTypeList)
                    {
                        cklbContactTypes.Items.Add(conType.ID + "," + conType.Name);
                    }
                }
                catch (Exception Ex)
                {
                    MessageBox.Show("Error calling Get ContactTypes. Details: " + Ex.Message);
                }

                try
                {                  
                    oiService.getUserDetails(out user, out errorMessage, AgentIn, AACCIP);

                    local_user = user;

                    if (errorMessage != string.Empty)
                        MessageBox.Show("Web service getUserDetails returned an error " + errorMessage);

                    if (user.voiceURI != "")
                        ckbSIPAgent.Checked = true;

                    if (ckbSIPAgent.Checked)
                    {
                        txtPersonnalDN.Text = user.voiceURI;
                        txtACDQueue.Text = user.iMURI;
                    }
                    else
                    {
                        txtPersonnalDN.Text = user.personalDN;
                        txtACDQueue.Text = user.defaultACDQueue;
                    }

                    ckbCTIEnabled.Checked = user.agentCTIEnabled;
                    txtPositionID.Text = user.positionID.ToString();
                    txtTNName.Text = user.tnName;
                    txtUserID.Text = user.userID;
                    txtFirstName.Text = user.firstName;
                    txtLastName.Text = user.lastName;
                    txtTitle.Text = user.title;
                    txtDepartment.Text = user.department;
                    txtLanguage.Text = user.languageID.ToString();
                    txtComment.Text = user.comment;
                    txtUserType.Text = user.userType.ToString();
                    txtLoginID.Text = user.loginID;
                    txtAgentKey.Text = user.agentKey.ToString();

                    if (user.loggedIn)
                        txtLoginStatus.Text = "Logged In";
                    else
                        txtLoginStatus.Text = "Logged Out";

                    contactSkillsetList = user.assignedContactsandSkillsets;
                    if (contactSkillsetList != null)
                    {
                        foreach (OICCMA.ContactTypeAndRelatedSkillsets conType in contactSkillsetList)
                        {
                            if (conType.isAssigned)
                            {
                                for (int i = 0; i < cklbContactTypes.Items.Count; i++)
                                {
                                    if (cklbContactTypes.Items[i].ToString().Split(',')[0] == conType.ID.ToString())
                                    {
                                        int y = cklbContactTypes.Items.IndexOf(conType.ID + "," + conType.Name);
                                        cklbContactTypes.SetItemCheckState(y, CheckState.Checked);
                                    }
                                }
                                foreach (OICCMA.SkillsetFullDetails skillset in conType.skillsetsOfThisContactType)
                                {
                                    foreach (DataGridViewRow dvr in dgvSkillsets.Rows)
                                    {
                                        if (dvr.Cells[0].Value.ToString() == skillset.Name)
                                        {
                                            if (skillset.Priority == 0)
                                                dvr.Cells[1].Value = skillset.Status;
                                            else 
                                                dvr.Cells[1].Value = skillset.Priority.ToString();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception Ex)
                {
                    MessageBox.Show("Error running GetUserdetails. Details: " + Ex.Message);
                    Cursor.Current = Cursors.Default;
                }
            }

            oiChannelFactory.Close();
            this.Cursor = Cursors.Default;
        }

        private void AgentInfo_Load(object sender, EventArgs e)
        {

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            return;
        }

        private void btnCommit_Click(object sender, EventArgs e)
        {
            string errorMessage = string.Empty;
            CheckState val;
            int contactTypeCounter = 0;

            this.Cursor = Cursors.WaitCursor;

            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
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

                for (int j = 0; j < cklbContactTypes.Items.Count; j++)
                {
                    val = cklbContactTypes.GetItemCheckState(j);
                    if (val == CheckState.Checked)
                        contactTypeCounter++;
                }
                OICCMA.ContactTypeAndRelatedSkillsets[] contactListArray = new OICCMA.ContactTypeAndRelatedSkillsets[cklbContactTypes.Items.Count];

                int counter = 0;
                for (int j = 0; j < cklbContactTypes.Items.Count; j++)
                {
                    val = cklbContactTypes.GetItemCheckState(j);

                    contactListArray[counter] = new OICCMA.ContactTypeAndRelatedSkillsets();
                    contactListArray[counter].ID = int.Parse(cklbContactTypes.Items[j].ToString().Split(',')[0]);
                    contactListArray[counter].Name = cklbContactTypes.Items[j].ToString().Split(',')[1];
                    if (val == CheckState.Checked)
                    {
                        contactListArray[counter].ON = "1";
                        contactListArray[counter].isAssigned = true;
                    }
                    else
                    {
                        contactListArray[counter].ON = "0";
                        contactListArray[counter].isAssigned = false;
                    }
                    counter++;

                }

                OICCMA.SkillsetFullDetails skillset;

                List<OICCMA.SkillsetFullDetails> skillsetsToAdd = new List<OICCMA.SkillsetFullDetails>();
                foreach (OICCMA.ContactTypeAndRelatedSkillsets selectedContactTypes in contactListArray)
                {
                    if (selectedContactTypes.isAssigned)
                    {
                        for (int i = 0; i < dgvSkillsets.Rows.Count; i++)
                        {
                            if (int.Parse(dgvSkillsets.Rows[i].Cells[2].Value.ToString()) == selectedContactTypes.ID)
                            {
                                if (dgvSkillsets.Rows[i].Cells[1].FormattedValue.ToString() != "")
                                {
                                    skillset = new OICCMA.SkillsetFullDetails();
                                    skillset.ContactTypeID = int.Parse(dgvSkillsets.Rows[i].Cells[2].Value.ToString());
                                    skillset.ID = int.Parse(dgvSkillsets.Rows[i].Cells[3].Value.ToString());
                                    if (dgvSkillsets.Rows[i].Cells[1].FormattedValue.ToString().ToLower().Trim() == "unassigned")
                                    {
                                        skillset.Priority = 49;
                                        skillset.Status = OICCMA.SkillsetStatus.UNASSIGNED;
                                    }
                                    else if (dgvSkillsets.Rows[i].Cells[1].FormattedValue.ToString().ToLower().Trim() == "standby")
                                    {
                                        skillset.Priority = 0;
                                        skillset.Status = OICCMA.SkillsetStatus.STANDBY;
                                    }
                                    else
                                    {
                                        try
                                        {
                                            skillset.Priority = int.Parse(dgvSkillsets.Rows[i].Cells[1].FormattedValue.ToString().Trim());
                                        }
                                        catch (Exception)
                                        {
                                            MessageBox.Show("Invalid skillset priority " + dgvSkillsets.Rows[i].Cells[1].Value.ToString());
                                            return;
                                        }
                                        skillset.Status = OICCMA.SkillsetStatus.ACTIVE;
                                    }
                                    skillsetsToAdd.Add(skillset);
                                }
                            }
                        }
                    }

                    selectedContactTypes.skillsetsOfThisContactType = skillsetsToAdd.ToArray();
                    skillsetsToAdd.Clear();
                }

                local_user.assignedContactsandSkillsets = contactListArray;
                if(local_user.password == "") local_user.password = "1234";

                try
                {
                    int result = oiService.updateUser(out errorMessage, AACCIP, AgentIn, local_user);

                    if (errorMessage != string.Empty)
                        MessageBox.Show("Error returned from UpdateUser: " + errorMessage);
                }
                catch (Exception Ex)
                {
                    MessageBox.Show("Error updating agent. Details: " + Ex.Message);
                }

                oiChannelFactory.Close();
                this.Cursor = Cursors.Default;
            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            LoadAgentDetails();
        }
    }
}
