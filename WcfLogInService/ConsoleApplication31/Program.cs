using System;
using System.ServiceModel;
using Topshelf;
using WcfLogInService;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            DuplexChannelFactory<ILoLiService> factory = new DuplexChannelFactory<ILoLiService>(new InstanceContext(new LoLiServiceCallback()), new NetTcpBinding(), "net.tcp://localhost:8523/LoLi_ExtToLoginID");
            ILoLiService service = factory.CreateChannel();
            Console.WriteLine(service.GetData("5"));
            //Console.WriteLine(service.Mala_EWT_Proxy("Nurse", "", "", "", ""));
            Console.WriteLine(service.Mala_InsertNewProxy("905433333", "27000", "stam", "90546540077", 444, "stam", "Nurse", "000018", 3,"test"));
           
            Console.WriteLine(service.TB_CloseCB_mac("90546540077","000018", "test"));


            //Console.WriteLine(service.AgentDB("8tmoma-pituh2", "mor_nisim"));
            // Console.WriteLine(service.AgentLoLi("0024", "10.71.9.37", "68998", "mor_nissim","ascribe"));
            Console.ReadLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}

class LoLiServiceCallback : ILoLiServiceCallback
{
    public void OnDataReceived(string data)
    {
        Console.WriteLine(data);
    }
}
