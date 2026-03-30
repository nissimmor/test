using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace LogInService
{
    public class UpdateService : IUpdateService
    {
        public void Subscribe()
        {
            IUpdateCallback callback = OperationContext.Current.GetCallbackChannel<IUpdateCallback>();
            // Add the callback to a list of subscribers
        }

        public void Unsubscribe()
        {
            IUpdateCallback callback = OperationContext.Current.GetCallbackChannel<IUpdateCallback>();
            // Remove the callback from the list of subscribers
        }

        public void SendUpdates()
        {
            // Send updates to all subscribers
        }

        internal bool Start()
        {
            throw new NotImplementedException();
        }

        internal bool Stop()
        {
            throw new NotImplementedException();
        }
    }
}
