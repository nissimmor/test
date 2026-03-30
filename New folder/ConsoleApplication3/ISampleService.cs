using System;
using System.ServiceModel;
using Topshelf;

[ServiceContract(CallbackContract = typeof(ILoLiServiceCallback))]
public interface ILoLiService
{
    [OperationContract]
    string GetData(string value);

    [OperationContract]
    string GetData1(string value);
}

public interface ILoLiServiceCallback
{
    [OperationContract]
    void OnDataReceived(string data);
}

public class LoLiService : ILoLiService
{
    public string GetData(string value)
    {
        ILoLiServiceCallback callback = OperationContext.Current.GetCallbackChannel<ILoLiServiceCallback>();
        callback.OnDataReceived(string.Format("You entered: {0}", value));
        return string.Format("You entered: {0}", value);
    }

    public string GetData1(string value)
    {
        ILoLiServiceCallback callback = OperationContext.Current.GetCallbackChannel<ILoLiServiceCallback>();
        callback.OnDataReceived(string.Format("Duplicate method: You entered: {0}", value));
        return string.Format("Duplicate method: You entered: {0}", value);
    }
}
