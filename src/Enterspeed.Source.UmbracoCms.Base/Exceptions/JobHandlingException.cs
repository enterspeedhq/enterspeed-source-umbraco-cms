using System;

namespace Enterspeed.Source.UmbracoCms.Base.Exceptions
{
    public class JobHandlingException : Exception
    {
        public JobHandlingException(string message) : base(message)
        {
        }
    }
}