using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Blake.NUI.WPF.Utility
{
    public class WorkQueue<TWorkRequest> : IDisposable
    {
        #region Fields

        bool _shouldThreadRun = false;

        Thread _workerThread;

        object _queueLock = new object();
        Queue<TWorkRequest> _queue = new Queue<TWorkRequest>();

        ManualResetEvent _workAvailable;
        ManualResetEvent _queueClear;

        bool _isDisposed = false;

        Action _onThreadStartup = null;
        Action _onThreadShutdown = null;

        #endregion

        #region Properties

        private Action<TWorkRequest> _callback = null;
        public Action<TWorkRequest> Callback
        {
            get
            {
                return _callback;
            }
            set
            {
                if (_callback == value)
                    return;
                if (value == null)
                    throw new ArgumentNullException("value");
                _callback = value;
            }
        }

        private Action<TWorkRequest> _canceledCallback = null;
        public Action<TWorkRequest> CanceledCallback
        {
            get
            {
                return _canceledCallback;
            }
            set
            {
                if (_canceledCallback == value)
                    return;
                _canceledCallback = value;
            }
        }

        private int _maxQueueLength = 1;
        public int MaxQueueLength
        {
            get
            {
                return _maxQueueLength;
            }
            set
            {
                _maxQueueLength = Math.Max(1, value);
            }
        }

        public int QueueLength
        {
            get
            {
                return _queue.Count;
            }
        }

        public bool IsBusy { get; private set; }

        //public ManualResetEvent QueueClear { get { return queueClear; } }

        #endregion

        #region Constructors

        public WorkQueue(Action<TWorkRequest> callback)
            : this(callback, null, null)
        { }

        public WorkQueue(Action<TWorkRequest> callback, Action onThreadStartup, Action onThreadShutdown)
        {
            this.Callback = callback;
            _onThreadStartup = onThreadStartup;
            _onThreadShutdown = onThreadShutdown;
            _workAvailable = new ManualResetEvent(false);
            _queueClear = new ManualResetEvent(true);
            IsBusy = false;
            StartThread();
        }

        #endregion

        #region Public Methods

        public void AddWork(TWorkRequest data)
        {
            if (_isDisposed)
                return;
            lock (_queueLock)
            {
                _queue.Enqueue(data);

                while (_queue.Count > MaxQueueLength)
                {
                    var canceledWork = _queue.Dequeue();
                    if (_canceledCallback != null)
                    {
                        _canceledCallback(canceledWork);
                    }
                }
            }

            if (_workerThread == null)
            {
                StartThread();
            }
            _workAvailable.Set();
            _queueClear.Reset();
        }

        public void ClearQueue()
        {
            lock (_queueLock)
            {
                while (_queue.Count > 0)
                {
                    var canceledWork = _queue.Dequeue();
                    if (_canceledCallback != null)
                    {
                        _canceledCallback(canceledWork);
                    }
                }
            }
            _queueClear.Set();
        }

        public void Drain()
        {
            _queueClear.WaitOne(-1);
        }

        #endregion

        #region Private Methods

        private void StartThread()
        {
            ShutdownThread();
            IsBusy = false;

            ThreadStart ts = new ThreadStart(RunWorkerThread);
            _workerThread = new Thread(ts);
            _workerThread.Name = "WorkQueue thread " + Guid.NewGuid().ToString();
            _shouldThreadRun = true;
            _workerThread.Start();
        }

        private void ShutdownThread()
        {
            _shouldThreadRun = false;

            lock (_queueLock)
            {
                _queue.Clear();
            }

            _workAvailable.Set();
            _queueClear.Reset();
            if (_workerThread == null || !_workerThread.IsAlive)
                return;
            _workerThread.Join(200);

            if (_workerThread.IsAlive)
            {
                _workerThread.Abort();
            }
            _workerThread = null;
            _queueClear.Set();
        }

        private void RunWorkerThread()
        {
            if (_onThreadStartup != null)
            {
                _onThreadStartup();
            }

            while (_shouldThreadRun)
            {
                TWorkRequest data = default(TWorkRequest);
                bool dataRetrieved = false;
                lock (_queueLock)
                {
                    if (_queue.Count > 0)
                    {
                        data = _queue.Dequeue();
                        dataRetrieved = true;
                        IsBusy = true;
                    }
                }
                if (dataRetrieved)
                {
                    _callback(data);
                }
                else
                {
                    //No work available
                    _workAvailable.Reset();
                    _queueClear.Set();
                    IsBusy = false;
                    if (_shouldThreadRun)
                    {
                        _workAvailable.WaitOne(-1);
                    }
                }
            }

            if (_onThreadShutdown != null)
            {
                _onThreadShutdown();
            }
        }

        #endregion

        #region Cleanup Methods

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            ShutdownThread();
        }

        ~WorkQueue()
        {
            Dispose(false);
        }

        #endregion
    }
}
