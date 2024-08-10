using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Cr7Sund.CompilerServices
{
    public struct PromiseTaskAwaiter : ICriticalNotifyCompletion
    {
        private readonly PromiseTask task;

        [DebuggerHidden]
        public bool IsCompleted
        {
            [DebuggerHidden]
            get
            {
                return task.Status.IsCompleted();
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerHidden]
        public PromiseTaskAwaiter(in PromiseTask task)
        {
            this.task = task;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerHidden]
        /// <summary>Ends the await on the completed <see cref="System.Threading.Tasks.Task{TResult}"/>.</summary>
        public void GetResult()
        {
            var s = task.source;
            if (s != null)
            {
                s.GetResult(task.token);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnCompleted(Action continuation)
        {
            var s = task.source;
            if (s == null)
            {
                continuation();
            }
            else
            {
                s.OnCompleted(continuation, task.token);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <summary>Schedules the continuation onto the <see cref="System.Threading.Tasks.Task"/> associated with this <see cref="TaskAwaiter"/>.</summary>
        /// <param name="continuation">The action to invoke when the await operation completes.</param
        public void UnsafeOnCompleted(Action continuation)
        {
            var s = task.source;
            if (s == null)
            {
                continuation();
            }
            else
            {
                s.OnCompleted(continuation, task.token);
            }
        }

        public void SourceOnCompleted(Action continuation)
        {
            var s = task.source;
            if (s == null)
            {
                continuation();
            }
            else
            {
                s.OnCompleted(continuation, task.token);
            }
        }
    }
}