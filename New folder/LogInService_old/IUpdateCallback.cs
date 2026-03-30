using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace LogInService
{
    public interface IUpdateCallback
    {
        [OperationContract]
        void UpdateReceived(string update);
    }

}
