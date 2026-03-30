using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WcfLogInService;

namespace clientWinFormApp
{
    public class ClientForm : Form
    {
        private LoLi_ServiceInbound updateClient;
        private Label updateLabel;

        public ClientForm()
        {
            updateLabel = new Label();
            this.Controls.Add(updateLabel);
            InstanceContext context = new InstanceContext(this);
            try
            {
                // Create a client for the service
                NetTcpBinding binding = new NetTcpBinding();
                EndpointAddress endpoint = new EndpointAddress("net.tcp://localhost:8523/LoLi_ExtToLoginID");
                updateClient = new DuplexChannelFactory<LoLi_ServiceInbound>(context, binding, endpoint).CreateChannel();
                updateClient.JoinTheConversation("0024");
            }
            catch (Exception ex)
            {
                updateLabel.Text = ex.StackTrace;
            }
        }

        public void UpdateReceived(string update)
        {
            updateLabel.Text = update;
        }
    }
}
