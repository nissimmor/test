using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using LoginForm;
using LoginForm.CCMAlogin;

namespace CCMASample
{
    public partial class LoginForm : Form
    {
        AuthenticationService authenticate;
        string ssoToken = string.Empty;
        string error = string.Empty;

        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            authenticate = new AuthenticationService();
            authenticate.Url = "http://" + CCMAIP.Text + "/WebServices/Authentication/Service.asmx";
            try
            {
                if (!authenticate.AuthenticateUser(UserName.Text, Password.Text, out ssoToken, out error))
                {
                    MessageBox.Show("Error logging in user " + UserName.Text + ". Error: " + error);
                    return;
                }
                else
                {
                    this.Cursor = Cursors.WaitCursor;
                    Agent myAgent = new Agent(ssoToken, "http://" + CCMAIP.Text + "/WebServices/OpenInterfaces/soap.svc");
                    this.Cursor = Cursors.Default;

                    if (myAgent.ShowDialog() == DialogResult.OK)
                    {
                        ssoToken = string.Empty; //logout
                    }

                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show("An exception was raised while attempting to authenticate user. Details: " + Ex.Message);
            }
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {

        }
    }
}
