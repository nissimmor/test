using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace clientWinFormApp
{

      public interface IUpdateService
    {
        [OperationContract]
        void Subscribe();

        [OperationContract]
        void Unsubscribe();
    }
}
