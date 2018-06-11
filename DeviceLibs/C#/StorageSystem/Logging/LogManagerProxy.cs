using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rowa.Lib.Log;

namespace CareFusion.Lib.StorageSystem.Logging
{
    /// <summary>
    /// Class which counts how many instances uses LogManager and keep it active as long as it is needed.
    /// </summary>
    static class LogManagerProxy
    {
        /// <summary>
        /// Usage counter
        /// </summary>
        private static int useCount = 0;

        /// <summary>
        /// Thread synchronization object for wwi log file stream access.
        /// </summary>
        private static object _syncLock = new object();

        /// <summary>
        /// Initialize LogManager, if needed.
        /// </summary>
        public static void Initialize()
        {
            lock (_syncLock)
            {
                if (useCount == 0)
                {
                    LogManager.Initialize("Storage System Lib", "Application", System.Environment.UserInteractive);
                }

                useCount++;
            }
        }

        /// <summary>
        /// Cleanup LogManager, if needed.
        /// </summary>
        public static void Cleanup()
        {
            lock (_syncLock)
            {
                useCount--;

                if (useCount == 0)
                {
                    LogManager.Cleanup();
                }
            }
        }
    }
}
