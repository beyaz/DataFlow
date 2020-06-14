using System.Diagnostics;

namespace BOA.DataFlow
{
    /// <summary>
    ///     The layer debug view
    /// </summary>
    [DebuggerDisplay("{LayerName} : {Items}")]
    sealed class LayerDebugView
    {
        #region Public Properties
        /// <summary>
        ///     Gets or sets the items.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public DataContextEntry[] Items { get; set; }

        /// <summary>
        ///     Gets or sets the name of the layer.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public string LayerName { get; set; }
        #endregion
    }
}