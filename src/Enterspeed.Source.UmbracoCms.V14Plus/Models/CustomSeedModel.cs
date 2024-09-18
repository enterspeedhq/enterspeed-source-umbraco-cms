using System.Collections.Generic;

namespace Enterspeed.Source.UmbracoCms.V14Plus.Models
{
    public class CustomSeedModel
    {
        public List<CustomSeedNode> ContentNodes { get; set; }
        public List<CustomSeedNode> MediaNodes { get; set; }
        public List<CustomSeedNode> DictionaryNodes { get; set; }
    }

    public class CustomSeedNode
    {
        public string Id { get; set; }
        public bool IncludeDescendants { get; set; }
    }
}