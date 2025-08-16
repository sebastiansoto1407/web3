using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace web1.Pages
{
    public class CopiaModel : PageModel
    {
        //puse clases
        public class Tarea
        {
            public int Id { get; set; }
            public string? Titulo { get; set; }
            public string? Descripcion { get; set; }
            public DateTime? FechaVencimiento { get; set; }
            public bool EstaCompletada { get; set; }
        }

        // la lista
        private static readonly List<Tarea> ListaTareas = new();
        private static int _contadorId = 1;

        //creacion de tareas
        [BindProperty] public string? NuevoTitulo { get; set; }
        [BindProperty] public string? NuevaDescripcion { get; set; }
        [BindProperty] public DateTime? NuevaFecha { get; set; }

        //editar una tarea xd
        [BindProperty] public int EditarId { get; set; }
        [BindProperty] public string? EditarTitulo { get; set; }
        [BindProperty] public string? EditarDescripcion { get; set; }
        [BindProperty] public DateTime? EditarFecha { get; set; }

        //filtros
        [BindProperty(SupportsGet = true)]
        public string? Estado { get; set; } = "todas";

        //lista
        public List<Tarea> Tareas { get; private set; } = new();

        
        public void OnGet()
        {
            Tareas = FiltrarTareas(Estado);
        }

        //agrego de tareas
        public IActionResult OnPostAgregar()
        {
            if (!string.IsNullOrWhiteSpace(NuevoTitulo))
            {
                ListaTareas.Add(new Tarea
                {
                    Id = _contadorId++,
                    Titulo = NuevoTitulo.Trim(),
                    Descripcion = NuevaDescripcion?.Trim(),
                    FechaVencimiento = NuevaFecha,
                    EstaCompletada = false
                });
            }
            return RedirectToPage(new { Estado });
        }

        //cambio de estados
        public IActionResult OnPostCambiarEstado(int id)
        {
            var tarea = ListaTareas.FirstOrDefault(x => x.Id == id);
            if (tarea != null)
            {
                tarea.EstaCompletada = !tarea.EstaCompletada;
            }
            return RedirectToPage(new { Estado });
        }

        //eliminar tarea
        public IActionResult OnPostEliminar(int id)
        {
            ListaTareas.RemoveAll(x => x.Id == id);
            return RedirectToPage(new { Estado });
        }

        //datos de edicion
        public void OnGetEditar(int id)
        {
            var tarea = ListaTareas.FirstOrDefault(x => x.Id == id);
            if (tarea != null)
            {
                EditarId = tarea.Id;
                EditarTitulo = tarea.Titulo;
                EditarDescripcion = tarea.Descripcion;
                EditarFecha = tarea.FechaVencimiento;
            }
            Tareas = FiltrarTareas(Estado);
        }

        //guardar eicion
        public IActionResult OnPostGuardarEdicion()
        {
            var tarea = ListaTareas.FirstOrDefault(x => x.Id == EditarId);
            if (tarea != null)
            {
                if (!string.IsNullOrWhiteSpace(EditarTitulo))
                    tarea.Titulo = EditarTitulo.Trim();
                tarea.Descripcion = EditarDescripcion?.Trim();
                tarea.FechaVencimiento = EditarFecha;
            }
            return RedirectToPage(new { Estado });
        }

        //filtros cambiar
        private static List<Tarea> FiltrarTareas(string? estado)
        {
            IEnumerable<Tarea> consulta = ListaTareas;
            switch (estado?.ToLowerInvariant())
            {
                case "completadas":
                    consulta = consulta.Where(x => x.EstaCompletada);
                    break;
                case "pendientes":
                    consulta = consulta.Where(x => !x.EstaCompletada);
                    break;
                default:
                    break; // termine la tarea
            }

            return consulta
                .OrderBy(x => x.EstaCompletada)
                .ThenBy(x => x.FechaVencimiento ?? DateTime.MaxValue)
                .ToList();
        }
    }
}
