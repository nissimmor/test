using System;
using System.Windows.Forms;
using System.ServiceModel;
namespace clientWinFormApp
{
    [ServiceContract(CallbackContract = typeof(IUpdateCallback))]
    class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ClientForm());
        }
    }



   
    


}