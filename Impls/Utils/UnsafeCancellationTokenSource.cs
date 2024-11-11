using System;
using System.Collections.Generic;
using System.Threading;
using Cr7Sund.FrameWork.Util;


namespace Cr7Sund
{
    public struct UnsafeCancellationToken
    {
        public static UnsafeCancellationToken None => default;

        private UnsafeCancellationTokenSource _source;
        private int _version;

        public bool IsCancellationRequested
        {
            get
            {
                return Validate() ? _source.IsCancellationRequested : false;
            }
        }
        // if change to CancellationToken
        // skip it
        public bool IsValid => _source != null && _version == _source.Version;


        public UnsafeCancellationToken(UnsafeCancellationTokenSource source, int version)
        {
            _source = source;
            _version = version;
        }

        public void Register(Action action)
        {
            if (_source == null)
            {
                throw new System.Exception("try to use default token");
            }

            if (Validate())
            {
                _source.Register(action);
            }
        }

        private bool Validate()
        {
            if (_source == null)
            {
                return false;
            }

            if (_source.Version != _version)
            {
                throw new System.Exception("try to use old version token");
            }

            return true;
        }
    }

    public class UnsafeCancellationTokenSource : IDisposable, IPoolNode<UnsafeCancellationTokenSource>
    {
        private static ReusablePool<UnsafeCancellationTokenSource> _pool;

        private List<Action> _registrations;
        private short _version;
        private int _state;
        private UnsafeCancellationTokenSource _nextNode;
        // NotCanceledState 0,
        // NotifyingState  1,
        // NotifyingCompleteState 2,

        public bool IsCancellationRequested => _state > 0; // _state != State.NotCanceledState;
        public bool IsCancelling => _state == 1;
        public int Version => _version;
        public UnsafeCancellationToken Token
        {
            get
            {
                return new UnsafeCancellationToken(this, _version);
            }
        }
        public ref UnsafeCancellationTokenSource NextNode => ref _nextNode;

        public bool IsRecycled { get; set; }


        private UnsafeCancellationTokenSource()
        {

        }

        public static UnsafeCancellationTokenSource Create()
        {
            if (!_pool.TryPop(out var result))
            {
                result = new UnsafeCancellationTokenSource();
            }
            return result;
        }

        /// <summary>
        /// Cancellation is asynchronous, since some async jobs are still running after this call.
        /// However, we ensure the clean job will be done when the async job finish 
        /// </summary>
        public void Cancel()
        {
            ThrowIfCancel();
            ExecuteCallbackHandlers();
        }

        public void Register(Action action)
        {
            ThrowIfCancel();

            if (_registrations == null)
            {
                _registrations = new();
            }
            _registrations.Add(action);
        }

        public void Dispose()
        {
            //_state == State.NotifyingState
            if (_state == 1)
            {
                throw new MyException(PromiseTaskExceptionType.dispose_notifying_cancel);
            }

            if (_state != 0)
            {
                // rest to State.NotCanceledState;
                Interlocked.CompareExchange(ref _state, 0, 2);
            }
            _registrations?.Clear();
            _version++;
        }

        public CancellationTokenSource Join()
        {
            var cancellation = new CancellationTokenSource();

            Token.Register(cancellation.Cancel);
            return cancellation;
        }

        public void TryReturn()
        {
            Dispose();
            _pool.TryPush(this);
        }

        private void ExecuteCallbackHandlers()
        {
            // ->State.NotifyingState
            Interlocked.CompareExchange(ref _state, 1, 0);

            if (_registrations != null)
            {
                foreach (var callback in _registrations)
                {
                    try
                    {
                        callback.Invoke();
                    }
                    catch (System.Exception ex)
                    {
                        Console.Error(ex);
                    }
                }

            }
            // =》 State.NotifyingCompleteState;
            Interlocked.CompareExchange(ref _state, 2, 1);
        }

        private void ThrowIfCancel()
        {
            if (IsCancellationRequested)
            {
                throw new System.Exception("token is already canceled or cancelling");
            }
        }
    }
}