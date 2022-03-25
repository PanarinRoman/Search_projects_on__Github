using System;
using Search_projects_on__Github.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Search_projects_on__Github.Models.MySql;
using Search_projects_on__Github.Models.GitResponse;
using Search_projects_on__Github.Models.Search;

namespace Search_projects_on__Github.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        [RegularExpression(@"[A-Za-z0-9_ ]{2,50}")]
        public string Subject { get; set; }

        public string Info { get; set; }

        public GitResponceModel GitResponceModel;

        private readonly IConfigurationRoot ConfigurationRoot;
        private readonly IHttpClientFactory clientFactory;

        private readonly string stringConnectionDb;
        private readonly Version version;
        private readonly string urlGithubMethod = "https://api.github.com/search/repositories?q=";



        public IndexModel(IConfiguration configRoot, IHttpClientFactory clientFactory)
        {
            ConfigurationRoot = (IConfigurationRoot)configRoot;
            this.clientFactory = clientFactory;

            stringConnectionDb = ConfigurationRoot.GetConnectionString("DefaultConnection");

            urlGithubMethod = ConfigurationRoot["UrlGithubMethod"];

            version = new Version(int.Parse(ConfigurationRoot["Version:Major"]),
                                        int.Parse(ConfigurationRoot["Version:Minor"]),
                                            int.Parse(ConfigurationRoot["Version:Build"]));
        }

        public void OnGet()
        {
            Info = "";
        }
        
        public async Task OnPostAsync(string Subject)
        {
            // проверка введенных данных
            if (!ModelState.IsValid)
            {
                Info = "Ошибка ввода!";
                return;
            }
            else
            {
                Info = "";
            }

            string response = "";

            // поиск запроса в б.д.
            using (MySqlDbContext db = new MySqlDbContext(stringConnectionDb, version))
            {
                var searchRequests = db.SearchList.Where(p => p.Request == Subject).FirstOrDefault();

                if (searchRequests != null)
                {
                    response = searchRequests.Response;
                }
                else
                {
                    // поиск на githab
                    HttpClient client = clientFactory.CreateClient("github");
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, urlGithubMethod + Subject);
                    HttpResponseMessage responseMessage = await client.SendAsync(request);

                    response = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

                    // сохранение в б.д
                    SearchModel searchRequest = new SearchModel { Request = Subject, Response = response };
                    db.SearchList.AddRange(searchRequest);
                    db.SaveChanges();
                }
            }

            this.GitResponceModel = JsonSerializer.Deserialize<GitResponceModel>(response);
        }
    }
}
