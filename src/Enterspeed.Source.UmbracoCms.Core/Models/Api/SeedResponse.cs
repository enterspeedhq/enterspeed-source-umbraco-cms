namespace Enterspeed.Source.UmbracoCms.Core.Models.Api
{
    public class SeedResponse
    {
        public int JobsAdded { get; set; }
        public int NumberOfPendingJobs { get; set; }
        public int ContentCount { get; set; }
        public int DictionaryCount { get; set; }
        public long MediaCount { get; set; }
    }
}
