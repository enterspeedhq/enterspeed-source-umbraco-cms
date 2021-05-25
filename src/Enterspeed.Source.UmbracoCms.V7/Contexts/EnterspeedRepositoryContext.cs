using Enterspeed.Source.UmbracoCms.V7.Data.Repositories;

namespace Enterspeed.Source.UmbracoCms.V7.Contexts
{
    public class EnterspeedRepositoryContext
    {
        public EnterspeedJobRepository JobRepository => new EnterspeedJobRepository();
    }
}
