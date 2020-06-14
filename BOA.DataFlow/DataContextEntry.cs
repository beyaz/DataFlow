using System;
using System.Diagnostics;

namespace BOA.DataFlow
{
    /// <summary>
    ///     The data context entry
    /// </summary>
    [DebuggerDisplay("{DataKeyName} = {Value}")]
    [DebuggerTypeProxy(typeof(DataContextEntryDebugView))]
    [Serializable]
    public class DataContextEntry
    {
        #region Constructors
        /// <summary>
        ///     Initializes a new instance of the <see cref="DataContextEntry" /> class.
        /// </summary>
        public DataContextEntry(string key, int layerIndex, object value, string dataKeyName)
        {
            Key         = key;
            LayerIndex  = layerIndex;
            Value       = value;
            DataKeyName = dataKeyName;
        }
        #endregion

        #region Public Properties
        /// <summary>
        ///     The data key name
        /// </summary>
        public string DataKeyName { get; }

        /// <summary>
        ///     The key
        /// </summary>
        public string Key { get; }

        /// <summary>
        ///     The layer index
        /// </summary>
        public int LayerIndex { get; }

        /// <summary>
        ///     The value
        /// </summary>
        public object Value { get; }
        #endregion

        #region Public Methods
        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        public override string ToString()
        {
            return $"Layer: {LayerIndex} - Key: {DataKeyName} : {Value}";
        }
        #endregion
    }
}