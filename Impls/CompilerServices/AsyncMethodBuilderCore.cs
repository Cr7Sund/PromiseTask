using System.Runtime.CompilerServices;
using System;
using System.Diagnostics;

namespace Cr7Sund.CompilerServices
{
    public sealed class AsyncMethodBuilderCore<TStateMachine> : IStateMachineRunnerPromise, IPromiseTaskSource, ITaskPoolNode<AsyncMethodBuilderCore<TStateMachine>>
    where TStateMachine : IAsyncStateMachine
    {
        private static TaskPool<AsyncMethodBuilderCore<TStateMachine>> pool;

        private TStateMachine stateMachine;
        private PromiseTaskCompletionSourceCore core;
        private AsyncMethodBuilderCore<TStateMachine> nextNode;

        public Action MoveNext { get; }
        public PromiseTask Task => new PromiseTask(this, core.Version);
        public ref AsyncMethodBuilderCore<TStateMachine> NextNode => ref nextNode;


        public AsyncMethodBuilderCore()
        {
            //Task = new PromiseTask(this, core.Version);
            MoveNext = Run;
        }

        [DebuggerHidden]
        public static void SetStateMachine(ref TStateMachine stateMachine, ref IStateMachineRunnerPromise runnerPromiseFieldRef)
        {
            if (!pool.TryPop(out var result))
            {
                result = new AsyncMethodBuilderCore<TStateMachine>();
            }

            runnerPromiseFieldRef = result; // set runner before copied.
            result.stateMachine = stateMachine; // copy struct StateMachine(in release build).
        }
        [DebuggerHidden]
        public void GetResult(short token)
        {
            try
            {
                core.GetResult(token);
            }
            finally
            {
                TryReturn();
            }
        }
        [DebuggerHidden]
        public PromiseTaskStatus GetStatus(short token)
        {
            return core.GetStatus(token);
        }
        [DebuggerHidden]
        public void SetException(Exception exception)
        {
            core.TrySetException(exception);
        }
        [DebuggerHidden]
        public void SetResult()
        {
            core.TrySetResult();
        }
        [DebuggerHidden]
        void IPromiseTaskSource.GetResult(short token)
        {
            try
            {
                core.GetResult(token);
            }
            finally
            {
                TryReturn();
            }
        }

        [DebuggerHidden]
        public PromiseTaskStatus UnsafeGetStatus()
        {
            return core.UnsafeGetStatus();
        }
        [DebuggerHidden]
        public void OnCompleted(Action continuation, short token)
        {
            core.OnCompleted(continuation, token);
        }
        [DebuggerHidden]
        private bool TryReturn()
        {
            core.Reset();
            stateMachine = default;
            return pool.TryPush(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerHidden]
        private void Run()
        {
            stateMachine.MoveNext();
        }
    }


    public sealed class AsyncMethodBuilderCore<TStateMachine, T> : IStateMachineRunnerPromise<T>, IPromiseTaskSource<T>, ITaskPoolNode<AsyncMethodBuilderCore<TStateMachine, T>>
        where TStateMachine : IAsyncStateMachine
    {
        private static TaskPool<AsyncMethodBuilderCore<TStateMachine, T>> pool;

        private TStateMachine stateMachine;
        private PromiseTaskCompletionSourceCore<T> core;
        private AsyncMethodBuilderCore<TStateMachine, T> nextNode;

        public Action MoveNext { get; }
        public PromiseTask<T> Task => new PromiseTask<T>(this, core.Version);
        public ref AsyncMethodBuilderCore<TStateMachine, T> NextNode => ref nextNode;


        public AsyncMethodBuilderCore()
        {
            //Task = new PromiseTask<T>(this, core.Version);
            MoveNext = Run;
        }


        public static void SetStateMachine(ref TStateMachine stateMachine, ref IStateMachineRunnerPromise<T> runnerPromiseFieldRef)
        {
            if (!pool.TryPop(out var result))
            {
                result = new AsyncMethodBuilderCore<TStateMachine, T>();
            }

            runnerPromiseFieldRef = result; // set runner before copied.
            result.stateMachine = stateMachine; // copy struct StateMachine(in release build).
        }
        [DebuggerHidden]
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
        [DebuggerHidden]
        public PromiseTaskStatus GetStatus(short token)
        {
            return core.GetStatus(token);
        }
        [DebuggerHidden]
        public void SetException(Exception exception)
        {
            core.TrySetException(exception);
        }
        [DebuggerHidden]
        public void SetResult(T result)
        {
            core.TrySetResult(result);
        }
        [DebuggerHidden]
        void IPromiseTaskSource.GetResult(short token)
        {
            try
            {
                core.GetResult(token);
            }
            finally
            {
                TryReturn();
            }
        }

        public PromiseTaskStatus UnsafeGetStatus()
        {
            return core.UnsafeGetStatus();
        }

        public void OnCompleted(Action continuation, short token)
        {
            core.OnCompleted(continuation, token);
        }

        private bool TryReturn()
        {
            core.Reset();
            stateMachine = default;
            return pool.TryPush(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerHidden]
        private void Run()
        {
            stateMachine.MoveNext();
        }
    }

}