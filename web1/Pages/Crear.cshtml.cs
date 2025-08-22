using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace web1.Pages
{
    public class CrearModel : PageModel
    {
        private readonly IWebHostEnvironment _env;
        public CrearModel(IWebHostEnvironment env) => _env = env;

        public class TareaJson
        {
            [Required, StringLength(200)]
            public string? NombreTarea { get; set; }

            [Required, DataType(DataType.Date)]
            public string? FechaVencimiento { get; set; }

            [Required]
            public string? Estado { get; set; }
        }

        [BindProperty]
        public TareaJson Input { get; set; } = new();

        public void OnGet() { }

        
    }
}
