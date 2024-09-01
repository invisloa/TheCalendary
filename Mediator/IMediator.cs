using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kalendarzyk.Mediator
{
    public interface IMediator
    {
        void Notify(string message, object sender, object args = null);
        void Subscribe(string message, Action<object, object> callback);
        void Unsubscribe(string message, Action<object, object> callback);
    }

}
