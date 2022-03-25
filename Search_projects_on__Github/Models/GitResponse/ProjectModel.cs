using System.Text.Json.Serialization;

namespace Search_projects_on__Github.Models.GitResponse
{
    public class ProjectModel
    {
        /// <summary>
        /// Имя проекта 
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }
        /// <summary>
        /// Автор
        /// </summary>
        [JsonPropertyName("creator")]
        public CreatorModel Creator { get; set; }
        /// <summary>
        /// Количество звезд 
        /// </summary>
        [JsonPropertyName("stargazers")]
        public int Stargazers { get; set; }
        /// <summary>
        /// Количество просмотров 
        /// </summary>
        [JsonPropertyName("watchers")]
        public int Watchers { get; set; }

    }
}
