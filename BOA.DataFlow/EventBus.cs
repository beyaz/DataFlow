using System;
using System.Collections;
using System.Collections.Generic;

namespace BOA.DataFlow
{
    /// <summary>
    ///     The event bus
    /// </summary>
    sealed class EventBus
    {
        #region Fields
        /// <summary>
        ///     The subscribers
        /// </summary>
        readonly Dictionary<string, ArrayList> Subscribers = new Dictionary<string, ArrayList>();
        #endregion

        #region Public Methods
        /// <summary>
        ///     Publishes the specified event name.
        /// </summary>
        public void Publish(string eventName)
        {
            if (!Subscribers.ContainsKey(eventName))
            {
                return;
            }

            var arrayList = Subscribers[eventName];
            foreach (Action action in arrayList)
            {
                action();
            }
        }

        /// <summary>
        ///     Subscribes the specified event name.
        /// </summary>
        public void Subscribe(string eventName, Action action)
        {
            if (Subscribers.ContainsKey(eventName) == false)
            {
                Subscribers[eventName] = ArrayList.Synchronized(new ArrayList());
            }

            var concurrentBag = Subscribers[eventName];
            if (concurrentBag.Contains(action))
            {
                throw new InvalidOperationException("Action is already subsribed to event. @eventName:" + eventName);
            }

            concurrentBag.Add(action);
        }

        /// <summary>
        ///     Uns the subscribe.
        /// </summary>
        public void UnSubscribe(string eventName, Action action)
        {
            if (Subscribers.ContainsKey(eventName))
            {
                Subscribers[eventName].Remove(action);
            }
        }
        #endregion
    }
}