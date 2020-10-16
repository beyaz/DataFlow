using System.Collections.Generic;

namespace BOA.DataFlow
{
    /// <summary>
    ///     The layer helper
    /// </summary>
    static class LayerHelper
    {
        #region Public Methods
        /// <summary>
        ///     Gets the current layer identifier.
        /// </summary>
        public static string GetCurrentLayerId(IList<string> layerNames)
        {
            if (layerNames.Count == 0)
            {
                return "0:Root";
            }

            return layerNames.Count + ":" + layerNames[layerNames.Count - 1];
        }

        /// <summary>
        ///     Gets the current layer identifier.
        /// </summary>
        public static string GetCurrentLayerId(string layerName, int index)
        {
            return index + ":" + layerName;
        }
        #endregion
    }
}