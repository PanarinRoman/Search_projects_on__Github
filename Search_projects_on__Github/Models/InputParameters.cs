using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Search_projects_on__Github.Models
{
    public class InputParameters
    {
        [BindProperty]
        [RegularExpression(@"[A-Za-z0-9_ ]{2,50}")]
        public string Subject { get; set; }
    }
}
