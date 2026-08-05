using System;

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

        // Umbraco 18 removed the int-keyed dictionary item lookup, so dictionary
        // nodes must carry the Guid key alongside the int id
        public Guid? Key { get; set; }
        public bool IncludeDescendants { get; set; }
    }
}
