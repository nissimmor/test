using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Windows.Forms;
using WcfLogInService;
using System.Data.Odbc;

namespace FindandChgAaccExt
{
    public partial class Form1 : Form
    {
        DuplexChannelFactory<ILoLiService> factory ;
        ILoLiService service ;
        public Form1()
        {
            factory = new DuplexChannelFactory<ILoLiService>(new InstanceContext(new LoLiServiceCallback()), new NetTcpBinding(), "net.tcp://172.26.30.53:8523/LoLi_ExtToLoginID");
            service = factory.CreateChannel();
            InitializeComponent();
        }

        private void b_findext_Click(object sender, EventArgs e)
        {
            try
            {
                string agentLogin = "1";
                string extension = t_ext.Text;// "811044";
                string surName = "";
                string givenName = "";
                OdbcConnection odbConnection = null;
                string queryString = "SELECT TelsetLoginID, SurName, GivenName FROM dbo.Agent WHERE(URI = 'Voice: sip:" + extension + "@mac.org.il')";
                string odbconnectionString = "Dsn=CCMS_STAT_A;UID=sysadmin;PWD=avaya1";                   
                if (odbConnection == null)
                    odbConnection = new OdbcConnection(odbconnectionString);
                if (odbConnection.State != System.Data.ConnectionState.Open)
                    odbConnection.Open();
                string query = $"{queryString}";
                using (OdbcCommand odbcCommand = new OdbcCommand(query, odbConnection))
                using (OdbcDataReader reader = odbcCommand.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        try
                        {                            
                            agentLogin = reader[0].ToString();
                            surName = reader[1].ToString();
                            givenName = reader[2].ToString();
                        }
                        catch (Exception ex)
                        {
                           // Console.WriteLine(ex.Source);
                        }
                    }
                }
                odbConnection.Close();
                //Console.WriteLine(service.GetData("5"));
                //Console.WriteLine(service.Mala_EWT_Proxy("Nurse", "", "", "", ""));
                //Console.WriteLine(service.Mala_InsertNewProxy("905433333", "27000", "stam", "90546540077", 444, "stam", "Nurse", "000018", 3, "test"));

                //Console.WriteLine(service.TB_CloseCB_mac("90546540077", "000018", "test"));
                /* string returns = service.AgentDB(t_ext.Text, "mor_nisim");
                string[] results = returns.Split(';');
                if (results.Length < 1)
                {
                    MessageBox.Show("Not Found");
                    return;
                }
                string[] result = results[1].Split(',');
                if ((result.Length < 1) || (result[0].Length < 1))
                {
                    MessageBox.Show("Not Found");
                    return;
                }
                panel1.Visible = true;
                l_name.Text  = result[0];
                string [] loginid = result[1].Split('|');
                l_loginId.Text = loginid[0];
                t_newext.Text = loginid[0];  */
                if (agentLogin.Length < 3)
                {
                    MessageBox.Show("Not Found");
                    return;
                }
                panel1.Visible = true;
                l_name.Text =surName+" "+givenName;                
                l_loginId.Text = agentLogin;
                t_newext.Text = agentLogin;
                // Console.WriteLine(service.AgentDB("0088", "mor_nisim"));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Source);
            }
            // Console.WriteLine(service.AgentLoLi("0024", "10.71.9.37", "68998", "mor_nissim","ascribe"));


        }

        private void b_chgext_Click(object sender, EventArgs e)
        {
            string result = service.AgentLoLi(l_loginId.Text, "localhost", t_newext.Text, l_name.Text, "free");
            panel1.Visible = false;
            MessageBox.Show(" OK " + result);
            t_ext.Text = "";
            
        }

        private void b_chgext_Click_1(object sender, EventArgs e)
        {
            string result = service.AgentLoLi(l_loginId.Text, "localhost", t_newext.Text, l_name.Text, "free");
            panel1.Visible = false;
            MessageBox.Show(" OK " + result);
            t_ext.Text = "";

        }

    }

    class LoLiServiceCallback : ILoLiServiceCallback
    {
        public void OnDataReceived(string data)
        {
            Console.WriteLine(data);
        }
    }

}
