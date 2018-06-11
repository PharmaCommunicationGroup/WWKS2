using CareFusion.Lib.StorageSystem.Logging;
using System;
using System.Net.Sockets;

namespace CareFusion.Lib.StorageSystem.Net
{
    /// <summary>
    /// Class which extends the TcpClient class with some useful functionality.
    /// </summary>
    internal static class TcpClientExtensions
    {
        #region Constants

        /// <summary>
        /// Optimized size of the tcp read buffer size.
        /// </summary>
        private const int ReadBufferSize = 4194304;

        #endregion

        /// <summary>
        /// Increases the read buffer size of the specified tcp client socket instance to
        /// speed up the read operations of large data chunks.
        /// </summary>
        public static void IncreaseReadBufferSize(this TcpClient instance)
        {
            if (instance == null)
            {
                throw new ArgumentException("Invalid TcpClient instance specified.");
            }

            instance.ReceiveBufferSize = ReadBufferSize;
        }

        /// <summary>
        /// Enables the socket keep alive feature at the current TcpClient connection.
        /// This incredibly improves the "connection broken" detection of the TcpClient via its Connected property.
        /// <see cref="http://social.msdn.microsoft.com/Forums/en-US/netfxnetcom/thread/d5b6ae25-eac8-4e3d-9782-53059de04628"/>
        /// </summary>
        public static void EnableSocketKeepAlive(this TcpClient instance)
        {
            if (instance == null)
            {
                throw new ArgumentException("Invalid TcpClient instance specified.");
            }

            instance.Trace("Enable socket keep alive...");

            const int bytesperlong = 4;
            const int bitsperbyte = 8;
            const ulong time = 2000;        // keep alive time in milliseconds
            const ulong interval = 500;     // keep alive interval in milliseconds

            try
            {
                // resulting structure
                byte[] SIO_KEEPALIVE_VALS = new byte[3 * bytesperlong];

                // array to hold input values
                ulong[] input = new ulong[3];
                input[0] = (1UL);    // on
                input[1] = time;     // time millis
                input[2] = interval; // interval millis

                // pack input into byte struct
                for (int i = 0; i < input.Length; i++)
                {
                    SIO_KEEPALIVE_VALS[i * bytesperlong + 3] = (byte)(input[i] >> ((bytesperlong - 1) * bitsperbyte) & 0xff);
                    SIO_KEEPALIVE_VALS[i * bytesperlong + 2] = (byte)(input[i] >> ((bytesperlong - 2) * bitsperbyte) & 0xff);
                    SIO_KEEPALIVE_VALS[i * bytesperlong + 1] = (byte)(input[i] >> ((bytesperlong - 3) * bitsperbyte) & 0xff);
                    SIO_KEEPALIVE_VALS[i * bytesperlong + 0] = (byte)(input[i] >> ((bytesperlong - 4) * bitsperbyte) & 0xff);
                }

                // create bytestruct for result (bytes pending on server socket)
                byte[] result = BitConverter.GetBytes(0);

                // write SIO_VALS to Socket IOControl
                instance.Client.IOControl(IOControlCode.KeepAliveValues, SIO_KEEPALIVE_VALS, result);
            }
            catch (Exception ex)
            {
                instance.Error("Enable socket keep alive failed!", ex);
            }
        }
    }
}
