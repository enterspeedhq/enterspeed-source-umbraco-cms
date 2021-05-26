using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Enterspeed.Source.UmbracoCms.V7.Data.Models
{
    public class EnterspeedJob
    {
        public int Id { get; set; }

        public int ContentId { get; set; }

        public string Culture { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public EnterspeedJobType JobType { get; set; }

        public DateTime CreatedAt { get; set; }

        public EnterspeedJobState State { get; set; }

        public string Exception { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
