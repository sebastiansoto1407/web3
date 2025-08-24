using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace web1.Pages
{
    public class EditarModel : PageModel
    {
        private readonly IWebHostEnvironment _env;
        public EditarModel(IWebHostEnvironment env) => _env = env;

        public class TareaJson
        {
            public string? NombreTarea { get; set; }
            public string? FechaVencimiento { get; set; }
            public string? Estado { get; set; }
        }

        public class TareaInput
        {
            [Required, StringLength(200)]
            public string? NombreTarea { get; set; }

            [Required, DataType(DataType.Date)]
            public string? FechaVencimiento { get; set; }

            [Required]
            public string? Estado { get; set; }
        }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        [BindProperty]
        public TareaInput Input { get; set; } = new();

        public string? MensajeBloqueo { get; private set; }

        private string GetRuta() => Path.Combine(_env.WebRootPath, "data", "tareas.json");

        public IActionResult OnGet()
        {
            var ruta = GetRuta();
            if (!System.IO.File.Exists(ruta)) return RedirectToPage("Copia");

            var json = System.IO.File.ReadAllText(ruta);
            var lista = JsonSerializer.Deserialize<List<TareaJson>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<TareaJson>();

            if (Id < 0 || Id >= lista.Count) return RedirectToPage("Copia");

            var item = lista[Id];
            var estado = (item.Estado ?? "").Trim().ToLowerInvariant();

            if (estado.StartsWith("final"))
            {
                MensajeBloqueo = "Esta tarea ya está finalizada y no se puede editar.";
                return Page();
            }

            Input.NombreTarea = item.NombreTarea;
            if (DateTime.TryParse(item.FechaVencimiento, out var dt))
                Input.FechaVencimiento = dt.ToString("yyyy-MM-dd");
            else
                Input.FechaVencimiento = item.FechaVencimiento;

            Input.Estado = item.Estado;

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid) return Page();

            var ruta = GetRuta();
            if (!System.IO.File.Exists(ruta)) return RedirectToPage("Copia");

            var json = System.IO.File.ReadAllText(ruta);
            var lista = JsonSerializer.Deserialize<List<TareaJson>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<TareaJson>();

            if (Id < 0 || Id >= lista.Count) return RedirectToPage("Copia");

            var item = lista[Id];
            var estadoActual = (item.Estado ?? "").Trim().ToLowerInvariant();
            if (estadoActual.StartsWith("final"))
            {
                TempData["Msg"] = "La tarea fue finalizada y ya no puede editarse.";
                return RedirectToPage("Copia");
            }

            item.NombreTarea = Input.NombreTarea;

            if (DateTime.TryParse(Input.FechaVencimiento, out var dt))
                item.FechaVencimiento = dt.ToString("dd/MM/yyyy");
            else
                item.FechaVencimiento = Input.FechaVencimiento;

            item.Estado = Input.Estado;

            var nuevoJson = JsonSerializer.Serialize(lista, new JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(ruta, nuevoJson);

            return RedirectToPage("Copia", new { Refrescar = true });
        }
    }
}
