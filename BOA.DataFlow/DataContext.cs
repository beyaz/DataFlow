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
    public class DataContext : IEnumerable<DataContextEntry>
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
        ///     The current layer index
        /// </summary>
        internal int currentLayerIndex;

        /// <summary>
        ///     The forward map
        /// </summary>
        readonly Dictionary<string, string> forwardMap = new Dictionary<string, string>();
        #endregion

        #region Public Properties
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

            dictionary[dataKey.Id] = new DataContextEntry(dataKey.Id, currentLayerIndex, value, dataKey.Name);

            OnInserted(dataKey);
        }

        /// <summary>
        ///     Closes the current layer.
        /// </summary>
        public void CloseCurrentLayer()
        {
            if (currentLayerIndex < 0)
            {
                throw new DataFlowException("There is no layer to close.");
            }

            var removeList = dictionary.Values.Where(p => p.LayerIndex == currentLayerIndex).Select(p => p.Key).ToList();

            foreach (var key in removeList)
            {
                dictionary.Remove(key);
            }

            currentLayerIndex--;
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

            throw NoDataFoundException(dataKey);
        }

        /// <summary>
        ///     Opens the new layer.
        /// </summary>
        public void OpenNewLayer(string layerName)
        {
            currentLayerIndex++;
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
                if (dataContextEntry.LayerIndex != currentLayerIndex)
                {
                    throw new DataFlowException($"Other layer variables can not be remove. CurrentLayerIndex: {currentLayerIndex}, TargetLayerIndex: {dataContextEntry.LayerIndex}");
                }

                dictionary.Remove(dataKey.Id);
                return;
            }

            throw NoDataFoundException(dataKey);
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
        ///     Updates the specified data key.
        /// </summary>
        public void Update<T>(DataKey<T> dataKey, T value)
        {
            CheckKeyIsNotForwarded(dataKey);

            if (dictionary.ContainsKey(dataKey.Id))
            {
                Remove(dataKey);
            }

            dictionary[dataKey.Id] = new DataContextEntry(dataKey.Id, currentLayerIndex, value, dataKey.Name);
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
            var id = dataKey.Id;

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
        ///     Called when [insert].
        /// </summary>
        public void OnInsert<T>(DataKey<T> dataKey, Action action)
        {
            eventBus.Subscribe(GetInsertEventName(dataKey), action);
        }

        /// <summary>
        ///     Called when [inserted].
        /// </summary>
        void OnInserted<T>(DataKey<T> dataKey)
        {
            var eventName = GetInsertEventName(dataKey);

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
    }
}