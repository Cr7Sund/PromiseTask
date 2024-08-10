using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Cr7Sund.CompilerServices
{
    public struct PromiseTaskMethodBuilder
    {
        private IStateMachineRunnerPromise runnerPromise;
        private Exception ex;

        public PromiseTask Task
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (runnerPromise == null)
                {
                    if (ex != null)
                    {
                        return PromiseTask.FromException(ex);
                    }
                    return PromiseTask.CompletedTask;
                }
                else
                {
                    return runnerPromise.Task;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerHidden]

        public static PromiseTaskMethodBuilder Create()
            => new PromiseTaskMethodBuilder();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerHidden]
        public void Start<TStateMachine>(ref TStateMachine stateMachine)
            where TStateMachine : IAsyncStateMachine
        {
            stateMachine.MoveNext();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerHidden]
        public void SetResult()
        {
            if (runnerPromise != null)
            {
                runnerPromise.SetResult();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerHidden]
        public void SetException(Exception exception)
        {
            if (runnerPromise != null)
            {
                runnerPromise.SetException(exception);
            }
            else
            {
                ex = exception;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : INotifyCompletion where TStateMachine : IAsyncStateMachine
        {
            if (runnerPromise == null)
                AsyncMethodBuilderCore<TStateMachine>.SetStateMachine(ref stateMachine, ref runnerPromise);
            awaiter.OnCompleted(runnerPromise.MoveNext);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <summary>
        ///  register itself as the task’s continuation.
        /// </summary>
        /// <typeparam name="TAwaiter"></typeparam>
        /// <typeparam name="TStateMachine"></typeparam>
        /// <param name="awaiter"></param>
        /// <param name="stateMachine"></param>
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine
        {
            // aovid boxed operation (interface to struct)
            if (runnerPromise == null)
                AsyncMethodBuilderCore<TStateMachine>.SetStateMachine(ref stateMachine, ref runnerPromise);
            awaiter.UnsafeOnCompleted(runnerPromise.MoveNext);
        }

        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
            // don't use boxed stateMachine.
        }
    }

}