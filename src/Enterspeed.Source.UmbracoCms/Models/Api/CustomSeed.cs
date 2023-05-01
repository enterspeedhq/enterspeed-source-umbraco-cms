namespace Enterspeed.Source.UmbracoCms.Models.Api
{
    public class CustomSeed
    {
        public CustomSeedNode[] ContentNodes { get; set; }
        public CustomSeedNode[] MediaNodes { get; set; }
        public CustomSeedNode[] DictionaryNodes { get; set; }
    }

    public class CustomSeedNode
    {
        public int Id { get; set; }
        public bool IncludeDescendents { get; set; }
    }
}
