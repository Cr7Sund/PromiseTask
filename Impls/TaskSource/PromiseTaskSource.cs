using System;
using System.Diagnostics;
using Cr7Sund.CompilerServices;

namespace Cr7Sund
{
    public sealed class PromiseTaskSource<T> : IPromiseTaskSource<T>, ITaskPoolNode<PromiseTaskSource<T>>
    {
        private static TaskPool<PromiseTaskSource<T>> _pool;

        public PromiseTaskSource<T> _nextNode;
        private PromiseTaskCompletionSourceCore<T> core;
        private short version;

        public ref PromiseTaskSource<T> NextNode => ref _nextNode;
        public PromiseTask<T> Task
        {
            get
            {
                return new PromiseTask<T>(this, version);
            }
        }


        public PromiseTaskSource()
        {

        }

        public static PromiseTaskSource<T> Create()
        {
            if (!_pool.TryPop(out PromiseTaskSource<T> result))
            {
                result = new PromiseTaskSource<T>();
            }
            result.version = result.core.Version;
            // TaskTracker.TrackActiveTask(result, 2);
            return result;
        }

        public static PromiseTaskSource<T> Create(T value)
        {
            if (!_pool.TryPop(out PromiseTaskSource<T> result))
            {
                result = new PromiseTaskSource<T>();
            }

            // TaskTracker.TrackActiveTask(result, 2);
            result.core.TrySetResult(value);
            return result;
        }

        [DebuggerHidden]
        public bool TryResolve(T result)
        {
            return version == core.Version && core.TrySetResult(result);
        }

        [DebuggerHidden]
        public bool TryCancel(string cancelMsg, UnsafeCancellationToken cancellationToken = default)
        {
            return version == core.Version && core.TrySetCanceled(cancelMsg, cancellationToken);
        }

        [DebuggerHidden]
        public bool TryReject(Exception exception)
        {
            return version == core.Version && core.TrySetException(exception);
        }

        public T GetResult(short token)
        {
            try
            {
                return core.GetResult(token);
            }
            finally
            {
                TryReturn();
            }
        }

        public PromiseTaskStatus GetStatus(short token)
        {
            return core.GetStatus(token);
        }

        public PromiseTaskStatus UnsafeGetStatus()
        {
            return core.UnsafeGetStatus();
        }

        void IPromiseTaskSource.GetResult(short token)
        {
            GetResult(token);
        }

        public void OnCompleted(Action continuation, short token)
        {
            core.OnCompleted(continuation, token);
        }

        #region UnityTest
        public static int Test_GetPoolCount()
        {
            return _pool.Size;
        }
        #endregion
        private void TryReturn()
        {
            // TaskTracker.RemoveTracking(this);
            core.Reset();
            _pool.TryPush(this);
        }
    }
}
