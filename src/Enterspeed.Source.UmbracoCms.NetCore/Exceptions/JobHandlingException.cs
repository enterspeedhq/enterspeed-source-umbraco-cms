using System;

namespace Enterspeed.Source.UmbracoCms.NetCore.Exceptions
{
    public class JobHandlingException : Exception
    {
        public JobHandlingException(string message) : base(message)
        {
        }
    }
}