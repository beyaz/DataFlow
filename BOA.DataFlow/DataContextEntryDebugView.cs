using System.Diagnostics;

namespace BOA.DataFlow
{
    /// <summary>
    ///     The data context entry debug view
    /// </summary>
    [DebuggerDisplay("{data.DataKeyName} : {Value}")]
    class DataContextEntryDebugView
    {
        #region Fields
        /// <summary>
        ///     The data
        /// </summary>
        readonly DataContextEntry data;
        #endregion

        #region Constructors
        /// <summary>
        ///     Initializes a new instance of the <see cref="DataContextEntryDebugView" /> class.
        /// </summary>
        public DataContextEntryDebugView(DataContextEntry data)
        {
            this.data = data;
        }
        #endregion

        #region Public Properties
        /// <summary>
        ///     Gets the value.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public object Value
        {
            get { return data.Value; }
        }
        #endregion
    }
}