using Cr7Sund.CompilerServices;
using System;
using System.Threading;

namespace Cr7Sund
{
    public sealed class WhenAllPromiseTaskSource : IPromiseTaskSource
    {
        private int completeCount;
        private int tasksLength;
        private PromiseTaskCompletionSourceCore<AsyncUnit> core;

        public WhenAllPromiseTaskSource(PromiseTask[] tasks, int tasksLength)
        {
            completeCount = 0;
            this.tasksLength = tasksLength;

            if (tasksLength == 0)
            {
                core.TrySetResult(AsyncUnit.Default);
                return;
            }

            for (int i = 0; i < tasksLength; i++)
            {
                PromiseTaskAwaiter awaiter;
                try
                {
                    awaiter = tasks[i].GetAwaiter();
                }
                catch (Exception ex)
                {
                    core.TrySetException(ex);
                    continue;
                }

                if (awaiter.IsCompleted)
                {
                    TryInvokeContinuation(this, awaiter, i);
                }
                else
                {
                    int index = i;
                    awaiter.SourceOnCompleted(() =>
                    {
                        TryInvokeContinuation(this, awaiter, index);
                    });
                }
            }
        }

        static void TryInvokeContinuation(WhenAllPromiseTaskSource self, in PromiseTaskAwaiter awaiter, int i)
        {
            try
            {
                awaiter.GetResult();
            }
            catch (Exception ex)
            {
                self.core.TrySetException(ex);
                return;
            }

            if (Interlocked.Increment(ref self.completeCount) == self.tasksLength)
            {
                self.core.TrySetResult(AsyncUnit.Default);
            }
        }

        public void GetResult(short token)
        {
            core.GetResult(token);
        }

        public PromiseTaskStatus GetStatus(short token)
        {
            return core.GetStatus(token);
        }

        public void OnCompleted(Action continuation, short token)
        {
            core.OnCompleted(continuation, token);
        }

        public PromiseTaskStatus UnsafeGetStatus()
        {
            return core.UnsafeGetStatus();
        }

        void IPromiseTaskSource.GetResult(short token)
        {
            core.GetResult(token);
        }
    }

    public sealed class WhenAllPromiseTaskSource<T> : IPromiseTaskSource<T[]>
    {
        private T[] result;
        private int completeCount;
        private PromiseTaskCompletionSourceCore<T[]> core;

        public WhenAllPromiseTaskSource(PromiseTask<T>[] tasks, int tasksLength)
        {
            completeCount = 0;

            if (tasksLength == 0)
            {
                result = Array.Empty<T>();
                core.TrySetResult(result);
                return;
            }

            result = new T[tasksLength];

            for (int i = 0; i < tasksLength; i++)
            {
                PromiseTaskAwaiter<T> awaiter;
                try
                {
                    awaiter = tasks[i].GetAwaiter();
                }
                catch (Exception ex)
                {
                    core.TrySetException(ex);
                    continue;
                }

                if (awaiter.IsCompleted)
                {
                    TryInvokeContinuation(this, awaiter, i);
                }
                else
                {
                    int index = i;
                    awaiter.SourceOnCompleted(() =>
                    {
                        TryInvokeContinuation(this, awaiter, index);
                    });
                }
            }
        }

        static void TryInvokeContinuation(WhenAllPromiseTaskSource<T> self, in PromiseTaskAwaiter<T> awaiter, int i)
        {
            try
            {
                self.result[i] = awaiter.GetResult();
            }
            catch (Exception ex)
            {
                self.core.TrySetException(ex);
                return;
            }

            if (Interlocked.Increment(ref self.completeCount) == self.result.Length)
            {
                self.core.TrySetResult(self.result);
            }
        }

        public T[] GetResult(short token)
        {
            // when the operation will fail-fast regardless of the order of rejection of the promises, and the error will always occur within the configured promise chain, enabling it to be caught in the normal way

            return core.GetResult(token);
        }

        public PromiseTaskStatus GetStatus(short token)
        {
            return core.GetStatus(token);
        }

        public void OnCompleted(Action continuation, short token)
        {
            core.OnCompleted(continuation, token);
        }

        public PromiseTaskStatus UnsafeGetStatus()
        {
            return core.UnsafeGetStatus();
        }

        void IPromiseTaskSource.GetResult(short token)
        {
            core.GetResult(token);
        }
    }

}
