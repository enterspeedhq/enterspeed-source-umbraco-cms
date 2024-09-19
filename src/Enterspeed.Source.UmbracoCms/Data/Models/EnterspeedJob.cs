using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Enterspeed.Source.UmbracoCms.Base.Data.Models
{
    public class EnterspeedJob
    {
        public int Id { get; set; }

        public string EntityId { get; set; }

        public string Culture { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public EnterspeedJobType JobType { get; set; }

        public DateTime CreatedAt { get; set; }

        public EnterspeedJobState State { get; set; }

        public string Exception { get; set; }

        public DateTime UpdatedAt { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public EnterspeedJobEntityType EntityType { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public EnterspeedContentState ContentState { get; set; }
    }
}
