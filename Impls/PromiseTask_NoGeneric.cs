#pragma warning disable CS0436
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Cr7Sund.CompilerServices;

namespace Cr7Sund
{
    [AsyncMethodBuilder(typeof(PromiseTaskMethodBuilder))]
    public partial struct PromiseTask
    {
        public static readonly PromiseTask CompletedTask = new PromiseTask();

        public readonly IPromiseTaskSource source;
        public readonly short token;

        public PromiseTaskStatus Status
        {
            [DebuggerHidden]
            get
            {
                return (source == null) ? PromiseTaskStatus.Succeeded : source.GetStatus(token);
            }
        }

        public PromiseTask(short token = 10)
        {
            this.source = null;
            this.token = token;
        }

        public PromiseTask(IPromiseTaskSource source, short token)
        {
            this.source = source;
            this.token = token;
        }

        [DebuggerHidden]
        public PromiseTaskAwaiter GetAwaiter() => new PromiseTaskAwaiter(this);


    }

}