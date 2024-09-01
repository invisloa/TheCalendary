using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kalendarzyk.Mediator
{
    public class EventMediator : IMediator
    {
        private readonly Dictionary<string, List<Action<object, object>>> _subscriptions = new();

        public void Notify(string message, object sender, object args = null)
        {
            if (_subscriptions.ContainsKey(message))
            {
                foreach (var callback in _subscriptions[message])
                {
                    callback(sender, args);
                }
            }
        }

        public void Subscribe(string message, Action<object, object> callback)
        {
            if (!_subscriptions.ContainsKey(message))
            {
                _subscriptions[message] = new List<Action<object, object>>();
            }
            _subscriptions[message].Add(callback);
        }

        public void Unsubscribe(string message, Action<object, object> callback)
        {
            if (_subscriptions.ContainsKey(message))
            {
                _subscriptions[message].Remove(callback);
            }
        }
    }
