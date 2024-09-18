using System;

namespace Enterspeed.Source.UmbracoCms.Exceptions
{
    public class JobHandlingException : Exception
    {
        public JobHandlingException(string message) : base(message)
        {
        }
    }
}