using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.Text;
using MCCMLogger;


namespace WcfLogInService
{
    class LoginSvc
    {
        internal static ServiceHost serviceHost = null;
        public void Start()
        {

            ServicesSection servicesSection = (ServicesSection)ConfigurationManager.GetSection("system.serviceModel/services");
            ServiceEndpointElement endpoint = servicesSection.Services[0].Endpoints[0];
            string endpointStr = servicesSection.Services[0].Host.BaseAddresses[0].BaseAddress;
            string binding = endpoint.Binding;
            try
            {
                serviceHost = new ServiceHost(typeof(LoLi_MessageService));

                ServiceThrottlingBehavior stb = new ServiceThrottlingBehavior();
                stb.MaxConcurrentCalls = 6000;
                stb.MaxConcurrentInstances = 6000;
                stb.MaxConcurrentSessions = 6000;
                serviceHost.Description.Behaviors.Add(stb);
                serviceHost.Faulted += OnFaultState;
                InitLogger();

                // Open the host and start listening for incoming messages.
                serviceHost.Open();
               
                MccMLogger.Info("LoLi is ready ...");
                Console.ReadLine();

            }
            catch (Exception ex)
            {
                MccMLogger.Error("Failed to start CP service !!! \n\n", ex);

                Console.WriteLine("Press the Enter key to terminate service.");
                Console.ReadLine();
            }
        }

        private void OnFaultState(object sender, EventArgs e)
        {
            MccMLogger.Error("OnFaultState!!");
        }


        private void InitLogger()
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
