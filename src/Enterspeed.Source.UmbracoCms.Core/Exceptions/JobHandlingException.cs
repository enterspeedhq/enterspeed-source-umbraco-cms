using System;

namespace Enterspeed.Source.UmbracoCms.Core.Exceptions
{
    public class JobHandlingException : Exception
    {
        public JobHandlingException(string message) : base(message)
        {
        }
    }
}