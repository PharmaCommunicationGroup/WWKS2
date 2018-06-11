using Rowa.Lib.Log;

namespace CareFusion.Lib.StorageSystem.Logging
{
    public class WwiLogIntercept : IWwi
    {
        static WwiLogIntercept singleton = null;
        private IWwi baseWwiLog;

        private WwiLogIntercept()
        {

        }

        static public WwiLogIntercept GetSingleton()
        {
            if (singleton == null)
            {
                singleton = new WwiLogIntercept();
            }

            return singleton;
        }
        static public void CreateBaseWwiLogger(string description, string remoteAddress, ushort port)
        {
            GetSingleton().baseWwiLog = LogManager.GetWwi(description, remoteAddress, port);
        }

        public void Dispose()
        {
            if (this.baseWwiLog != null)
            {
                this.baseWwiLog.Dispose();
            }
        }

        public void LogMessage(byte[] message, bool isIncommingMessage = true)
        {
            if (this.baseWwiLog != null)
            {
                this.baseWwiLog.LogMessage(message, isIncommingMessage);
            }
        }

        public void LogMessage(string message, bool isIncommingMessage = true)
        {
            if (this.baseWwiLog != null)
            {
                this.baseWwiLog.LogMessage(message, isIncommingMessage);
            }
        }

        public void LogMessage(byte[] message, int index, int length, bool isIncommingMessage = true)
        {
            if (this.baseWwiLog != null)
            {
                this.baseWwiLog.LogMessage(message, index, length, isIncommingMessage);
            }
        }
    }
}
