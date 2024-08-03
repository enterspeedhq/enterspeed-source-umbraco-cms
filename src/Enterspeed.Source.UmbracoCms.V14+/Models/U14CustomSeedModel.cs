using System.Collections.Generic;

namespace Enterspeed.Source.UmbracoCms.V14.Models
{
    public class U14CustomSeedModel
    {
        public List<U14CustomSeedNode> ContentNodes { get; set; }
        public List<U14CustomSeedNode> MediaNodes { get; set; }
        public List<U14CustomSeedNode> DictionaryNodes { get; set; }
    }

    public class U14CustomSeedNode
    {
        public string Id { get; set; }
        public bool IncludeDescendants { get; set; }
    }
}