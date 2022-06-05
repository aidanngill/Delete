using Newtonsoft.Json;

namespace Delete.Api.Response
{
    internal class Search
    {
        [JsonProperty("total_results")]
        public int TotalResults { get; set; }
        [JsonProperty("messages")]
        private List<List<Message>> UnformattedMessages { get; set; }
        [JsonIgnore]
        public List<Message> Messages { get => UnformattedMessages.Select(m => m.First()).ToList(); }
    }
}
