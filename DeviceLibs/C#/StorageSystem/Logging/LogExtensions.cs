using System;
using Rowa.Lib.Log;

namespace CareFusion.Lib.StorageSystem.Logging
{
    /// <summary>
    /// Class which extends any managed type with handy logging methods to ease the usage of the assembly log.
    /// </summary>
    internal static class LogExtensions
    {
        /// <summary>
        /// Writes a trace log entry.
        /// </summary>
        /// <param name="logInstance">The log object instance.</param>
        /// <param name="format">The format string of the log entry.</param>
        /// <param name="args">The format arguments of the log entry.</param>
        public static void Trace(this object logInstance, string format, params object[] args)
        {
            LogManager.GetLogger(logInstance.GetType().Name).Debug(format, args);
        }

        /// <summary>
        /// Writes an informational log entry.
        /// </summary>
        /// <param name="logInstance">The log object instance.</param>
        /// <param name="format">The format string of the log entry.</param>
        /// <param name="args">The format arguments of the log entry.</param>
        public static void Info(this object logInstance, string format, params object[] args)
        {
            LogManager.GetLogger(logInstance.GetType().Name).Info(format, args);
        }

        /// <summary>
        /// Writes a warning log entry.
        /// </summary>
        /// <param name="logInstance">The log object instance.</param>
        /// <param name="format">The format string of the log entry.</param>
        /// <param name="args">The format arguments of the log entry.</param>
        public static void Warning(this object logInstance, string format, params object[] args)
        {
            LogManager.GetLogger(logInstance.GetType().Name).Warning(format, args);
        }

        /// <summary>
        /// Writes an error log entry.
        /// </summary>
        /// <param name="logInstance">The log object instance.</param>
        /// <param name="format">The format string of the log entry.</param>
        /// <param name="args">The format arguments of the log entry.</param>
        public static void Error(this object logInstance, string format, params object[] args)
        {
            LogManager.GetLogger(logInstance.GetType().Name).Error(format, args);
        }

        /// <summary>
        /// Writes an error log entry with exception details.
        /// </summary>
        /// <param name="logInstance">The log object instance.</param>
        /// <param name="format">The format string of the log entry.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="args">The format arguments of the log entry.</param>
        public static void Error(this object logInstance, string format, Exception exception, params object[] args)
        {
            var errorMessage = string.Format(format, args);
            LogManager.GetLogger(logInstance.GetType().Name).Error(errorMessage, exception);
        }

        /// <summary>
        /// Writes an fatal error log entry.
        /// </summary>
        /// <param name="logInstance">The log object instance.</param>
        /// <param name="format">The format string of the log entry.</param>
        /// <param name="args">The format arguments of the log entry.</param>
        public static void Fatal(this object logInstance, string format, params object[] args)
        {
            LogManager.GetLogger(logInstance.GetType().Name).Fatal(format, args);
        }
    }
}
