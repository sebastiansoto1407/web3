using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
//configure el json
namespace web1.Pages
{
    public class CopiaModel : PageModel
    {
        public class TareaJson
        {
            public string? NombreTarea { get; set; }
            public string? FechaVencimiento { get; set; }  
            public string? Estado { get; set; }            
        }

        private static List<TareaJson> _todas = new();    
        private readonly IWebHostEnvironment _env;
        public CopiaModel(IWebHostEnvironment env) => _env = env;

        [BindProperty(SupportsGet = true)]
        public int Pagina { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int Tamano { get; set; } = 5; 

        [BindProperty(SupportsGet = true)]
        public string? FiltroEstado { get; set; } = "todos"; 

        public List<TareaJson> Tareas { get; private set; } = new();
        public int TotalPaginas { get; private set; }
        public int TotalRegistros { get; private set; }

        public int CntPendientes { get; private set; }
        public int CntEnCurso { get; private set; }
        public int CntFinalizados { get; private set; }

        public void OnGet()
        {
            if (_todas.Count == 0)
            {
                var ruta = Path.Combine(_env.WebRootPath, "data", "tareas.json");
                if (System.IO.File.Exists(ruta))
                {
                    var json = System.IO.File.ReadAllText(ruta);
                    _todas = JsonSerializer.Deserialize<List<TareaJson>>(
                        json,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    ) ?? new List<TareaJson>();
                }
            }

            CntPendientes = _todas.Count(t => NormalizarEstado(t.Estado) == "pendientes");
            CntEnCurso = _todas.Count(t => NormalizarEstado(t.Estado) == "en-curso");
            CntFinalizados = _todas.Count(t => NormalizarEstado(t.Estado) == "finalizados");

            IEnumerable<TareaJson> consulta = _todas;
            var filtro = (FiltroEstado ?? "todos").Trim().ToLowerInvariant();
            if (filtro != "todos")
                consulta = consulta.Where(t => NormalizarEstado(t.Estado) == filtro);

            if (Tamano < 5 || Tamano > 10) Tamano = 5;   
            TotalRegistros = consulta.Count();
            TotalPaginas = Math.Max(1, (int)Math.Ceiling(TotalRegistros / (double)Tamano));
            if (Pagina < 1) Pagina = 1;
            if (Pagina > TotalPaginas) Pagina = TotalPaginas;

            Tareas = consulta
                .Skip((Pagina - 1) * Tamano)
                .Take(Tamano)
                .ToList();
        }

        private static string NormalizarEstado(string? estado)
        {
            if (string.IsNullOrWhiteSpace(estado)) return "pendientes";
            var e = estado.Trim().ToLowerInvariant();
            if (e.Contains("curso")) return "en-curso";
            if (e.StartsWith("final")) return "finalizados";
            return "pendientes";
        }
    }
}
