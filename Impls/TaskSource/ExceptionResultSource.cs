using System;
using System.Diagnostics;
using System.Runtime.ExceptionServices;

namespace Cr7Sund
{
    public sealed class ExceptionResultSource : IPromiseTaskSource
    {
#if DEBUG
        readonly ExceptionDispatchInfo exception;
#else
        readonly Exception exception;
#endif
        bool calledGet;

        public ExceptionResultSource(Exception exception)
        {
#if DEBUG
            this.exception = ExceptionDispatchInfo.Capture(exception);
#else
            this.exception = exception;
#endif
        }


        [DebuggerHidden]
        public void GetResult(short token)
        {
            if (!calledGet)
            {
                calledGet = true;
                GC.SuppressFinalize(this);
            }
#if DEBUG
            exception.Throw();
#else
            throw exception;
#endif
        }

        [DebuggerHidden]
        void IPromiseTaskSource.GetResult(short token)
        {
            if (!calledGet)
            {
                calledGet = true;
                GC.SuppressFinalize(this);
            }
#if DEBUG
            exception.Throw();
#else
            throw exception;
#endif
        }
        [DebuggerHidden]
        public PromiseTaskStatus GetStatus(short token)
        {
            return PromiseTaskStatus.Faulted;
        }

        public PromiseTaskStatus UnsafeGetStatus()
        {
            return PromiseTaskStatus.Faulted;
        }

        public void OnCompleted(Action continuation, short token)
        {
            continuation?.Invoke();
        }

        ~ExceptionResultSource()
        {
            if (!calledGet)
            {
#if DEBUG
                Console.Error(exception.SourceException);
#else
                Console.Error(exception);
#endif
                //UniTaskScheduler.PublishUnobservedTaskException(exception.SourceException);
            }
        }
    }

    public sealed class ExceptionResultSource<T> : IPromiseTaskSource<T>
    {
#if DEBUG
        readonly ExceptionDispatchInfo exception;
#else
        readonly Exception exception;
#endif
        bool calledGet;

        public ExceptionResultSource(Exception exception)
        {
#if DEBUG
            this.exception = ExceptionDispatchInfo.Capture(exception);
#else
            this.exception = exception;
#endif
        }


        [DebuggerHidden]
        public T GetResult(short token)
        {
            if (!calledGet)
            {
                calledGet = true;
                GC.SuppressFinalize(this);
            }
#if DEBUG
            exception.Throw();
            return default;
#else
            throw exception;
#endif
        }

        void IPromiseTaskSource.GetResult(short token)
        {
            if (!calledGet)
            {
                calledGet = true;
                GC.SuppressFinalize(this);
            }
#if DEBUG
            exception.Throw();
#else
            throw exception;
#endif
        }

        public PromiseTaskStatus GetStatus(short token)
        {
            return PromiseTaskStatus.Faulted;
        }

        public PromiseTaskStatus UnsafeGetStatus()
        {
            return PromiseTaskStatus.Faulted;
        }

        public void OnCompleted(Action continuation, short token)
        {
            continuation?.Invoke();
        }

        ~ExceptionResultSource()
        {
            if (!calledGet)
            {
                //UniTaskScheduler.PublishUnobservedTaskException(exception.SourceException);
            }
        }
    }


}
