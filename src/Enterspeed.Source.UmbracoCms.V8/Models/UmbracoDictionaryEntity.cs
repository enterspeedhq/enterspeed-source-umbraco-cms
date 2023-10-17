using System.Collections.Generic;
using Enterspeed.Source.Sdk.Api.Models;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.V8.Services;
using Umbraco.Core.Models;

namespace Enterspeed.Source.UmbracoCms.V8.Models
{
    public class UmbracoDictionaryEntity : IEnterspeedEntity<IDictionary<string, IEnterspeedProperty>>
    {
        private readonly IDictionaryItem _dictionaryItem;
        private readonly IEntityIdentityService _entityIdentityService;
        private readonly string _culture;

        public UmbracoDictionaryEntity(
            IDictionaryItem dictionaryItem,
            IEnterspeedPropertyService propertyService,
            IEntityIdentityService entityIdentityService,
            string culture)
        {
            _dictionaryItem = dictionaryItem;
            _entityIdentityService = entityIdentityService;
            _culture = culture;
            Properties = propertyService.GetProperties(_dictionaryItem, _culture);
        }

        public string Id => _entityIdentityService.GetId(_dictionaryItem, _culture);
        public string Type => "umbDictionary";
        public string Url => null;
        public string[] Redirects => null;
        public string ParentId => _dictionaryItem.ParentId.HasValue ? _entityIdentityService.GetId(_dictionaryItem.ParentId, _culture) : null;
        public IDictionary<string, IEnterspeedProperty> Properties { get; }
    }
}
