using Search_projects_on__Github.Models;
using Search_projects_on__Github.Models.GitResponse;
using Search_projects_on__Github.Models.MySql;
using Search_projects_on__Github.Models.Search;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Search_projects_on__Github.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class FindControllers : ControllerBase
    {
        [BindProperty]
        public InputParameters parametr { get; set; }

        private IConfigurationRoot ConfigurationRoot;
        private readonly IHttpClientFactory clientFactory;

        private readonly string stringConnectionDb;
        private readonly Version version;
        private readonly string urlGithubMethod;



        public FindControllers(IConfiguration configRoot, IHttpClientFactory clientFactory)
        {
            ConfigurationRoot = (IConfigurationRoot)configRoot;
            this.clientFactory = clientFactory;


            stringConnectionDb = ConfigurationRoot.GetConnectionString("DefaultConnection");

            urlGithubMethod = ConfigurationRoot["UrlGithubMethod"];

            version = new Version(int.Parse(ConfigurationRoot["Version:Major"]),
                                        int.Parse(ConfigurationRoot["Version:Minor"]),
                                            int.Parse(ConfigurationRoot["Version:Build"]));
        }

        [HttpGet]
        public ActionResult<SearchlDModel[]> Get()
        {
            using (MySqlDbContext db = new MySqlDbContext(stringConnectionDb, version))
            {
                SearchModel[] searchModels = db.SearchList.ToArray();

                SearchlDModel[] searchIdModels = new SearchlDModel[searchModels.Length];

                for (int i = 0; i < searchModels.Length; i++)
                {
                    SearchlDModel itemIdModel = new SearchlDModel();

                    itemIdModel.Id = searchModels[i].Id;
                    itemIdModel.Request = searchModels[i].Request;

                    searchIdModels[i] = itemIdModel;
                }

                return searchIdModels;
            }
        }

        [HttpPost]
        public async Task<ActionResult<ProjectModel[]>> PostAsync([FromBody] InputParameters parametr)
        {
            string jsonResponse = "";

            // поиск в б.д.
            using (MySqlDbContext db = new MySqlDbContext(stringConnectionDb, version))
            {
                SearchModel searchModel = db.SearchList.Where(p => p.Request == parametr.Subject).FirstOrDefault();

                if (searchModel != null)
                {
                    jsonResponse = searchModel.Response;
                }
                else
                {
                    // запрос на githab
                    HttpClient client = clientFactory.CreateClient("github");
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, urlGithubMethod + parametr.Subject);
                    HttpResponseMessage responseMessage = await client.SendAsync(request);

                    jsonResponse = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

                    // сохранение в базу данных
                    SearchModel searchRequest = new SearchModel { Request = parametr.Subject, Response = jsonResponse };
                    db.SearchList.AddRange(searchRequest);
                    await db.SaveChangesAsync();
                }
            }

            GitResponceModel gitHubResponceModel = JsonSerializer.Deserialize<GitResponceModel>(jsonResponse);

            return gitHubResponceModel.Projects;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            using (MySqlDbContext db = new MySqlDbContext(stringConnectionDb, version))
            {
                SearchModel searchModel = db.SearchList.Where(p => p.Id == id).FirstOrDefault();

                if (searchModel == null)
                {
                    return NotFound();
                }
                else
                {
                    db.SearchList.Remove(searchModel);
                    await db.SaveChangesAsync();

                    return Ok();
                }
            }
        }
    }
}
