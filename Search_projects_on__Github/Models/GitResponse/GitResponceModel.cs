using System.Text.Json.Serialization;

namespace Search_projects_on__Github.Models.GitResponse
{
    public class GitResponceModel
    {
        [JsonPropertyName("items")]
        public ProjectModel[] Projects { get; set; }
    }
}
