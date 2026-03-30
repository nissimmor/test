/*using System;
using System.ServiceModel;
using Topshelf;

namespace MyWcfService
{
    [ServiceContract(CallbackContract = typeof(IMyCallback))]
    public interface IMyService
    {
        [OperationContract]
        void DoWork();
    }

    public interface IMyCallback
    {
        [OperationContract]
        void OnCallback();
    }

    public class MyService : IMyService
    {
        public void DoWork()
        {
            Console.WriteLine("Doing work...");
            var callback = OperationContext.Current.GetCallbackChannel<IMyCallback>();
            callback.OnCallback();
        }
    }

    public class MyCallback : IMyCallback
    {
        public void OnCallback()
        {
            Console.WriteLine("Callback received");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var host = HostFactory.New(config =>
            {
                config.Service<MyService>(s =>
                {
                    s.ConstructUsing(() => new MyService());
                    s.WhenStarted(tc => tc.DoWork());
                    s.WhenStopped(tc => tc.DoWork());
                });
                //config.UseNetTcp();
                config.SetServiceName("MyWcfService");
                config.SetDisplayName("My WCF Service");
                config.SetDescription("This is a sample WCF service hosted using Topshelf");
            });

            host.Run();
        }
    }
}

*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace LogInService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    [ServiceContract(CallbackContract = typeof(IUpdateCallback))]
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<UpdateService>(s =>
                {
                    s.ConstructUsing(name => new UpdateService());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
                x.RunAsLocalSystem();
                x.StartAutomatically();
                x.SetServiceName("UpdateService");
                x.SetDisplayName("Update Service");
                x.SetDescription("A service that sends updates to clients");

                x.EnableServiceRecovery(r =>
                {
                    r.RestartService(1);
                    r.RestartService(1);
                    r.RestartService(1);
                    r.SetResetPeriod(1);
                });

               // x.UseNLog();

            });
        }
    }


  


 

  
}

