namespace BOA.DataFlow
{
    /// <summary>
    ///     The service base
    /// </summary>
    public class ServiceBase
    {
        #region Public Properties
        /// <summary>
        ///     Gets or sets the context.
        /// </summary>
        public DataContext Context { get; set; }
        #endregion

        #region Public Methods
        /// <summary>
        ///     Gets the specified data key.
        /// </summary>
        public T Get<T>(DataKey<T> dataKey)
        {
            return Context.Get(dataKey);
        }
        #endregion
    }
}