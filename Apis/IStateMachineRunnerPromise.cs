using System;

namespace Cr7Sund.CompilerServices
{
    public interface IStateMachineRunnerPromise
    {
        Action MoveNext { get; }
        PromiseTask Task { get; }

        void SetResult();
        void SetException(Exception exception);
    }

    public interface IStateMachineRunnerPromise<T>
    {
        Action MoveNext { get; }
        PromiseTask<T> Task { get; }
        void SetResult(T result);
        void SetException(Exception exception);
    }

}