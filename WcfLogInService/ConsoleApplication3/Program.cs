using System;
using System.ServiceModel;
using Topshelf;
class Program
{
    static void Main(string[] args)
    {
        try
        {
            DuplexChannelFactory<ILoLiService> factory = new DuplexChannelFactory<ILoLiService>(new InstanceContext(new LoLiServiceCallback()), new NetTcpBinding(), "net.tcp://localhost:8523/LoLi_ExtToLoginID");
            ILoLiService service = factory.CreateChannel();
            Console.WriteLine(service.GetData("5"));
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
