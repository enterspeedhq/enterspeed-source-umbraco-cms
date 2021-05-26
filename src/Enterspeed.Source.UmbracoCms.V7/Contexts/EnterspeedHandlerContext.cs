using Enterspeed.Source.UmbracoCms.V7.Handlers;

namespace Enterspeed.Source.UmbracoCms.V7.Contexts
{
    public class EnterspeedHandlerContext
    {
        private EnterspeedJobHandler _jobHandler;
        public EnterspeedJobHandler JobHandler => _jobHandler ?? (_jobHandler = new EnterspeedJobHandler());
    }
}
