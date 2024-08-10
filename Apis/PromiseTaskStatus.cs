using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Cr7Sund
{
    public enum PromiseTaskStatus
    {
        /// <summary>The operation has not yet completed.</summary>
        Pending = 0,
        /// <summary>The operation completed successfully.</summary>
        Succeeded = 1,
        /// <summary>The operation completed with an error.</summary>
        Faulted = 2,
        /// <summary>The operation completed due to cancellation.</summary>
        Canceled = 3
    }

    public static class UniTaskStatusExtensions
    {
        /// <summary>status != Pending.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerHidden]
        public static bool IsCompleted(this PromiseTaskStatus status)
        {
            return status != PromiseTaskStatus.Pending;
        }

        /// <summary>status == Succeeded.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsCompletedSuccessfully(this PromiseTaskStatus status)
        {
            return status == PromiseTaskStatus.Succeeded;
        }

        /// <summary>status == Canceled.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsCanceled(this PromiseTaskStatus status)
        {
            return status == PromiseTaskStatus.Canceled;
        }

        /// <summary>status == Faulted.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFaulted(this PromiseTaskStatus status)
        {
            return status == PromiseTaskStatus.Faulted;
        }
    }

}
