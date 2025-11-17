using System.Text.Json.Serialization;

namespace OmniPyme.Web.Request.Mailtrap
{
    public class SendEmailRequest
    {
        [JsonPropertyName("from")]
        public From from { get; set; }

        [JsonPropertyName("to")]
        public List<To> to { get; set; }

        [JsonPropertyName("subject")]
        public string subject { get; set; }

        [JsonPropertyName("text")]
        public string text { get; set; }

        [JsonPropertyName("category")]
        public string category { get; set; } = "Integration Test";
    }

    public class From
    {
        [JsonPropertyName("email")]
        public string email { get; set; }

        [JsonPropertyName("name")]
        public string name { get; set; }
    }

    public class To
    {
        [JsonPropertyName("email")]
        public string email { get; set; }
    }
}
