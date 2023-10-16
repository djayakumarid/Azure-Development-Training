using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace jd1_webapp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public string? Fullname { get; set; }
        public string? FName { get; set; }
        public string? LName { get; set; }


        public IndexModel(IConfiguration configuration, ILogger<IndexModel> logger)
        {
            Fullname = configuration["FullName"];
            FName = configuration["Name:FirstName"];
            LName = configuration["Name:LastName"];
            _logger = logger;

        }

        public void OnGet()
        {

        }
    }
}