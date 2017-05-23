using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Task_3
{
    public class CancellationToken
    { 
        public Boolean IsCancellationRequested { get; private set; } 

        public void ThrowIfCancellationRequested()
        {
            if (IsCancellationRequested)
            {
                throw new AggregateException();
            }
        }

        public void Register(Action<Object> callback)
        {
            registered.Add(callback);
        }

        public void Cancel()
        {
            IsCancellationRequested = true;
            registered.AsParallel().ForAll(action => action(null));
        }

        private List<Action<object>> registered = new List<Action<object>>();
    }
}
