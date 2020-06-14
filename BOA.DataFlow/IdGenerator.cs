namespace BOA.DataFlow
{
    /// <summary>
    ///     The identifier generator
    /// </summary>
    static class IdGenerator
    {
        #region Static Fields
        /// <summary>
        ///     The lock item
        /// </summary>
        static readonly object lockItem = new object();

        /// <summary>
        ///     The identifier
        /// </summary>
        static int Id;
        #endregion

        #region Public Methods
        /// <summary>
        ///     Gets the next identifier.
        /// </summary>
        public static int GetNextId()
        {
            lock (lockItem)
            {
                return Id++;
            }
        }
        #endregion
    }
}