using System;

namespace BOA.DataFlow
{
    /// <summary>
    ///     The short name helper
    /// </summary>
    static class ShortNameHelper
    {
        #region Public Methods
        /// <summary>
        ///     Gets the short name.
        /// </summary>
        public static string GetShortName(string id)
        {
            if (!id.Contains(":"))
            {
                return id;
            }

            var lastIndexOf = id.LastIndexOf(":", StringComparison.Ordinal);

            return id.Substring(lastIndexOf + 1, id.Length - lastIndexOf - 1);
        }
        #endregion
    }
}