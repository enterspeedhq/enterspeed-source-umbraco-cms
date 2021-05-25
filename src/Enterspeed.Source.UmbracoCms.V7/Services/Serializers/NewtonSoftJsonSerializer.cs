using Enterspeed.Source.Sdk.Api.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Enterspeed.Source.UmbracoCms.V7.Services.Serializers
{
    public class NewtonSoftJsonSerializer : IJsonSerializer
    {
        private readonly JsonSerializerSettings _settings = new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public string Serialize(object value)
        {
            return JsonConvert.SerializeObject(value, _settings);
        }

        public T Deserialize<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value, _settings);
        }
    }
}