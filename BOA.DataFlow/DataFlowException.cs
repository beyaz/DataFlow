using System;

namespace BOA.DataFlow
{
    /// <summary>
    ///     The data flow exception
    /// </summary>
    [Serializable]
    public class DataFlowException : InvalidOperationException
    {
        #region Constructors
        /// <summary>
        ///     Initializes a new instance of the <see cref="DataFlowException" /> class.
        /// </summary>
        public DataFlowException(string message) : base(message)
        {
        }
        #endregion
    }
}