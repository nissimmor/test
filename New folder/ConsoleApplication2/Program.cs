using System;
using System.ServiceModel;
using Topshelf;

namespace WcfLogInService
{
   
    class Program
    {
        static void Main(string[] args)
        {
            string serviceName = "MccM_LoginSvc";
            if (serviceName == null || serviceName.Trim() == "")
                serviceName = "MccM LoginSvc to ccms --> Call LoLi Manager";
            TopshelfExitCode exit = HostFactory.Run(x =>
            {
                x.Service<LoginSvc>(s =>
                {
                    s.ConstructUsing(name => new LoginSvc());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
                x.RunAsLocalService();

                x.StartAutomatically();
                x.SetDescription("MccM LoginSvc to ccms --> assaign extension to loginID");
                x.SetDisplayName(serviceName);
                x.SetServiceName(serviceName);
                x.StartAutomatically();
            });





           
        }
    }
}