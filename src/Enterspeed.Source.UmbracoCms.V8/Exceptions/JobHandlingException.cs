using System;

namespace Enterspeed.Source.UmbracoCms.V8.Exceptions
{
    public class JobHandlingException : Exception
    {
        public JobHandlingException(string message)
            : base(message)
        {
        }
    }
}