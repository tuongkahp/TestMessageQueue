using Newtonsoft.Json;

namespace TestMessageQueue.Models
{
    public class TestMessageModel
    {
        [JsonProperty("client-id")]
        public int ClientId { get; set; }

        [JsonProperty("timestamps")]
        public string TimeStamps { get; set; }
    }
}