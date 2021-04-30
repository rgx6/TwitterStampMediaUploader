using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TwitterStampMediaUploader
{
    public class StampListItem
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("image")]
        public string Image { get; set; }

        [JsonPropertyName("src")]
        public IList<string> Src { get; set; }

        [JsonPropertyName("isCover")]
        public bool? IsCover { get; set; }

        [JsonPropertyName("searchTag")]
        public string SearchTag { get; set; }
    }
}
