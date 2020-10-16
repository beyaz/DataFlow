using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BOA.DataFlow
{
    /// <summary>
    ///     The data context
    /// </summary>
    [DebuggerTypeProxy(typeof(DataContextDebugView))]
    [Serializable]
    public class DataContext : ICollection<DataContextEntry>
    {
        #region Fields
        /// <summary>
        ///     The dictionary
        /// </summary>
        internal readonly Dictionary<string, DataContextEntry> dictionary = new Dictionary<string, DataContextEntry>();

        /// <summary>
        ///     The layer names
        /// </summary>
        internal readonly List<string> layerNames = new List<string> {"Root"};

        /// <summary>
        ///     The forward map
        /// </summary>
        readonly Dictionary<string, string> forwardMap = new Dictionary<string, string>();

        /// <summary>
        ///     The get handlers
        /// </summary>
        readonly Dictionary<string, Func<DataContext, object>> getHandlers = new Dictionary<string, Func<DataContext, object>>();
        #endregion

        #region Public Properties
        /// <summary>
        ///     Gets the name of the current layer.
        /// </summary>
        public string CurrentLayerName
        {
            get
            {
                return layerNames.Last();
            }
        }
        
        /// <summary>
        ///     Gets a value indicating whether this instance is empty.
        /// </summary>
        public bool IsEmpty
        {
            get { return dictionary.Count == 0; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        ///     Adds the specified data key.
        /// </summary>
        public void Add<T>(DataKey<T> dataKey, T value)
        {
            CheckKeyIsNotForwarded(dataKey);

            if (dictionary.ContainsKey(dataKey.Id))
            {
                throw new DataFlowException($"Data key should remove before set operation. Data key is '{dataKey}'");
            }

            dictionary[dataKey.Id] = new DataContextEntry(dataKey.Id, LayerHelper.GetCurrentLayerId(layerNames), value);

            OnInserted(dataKey);
        }

        /// <summary>
        ///     Closes the current layer.
        /// </summary>
        public void CloseCurrentLayer()
        {
            if (layerNames.Count <= 0)
            {
                throw new DataFlowException("There is no layer to close.");
            }

            var currentLayerId = LayerHelper.GetCurrentLayerId(layerNames);

            var removeList     = dictionary.Values.Where(p => p.Layer == currentLayerId).Select(p => p.Key).ToList();

            foreach (var key in removeList)
            {
                dictionary.Remove(key);
            }

            layerNames.RemoveAt(layerNames.Count-1);
        }

        /// <summary>
        ///     Determines whether [contains] [the specified data key].
        /// </summary>
        public bool Contains<T>(DataKey<T> dataKey)
        {
            var id = GetId(dataKey);

            return dictionary.ContainsKey(id);
        }

        /// <summary>
        ///     Forwards the key.
        /// </summary>
        public void ForwardKey<T>(DataKey<T> sourceKey, DataKey<T> targetKey)
        {
            if (dictionary.ContainsKey(sourceKey.Id))
            {
                throw new DataFlowException($"Key is already in context. Key is {sourceKey}");
            }

            forwardMap.Add(sourceKey.Id, targetKey.Id);
        }

        /// <summary>
        ///     Gets the specified data key.
        /// </summary>
        public T Get<T>(DataKey<T> dataKey)
        {
            if (dataKey == null)
            {
                throw new ArgumentNullException(nameof(dataKey));
            }

            var id = GetId(dataKey);

            DataContextEntry dataContextEntry = null;

            if (dictionary.TryGetValue(id, out dataContextEntry))
            {
                return (T) dataContextEntry.Value;
            }

            Func<DataContext, object> getHandlerFunc = null;
            if (getHandlers.TryGetValue(id, out getHandlerFunc))
            {
                return (T) getHandlerFunc(this);
            }

            throw NoDataFoundException(dataKey);
        }

        /// <summary>
        ///     Opens the new layer.
        /// </summary>
        public void OpenNewLayer(string layerName)
        {
            if (layerName == null)
            {
                throw new ArgumentNullException(nameof(layerName));
            }

            layerNames.Add(layerName);
        }

        /// <summary>
        ///     Removes the specified data key.
        /// </summary>
        public void Remove<T>(DataKey<T> dataKey)
        {
            DataContextEntry dataContextEntry = null;
            if (dictionary.TryGetValue(dataKey.Id, out dataContextEntry))
            {
                if (dataContextEntry.Layer != LayerHelper.GetCurrentLayerId(layerNames))
                {
                    throw new DataFlowException($"Other layer variables can not be remove. CurrentLayer: {LayerHelper.GetCurrentLayerId(layerNames)}, TargetLayer: {dataContextEntry.Layer}");
                }

                dictionary.Remove(dataKey.Id);

                OnRemoved(dataKey);
                return;
            }

            throw NoDataFoundException(dataKey);
        }

        /// <summary>
        ///     Setups the get.
        /// </summary>
        public void SetupGet<T>(DataKey<T> dataKey, Func<DataContext, T> getHandler)
        {
            var id = GetId(dataKey);

            getHandlers.Add(id, c => getHandler(c));
        }

        /// <summary>
        ///     Tries the get.
        /// </summary>
        public T TryGet<T>(DataKey<T> dataKey)
        {
            var id = GetId(dataKey);

            DataContextEntry dataContextEntry = null;
            if (dictionary.TryGetValue(id, out dataContextEntry))
            {
                return (T) dataContextEntry.Value;
            }

            return default(T);
        }

        /// <summary>
        ///     Tries the remove.
        /// </summary>
        public bool TryRemove<T>(DataKey<T> key)
        {
            if (Contains(key))
            {
                Remove(key);

                return true;
            }

            return false;
        }

        /// <summary>
        ///     Updates the specified data key.
        /// </summary>
        public void Update<T>(DataKey<T> dataKey, T value)
        {
            CheckKeyIsNotForwarded(dataKey);

            if (dictionary.ContainsKey(dataKey.Id))
            {
                Remove(dataKey);
            }

            dictionary[dataKey.Id] = new DataContextEntry(dataKey.Id, LayerHelper.GetCurrentLayerId(layerNames), value);

            OnUpdated(dataKey);
        }
        #endregion

        #region Methods
        /// <summary>
        ///     Noes the data found exception.
        /// </summary>
        static DataFlowException NoDataFoundException<T>(DataKey<T> dataKey)
        {
            return new DataFlowException($"No data found in context. Data key is '{dataKey}'");
        }

        /// <summary>
        ///     Checks the key is not forwarded.
        /// </summary>
        void CheckKeyIsNotForwarded<T>(DataKey<T> dataKey)
        {
            if (forwardMap.ContainsKey(dataKey.Id))
            {
                throw new DataFlowException($"Forwarded key can not be modify. Forwarded data key is '{dataKey}'");
            }
        }

        /// <summary>
        ///     Gets the identifier.
        /// </summary>
        string GetId<T>(DataKey<T> dataKey)
        {
            return GetId(dataKey.Id);
        }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        string GetId(string id)
        {
            string targetId = null;

            if (forwardMap.TryGetValue(id, out targetId))
            {
                id = targetId;
            }

            return id;
        }
        #endregion

        #region On Insert
        /// <summary>
        ///     The event bus
        /// </summary>
        readonly EventBus eventBus = new EventBus();
        
        /// <summary>
        ///     Gets the name of the insert event.
        /// </summary>
        string GetInsertEventName<T>(DataKey<T> dataKey)
        {
            return "Insert->" + dataKey.Id;
        }

        /// <summary>
        ///     Gets the name of the update event.
        /// </summary>
        string GetUpdateEventName<T>(DataKey<T> dataKey)
        {
            return "Update->" + dataKey.Id;
        }

        /// <summary>
        ///     Gets the name of the remove event.
        /// </summary>
        string GetRemoveEventName<T>(DataKey<T> dataKey)
        {
            return "Remove->" + dataKey.Id;
        }

        /// <summary>
        ///     Called when [insert].
        /// </summary>
        public void OnInsert<T>(DataKey<T> dataKey, Action action)
        {
            eventBus.Subscribe(GetInsertEventName(dataKey), action);
        }

        /// <summary>
        ///     Called when [update].
        /// </summary>
        public void OnUpdate<T>(DataKey<T> dataKey, Action action)
        {
            eventBus.Subscribe(GetUpdateEventName(dataKey), action);
        }

        /// <summary>
        ///     Called when [remove].
        /// </summary>
        public void OnRemove<T>(DataKey<T> dataKey, Action action)
        {
            eventBus.Subscribe(GetRemoveEventName(dataKey), action);
        }

        /// <summary>
        ///     Called when [inserted].
        /// </summary>
        void OnInserted<T>(DataKey<T> dataKey)
        {
            var eventName = GetInsertEventName(dataKey);

            eventBus.Publish(eventName);
        }

        /// <summary>
        ///     Called when [updated].
        /// </summary>
        void OnUpdated<T>(DataKey<T> dataKey)
        {
            var eventName = GetUpdateEventName(dataKey);

            eventBus.Publish(eventName);
        }

        /// <summary>
        ///     Called when [removed].
        /// </summary>
        void OnRemoved<T>(DataKey<T> dataKey)
        {
            var eventName = GetRemoveEventName(dataKey);

            eventBus.Publish(eventName);
        }
        #endregion

        #region IEnumerable
        /// <summary>
        ///     Returns an enumerator that iterates through the collection.
        /// </summary>
        public IEnumerator<DataContextEntry> GetEnumerator()
        {
            return ((IEnumerable<DataContextEntry>) dictionary.Values).GetEnumerator();
        }

        /// <summary>
        ///     Returns an enumerator that iterates through a collection.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        #region Event Bus
        /// <summary>
        ///     Publishes the event.
        /// </summary>
        public void PublishEvent(string eventName)
        {
            eventBus.Publish(eventName);
        }

        /// <summary>
        ///     Publishes the event.
        /// </summary>
        public void PublishEvent(Enum eventName)
        {
            eventBus.Publish(eventName.ToString());
        }

        /// <summary>
        ///     Subscribes the event.
        /// </summary>
        public void SubscribeEvent(string eventName, Action action)
        {
            eventBus.Subscribe(eventName, action);
        }

        /// <summary>
        ///     Subscribes the event.
        /// </summary>
        public void SubscribeEvent(Enum eventName, Action action)
        {
            eventBus.Subscribe(eventName.ToString(), action);
        }

        /// <summary>
        ///     Uns the subscribe event.
        /// </summary>
        public void UnSubscribeEvent(string eventName, Action action)
        {
            eventBus.UnSubscribe(eventName, action);
        }

        /// <summary>
        ///     Uns the subscribe event.
        /// </summary>
        public void UnSubscribeEvent(Enum eventName, Action action)
        {
            eventBus.UnSubscribe(eventName.ToString(), action);
        }
        #endregion
        #region ICollection

        void ICollection<DataContextEntry>.Add(DataContextEntry entry)
        {
            if (entry == null)
            {
                throw new ArgumentNullException(nameof(entry));
            }

            if (entry.Layer != LayerHelper.GetCurrentLayerId(layerNames))
            {
                OpenNewLayer(LayerHelper.GetLayerName(entry.Layer));
            }

            dictionary[entry.Key] = entry;
        }

        void ICollection<DataContextEntry>.Clear()
        {
            dictionary.Clear();
        }

        bool ICollection<DataContextEntry>.Contains(DataContextEntry entry)
        {
            if (entry == null)
            {
                throw new ArgumentNullException(nameof(entry));
            }

            return dictionary.ContainsKey(entry.Key);
        }

        void ICollection<DataContextEntry>.CopyTo(DataContextEntry[] array, int arrayIndex)
        {
        }

        bool ICollection<DataContextEntry>.Remove(DataContextEntry entry)
        {
            return false;
        }

        int ICollection<DataContextEntry>.Count => dictionary.Count;

        bool ICollection<DataContextEntry>.IsReadOnly => false; 
        #endregion
    }
}