using System;
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

        /// <summary>
        ///     Gets the name of the layer.
        /// </summary>
        public static string GetLayerName(string layerId)
        {
            var semiColumnIndex = layerId.IndexOf(":", StringComparison.OrdinalIgnoreCase);
            if (semiColumnIndex < 0)
            {
                return layerId;
            }

            return layerId.Substring(semiColumnIndex + 1);
        }
        #endregion
    }
}