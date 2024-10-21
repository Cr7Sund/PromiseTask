using System;
namespace Cr7Sund
{
    public class PromiseTaskCancelException : Exception
    {
        public PromiseTaskCancelException(UnsafeCancellationTokenSource cancellationTokenSource)
        {
            
        }
    }
}
