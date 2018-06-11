using System.Threading;

namespace CareFusion.Lib.StorageSystem.Wwks2.Messages
{
    /// <summary>
    /// Class which is used to generate WWKS 2.0 message identifiers in a thread-safe way.
    /// </summary>
    public static class MessageId
    {
        #region Members

        /// <summary>
        /// Holds the last generated message identifier.
        /// </summary>
        private static int _lastId = 0;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a new message identifier.
        /// </summary>
        public static string Next
        {
            get 
            {
                Interlocked.CompareExchange(ref _lastId, 0, 9999999);
                return Interlocked.Increment(ref _lastId).ToString();
            }
        }

        #endregion
    }
}
