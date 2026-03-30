using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.Text;
using MCCMLogger;
using System.Reflection;
using System.Threading.Tasks;
using System.IO;
using LoginForm;

namespace WcfLogInService
{
    class LoginSvc
    {
        internal static ServiceHost serviceHost = null;
        public void Start()
        {
            /*
            ServiceHost host = new ServiceHost(typeof(LoLiService));
            host.AddServiceEndpoint(typeof(ILoLiService), new NetTcpBinding(), "net.tcp://localhost:8000/SampleService");
            host.Open();
            MccMLogger.Info("Service started. Press enter to stop.");
            Console.ReadLine();
            host.Close();
             */

            string exePath = Assembly.GetExecutingAssembly().Location;
            string exeDir = Path.GetDirectoryName(exePath);
            AppDomain.CurrentDomain.SetData("APPBASE", exeDir);
            Task task = Task.Run(() => loliReqTask());


            
        }

        private static void loliReqTask()
        {
            ServicesSection servicesSection = (ServicesSection)ConfigurationManager.GetSection("system.serviceModel/services");
            ServiceEndpointElement endpoint = servicesSection.Services[0].Endpoints[0];
            string endpointStr = servicesSection.Services[0].Host.BaseAddresses[0].BaseAddress;
            string binding = endpoint.Binding;
            try
            {
                serviceHost = new ServiceHost(typeof(LoLiService));

                ServiceThrottlingBehavior stb = new ServiceThrottlingBehavior();
                stb.MaxConcurrentCalls = 6000;
                stb.MaxConcurrentInstances = 6000;
                stb.MaxConcurrentSessions = 6000;
                serviceHost.Description.Behaviors.Add(stb);
                serviceHost.Faulted += OnFaultState;
                InitLogger();

                // Open the host and start listening for incoming messages.
                serviceHost.Open();
               // AdressChg adressChg = new AdressChg(true); //432
               // adressChg.LoginAuth();
                MccMLogger.Info("LoLi is ready ...");
                //Console.ReadLine();

            }
            catch (Exception ex)
            {
                MccMLogger.Error("Failed to start CP service !!! \n\n", ex);

               // MccMLogger.Info("Press the Enter key to terminate service.");
               // Console.ReadLine();
            }
        }

        private static void OnFaultState(object sender, EventArgs e)
        {
            MccMLogger.Error("OnFaultState!!");
        }


        private static void InitLogger()
        {
            MccMLogger.SetDebugMode();

            MccMLogger.Info("\n\n");
            MccMLogger.Info("**********************************************************");
            MccMLogger.Info("MccM LoginSvc Dialer (LoLi):");
            MccMLogger.Info("   Assembly: " + System.Reflection.Assembly.GetExecutingAssembly().Location);
            MccMLogger.Info("   Version: " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
            MccMLogger.Info("   Server: " + Environment.MachineName);
            MccMLogger.Info("   Exec. User: " + Environment.UserDomainName + "\\" + Environment.UserName);
            MccMLogger.Info("**********************************************************");
        }

        public void Stop()
        {
            try
            {
                serviceHost.Close();
                MccMLogger.Info("============================================================");
                MccMLogger.Info("                  Service ended");
                MccMLogger.Info("=============================================================");
            }
            catch (Exception) { }
        }
    }
}
