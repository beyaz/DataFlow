using System;
using System.Diagnostics;

namespace BOA.DataFlow
{
    /// <summary>
    ///     The data context entry
    /// </summary>
    [DebuggerDisplay("{shortName} : {Value}")]
    [Serializable]
    public class DataContextEntry
    {
        #region Fields
        /// <summary>
        ///     The short name
        /// </summary>
        readonly string shortName;
        #endregion

        #region Constructors
        /// <summary>
        ///     Initializes a new instance of the <see cref="DataContextEntry" /> class.
        /// </summary>
        public DataContextEntry(string key, int layerIndex, object value)
        {
            Key        = key;
            LayerIndex = layerIndex;
            Value      = value;

            shortName = ShortNameHelper.GetShortName(key);
        }
        #endregion

        #region Public Properties
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
            return $"Layer: {LayerIndex} - Key: {shortName} : {Value}";
        }
        #endregion
    }
}