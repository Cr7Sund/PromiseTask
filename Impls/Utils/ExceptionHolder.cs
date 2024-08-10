using System;
using System.Diagnostics;
using System.Runtime.ExceptionServices;

namespace Cr7Sund
{
    public class ExceptionHolder
    {
        private ExceptionDispatchInfo exception;
        private bool calledGet = false;

        public Exception InnerException
        {
            get
            {
                calledGet = true;
                return exception.SourceException;
            }
        }

        [DebuggerHidden]
        public ExceptionHolder(ExceptionDispatchInfo exception)
        {
            this.exception = exception;
        }

        [DebuggerHidden]
        public ExceptionDispatchInfo GetException()
        {
            if (!calledGet)
            {
                calledGet = true;
                GC.SuppressFinalize(this);
            }
            return exception;
        }

        ~ExceptionHolder()
        {
            if (!calledGet)
            {
                // Uncomment the line below to log the exception if needed
                // Console.Error(exception.SourceException);
            }
        }
    }
}