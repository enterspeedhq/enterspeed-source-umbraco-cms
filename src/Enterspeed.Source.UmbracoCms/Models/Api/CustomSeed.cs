namespace Enterspeed.Source.UmbracoCms.Base.Models.Api
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
        public bool IncludeDescendants { get; set; }
    }
}
