using System;
using System.Diagnostics;
using System.Threading;
using Cr7Sund.CompilerServices;
using Cr7Sund.FrameWork.Util;

namespace Cr7Sund
{
    public sealed class PromiseTaskSource : IPromiseTaskSource, IPoolNode<PromiseTaskSource>
    {
        private static ReusablePool<PromiseTaskSource> _pool;

        private PromiseTaskSource _nextNode;
        private PromiseTaskCompletionSourceCore _core;
        private short _version;

        public ref PromiseTaskSource NextNode => ref _nextNode;
        public bool IsRecycled { get; set; }
        public PromiseTask Task
        {
            [DebuggerHidden]
            get
            {
                ValidateToken();
                return new PromiseTask(this, _core.Version);
            }
        }

        private PromiseTaskSource()
        {

        }

        public static PromiseTaskSource Create()
        {
            if (!_pool.TryPop(out PromiseTaskSource result))
            {
                result = new PromiseTaskSource();
            }
            result._version = result._core.Version;
            // TaskTracker.TrackActiveTask(result, 2);
            return result;
        }

        [DebuggerHidden]
        public void GetResult(short token)
        {
            try
            {
                _core.GetResult(token);
            }
            finally
            {
                TryReturn();
            }
        }

        [DebuggerHidden]
        public PromiseTaskStatus GetStatus(short token)
        {
            return _core.GetStatus(token);
        }

        [DebuggerHidden]
        public PromiseTaskStatus UnsafeGetStatus()
        {
            return _core.UnsafeGetStatus();
        }

        public void OnCompleted(Action continuation, short token)
        {
            _core.OnCompleted(continuation, token);
        }

        public bool TryCancel(string cancelMsg, UnsafeCancellationToken cancellation)
        {
            return _version == _core.Version && _core.TrySetCanceled(cancelMsg, cancellation);
        }

        public bool TryResolve()
        {
            return _version == _core.Version && _core.TrySetResult();
        }

        public bool TryReject(Exception exception)
        {
            return _version == _core.Version && _core.TrySetException(exception);
        }

        #region UnityTest
        public static int Test_GetPoolCount()
        {
            return _pool.Size;
        }
        #endregion

        private bool TryReturn()
        {
            //TaskTracker.RemoveTracking(this);

            ValidateToken();
            _core.Reset();
            return _pool.TryPush(this);
        }

        private void ValidateToken()
        {
            AssertUtil.AreEqual(this._version, this._core.Version, PromiseTaskExceptionType.CAN_VISIT_VALID_VERSION);
        }
    }
}
