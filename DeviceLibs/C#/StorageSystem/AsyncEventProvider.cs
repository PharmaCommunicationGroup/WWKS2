using CareFusion.Lib.StorageSystem.Logging;
using System;
using System.Collections.Generic;
using System.Threading;

namespace CareFusion.Lib.StorageSystem
{
    /// <summary>
    /// Base class for components which provide events that are raised asynchronously.
    /// </summary>
    public class AsyncEventProvider : IDisposable
    {
        #region Types

        /// <summary>
        /// Small helper class which is used to queue pending events.
        /// </summary>
        private class AsyncEvent
        {
            /// <summary>
            /// Name of the event to raise.
            /// </summary>
            public string EventName { get; set; }

            /// <summary>
            /// Gets or sets the delegate to call asynchronously.
            /// </summary>
            public Delegate EventMethod { get; set; }

            /// <summary>
            /// Gets or sets the parameters of the delegate to call asynchronously.
            /// </summary>
            public object[] Parameters { get; set; }
        }

        #endregion

        #region Members

        /// <summary>
        /// Queue which holds the pending events that should be raised asynchronously.
        /// </summary>
        private Queue<AsyncEvent> _eventQueue = new Queue<AsyncEvent>();

        /// <summary>
        /// The trigger which is notified when new events are in the queue.
        /// </summary>
        private AutoResetEvent _newEvent = new AutoResetEvent(false);

        /// <summary>
        /// The trigger which is notified when the event processor should shut down.
        /// </summary>
        private ManualResetEvent _shutdown = new ManualResetEvent(false);

        /// <summary>
        /// The thread which is raising the queued events.
        /// </summary>
        private Thread _eventThread = null;

        /// <summary>
        /// Flag whether this object has been disposed.
        /// </summary>
        protected bool _isDisposed = false;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncEventProvider"/> class.
        /// </summary>
        protected AsyncEventProvider()
        {
            _eventThread = new Thread(new ThreadStart(RunAsyncEventProvider));
            _eventThread.Start();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="AsyncEventProvider"/> class.
        /// </summary>
        ~AsyncEventProvider()
        {
            Dispose(false);
        }

        /// <summary>
        /// Raises the specified event in an asynchronous way.
        /// </summary>
        /// <param name="eventName">The name of the event that has to be raised.</param>
        /// <param name="eventMethod">The event to raise.</param>
        /// <param name="eventParameters">The event parameters to use.</param>
        protected void Raise(string eventName, Delegate eventMethod, params object[] eventParameters)
        {
            if (eventMethod == null)
            {
                this.Error("Raising event '{0}' failed because no event handler has been registered.", eventName);
                return;
            }                

            if (_isDisposed)
            {
                return;
            }                

            lock (_eventQueue)
            {
                _eventQueue.Enqueue(new AsyncEvent() { EventName = eventName, EventMethod = eventMethod, Parameters = eventParameters });
            }

            _newEvent.Set();
        }

        /// <summary>
        /// Resets the event queue and discards any currently pending events.
        /// </summary>
        protected void ResetEventQueue()
        {
            lock (_eventQueue)
            {
                _eventQueue.Clear();
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="isDisposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool isDisposing)
        {
            if (_isDisposed)
                return;

            if (isDisposing)
            {
                _shutdown.Set();

                //if (_eventThread != null)
                //    _eventThread.Join();

                //_eventThread = null;
                //_eventQueue.Clear();
                //_newEvent.Dispose();
                //_shutdown.Dispose();
            }

            _isDisposed = true;
        }

        /// <summary>
        /// Executes the asynchronous event processor.
        /// </summary>
        private void RunAsyncEventProvider()
        {
            var waitHandles = new WaitHandle[] { _newEvent, _shutdown };

            while (waitHandles[WaitHandle.WaitAny(waitHandles)] != _shutdown)
            {
                AsyncEvent asyncEvent = null;

                do
                {
                    lock (_eventQueue)
                    {
                        asyncEvent = _eventQueue.Count > 0 ? _eventQueue.Dequeue() : null;
                    }

                    if (asyncEvent != null)
                    {
                        try
                        {
                            this.Trace("Raising event '{0}' by invoking '{1}'.", asyncEvent.EventName, asyncEvent.EventMethod.Method.Name);
                            asyncEvent.EventMethod.DynamicInvoke(asyncEvent.Parameters);
                        }
                        catch (Exception ex)
                        {
                            this.Error("Raising event '{0}' by invoking '{1}' failed!", ex, asyncEvent.EventName, asyncEvent.EventMethod.Method.Name);
                        }
                    }
                }
                while (asyncEvent != null);
            }

            _eventThread = null;
            _eventQueue.Clear();
            _newEvent.Dispose();
            _shutdown.Dispose();
        }
    }
}
