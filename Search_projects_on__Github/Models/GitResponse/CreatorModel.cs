using System.Text.Json.Serialization;

namespace Search_projects_on__Github.Models.GitResponse
{
    public class CreatorModel
    {
        [JsonPropertyName("login")]
        public string Login { get; set; }
    }
}
