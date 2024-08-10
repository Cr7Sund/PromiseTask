using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Cr7Sund
{
    public interface ITaskPoolNode<T>
    {
        ref T NextNode { get; }
    }

    public struct TaskPool<T>
        where T : class, ITaskPoolNode<T>
    {
        public static int MaxPoolSize = Int32.MaxValue;

        int gate;
        int size;
        T root;
        public int Size => size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryPop(out T result)
        {
            if (Interlocked.CompareExchange(ref gate, 1, 0) == 0)
            {
                var v = root;
                if (!(v is null))
                {
                    ref var nextNode = ref v.NextNode;
                    root = nextNode;
                    nextNode = null;
                    size--;
                    result = v;
                    Volatile.Write(ref gate, 0);
                    return true;
                }

                Volatile.Write(ref gate, 0);
            }
            result = default;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerHidden]
        public bool TryPush(T item)
        {
            if (Interlocked.CompareExchange(ref gate, 1, 0) == 0)
            {
                if (size < MaxPoolSize)
                {
                    item.NextNode = root;
                    root = item;
                    size++;
                    Volatile.Write(ref gate, 0);
                    return true;
                }
                else
                {
                    Volatile.Write(ref gate, 0);
                }
            }
            return false;
        }
    }
}
