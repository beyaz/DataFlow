using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BOA.DataFlow
{
    /// <summary>
    ///     The data context debug view
    /// </summary>
    sealed class DataContextDebugView
    {
        #region Fields
        /// <summary>
        ///     The context
        /// </summary>
        readonly DataContext data;
        #endregion

        #region Constructors
        /// <summary>
        ///     Initializes a new instance of the <see cref="DataContextDebugView" /> class.
        /// </summary>
        public DataContextDebugView(DataContext data)
        {
            this.data = data;
        }
        #endregion

        #region Public Properties
        /// <summary>
        ///     Gets the items.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public LayerDebugView[] Items
        {
            get
            {
                var items = new List<LayerDebugView>();

                for (var i = 0; i < data.layerNames.Count; i++ )
                {
                    var layerName = data.layerNames[i];

                    var entries = data.dictionary.Values.Where(x => x.Layer == LayerHelper.GetCurrentLayerId(layerName,i+1)).ToArray();

                    items.Add(new LayerDebugView {LayerName = layerName, Items = entries});
                }

                return items.ToArray();
            }
        }
        #endregion
    }
}