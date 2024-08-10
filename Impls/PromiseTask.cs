#pragma warning disable CS0436
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Cr7Sund.CompilerServices;

namespace Cr7Sund
{
    [AsyncMethodBuilder(typeof(PromiseTaskMethodBuilder<>))]
    public partial struct PromiseTask<T>
    {
        public readonly IPromiseTaskSource<T> source;
        public readonly T result;
        public readonly short token;

        public PromiseTaskStatus Status
        {
            [DebuggerHidden]
            get
            {
                return (source == null) ? PromiseTaskStatus.Succeeded : source.GetStatus(token);
            }
        }

        public PromiseTask(T result, short token = 10)
        {
            this.source = null;
            this.token = token;
            this.result = result;
        }

        public PromiseTask(IPromiseTaskSource<T> source, short token)
        {
            this.source = source;
            this.token = token;
            this.result = default;
        }

        [DebuggerHidden]
        public PromiseTaskAwaiter<T> GetAwaiter() => new PromiseTaskAwaiter<T>(this);

    }

}