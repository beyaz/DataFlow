﻿namespace BOA.DataFlow
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

        #region Methods
        /// <summary>
        ///     Gets the specified data key.
        /// </summary>
        protected T Get<T>(DataKey<T> dataKey)
        {
            return Context.Get(dataKey);
        }
        #endregion
    }
}