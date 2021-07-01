using System.Collections.Generic;
using Enterspeed.Source.Sdk.Api.Models;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.V7.Contexts;
using Enterspeed.Source.UmbracoCms.V7.Services;
using Umbraco.Core.Models;

namespace Enterspeed.Source.UmbracoCms.V7.Models
{
    public class UmbracoDictionaryEntity : IEnterspeedEntity
    {
        private readonly IDictionaryItem _dictionaryItem;
        private readonly EntityIdentityService _entityIdentityService;
        private readonly string _culture;

        public UmbracoDictionaryEntity(
            IDictionaryItem dictionaryItem,
            string culture)
        {
            _dictionaryItem = dictionaryItem;
            _entityIdentityService = EnterspeedContext.Current.Services.EntityIdentityService;
            _culture = culture;

            var propertyService = EnterspeedContext.Current.Services.PropertyService;
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
