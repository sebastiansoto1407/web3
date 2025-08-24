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

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            var ruta = Path.Combine(_env.WebRootPath, "data", "tareas.json");
            var lista = new List<TareaJson>();

            if (System.IO.File.Exists(ruta))
            {
                var json = System.IO.File.ReadAllText(ruta);
                lista = JsonSerializer.Deserialize<List<TareaJson>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<TareaJson>();
            }

            if (DateTime.TryParse(Input.FechaVencimiento, out var dt))
                Input.FechaVencimiento = dt.ToString("dd/MM/yyyy");

            lista.Add(Input);

            var nuevoJson = JsonSerializer.Serialize(lista, new JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(ruta, nuevoJson);

            return RedirectToPage("Copia", new { Refrescar = true });
        }
    }
}
