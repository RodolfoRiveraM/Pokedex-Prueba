using Microsoft.AspNetCore.Mvc;
using Pokemon2.Models;
using Pokemon2.Services;
using ClosedXML.Excel;
using System.Text.Json;

// Controlador que gestiona los Pokémon: los muestra con filtros y paginación, permite exportarlos a Excel y enviarlos por correo electrónico.
namespace Pokemon2.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPokeService _pokeService;
        private readonly IEmailService _emailService;
        private const int PageSize = 12;

        public HomeController(IPokeService pokeService, IEmailService emailService)
        {
            _pokeService = pokeService;
            _emailService = emailService;
        }

        //Muestra el listado de los Pokémon con sus filtros
        public async Task<IActionResult> Index(string? search, string? species, int page = 1)
        {
            try
            {
                var pokemons = await _pokeService.GetPokemonsAsync();

                if (pokemons == null || pokemons.Count == 0)
                    return View(new PagedPokemonViewModel());

                if (!string.IsNullOrEmpty(search))
                    pokemons = pokemons.Where(p => p.Name != null && p.Name.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

                if (!string.IsNullOrEmpty(species))
                    pokemons = pokemons.Where(p => p.Name != null && p.Name.Equals(species, StringComparison.OrdinalIgnoreCase)).ToList();

                var totalItems = pokemons.Count;
                var items = pokemons.Skip((page - 1) * PageSize).Take(PageSize).ToList();

                var allSpecies = await _pokeService.GetAllSpeciesAsync();
                Console.WriteLine(JsonSerializer.Serialize(allSpecies));
                ViewData["AllSpecies"] = allSpecies;

                var model = new PagedPokemonViewModel
                {
                    Pokemons = items,
                    CurrentPage = page,
                    TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize),
                    Search = search,
                    Species = species
                };

                return View(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en Index: {ex.Message}");
                return RedirectToAction("NotFound");
            }
        }

        //Exporta a excel con el filtro
        public async Task<IActionResult> ExportExcel(string? search, string? species)
        {
            try
            {
                var pokemons = await _pokeService.GetPokemonsAsync();

                if (!string.IsNullOrEmpty(search))
                    pokemons = pokemons.Where(p => p.Name != null && p.Name.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

                if (!string.IsNullOrEmpty(species))
                    pokemons = pokemons.Where(p => p.Name != null && p.Name.Equals(species, StringComparison.OrdinalIgnoreCase)).ToList();

                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Pokemons");

                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "Nombre";

                for (int i = 0; i < pokemons.Count; i++)
                {
                    var p = pokemons[i];
                    worksheet.Cell(i + 2, 1).Value = p.Id;
                    worksheet.Cell(i + 2, 2).Value = p.Name;
                }

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;

                string fileName = $"Pokemon_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(stream.ToArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ExportExcel: {ex.Message}");
                return RedirectToAction("Index");
            }
        }

        //Envio de correo con el filtro
        [HttpPost]
        public async Task<IActionResult> SendPokemonEmail(string email, string? search, string? species)
        {
            try
            {
                var pokemons = await _pokeService.GetPokemonsAsync();

                if (!string.IsNullOrEmpty(search))
                    pokemons = pokemons.Where(p => p.Name != null && p.Name.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

                if (!string.IsNullOrEmpty(species))
                    pokemons = pokemons.Where(p => p.Name != null && p.Name.Equals(species, StringComparison.OrdinalIgnoreCase)).ToList();

                // crear cuerpo en html
                string body = "<h3>Listado de Pokémon filtrados:</h3><ul>";
                foreach (var p in pokemons)
                {
                    body += $"<li>{p.Name} /*<img src='{p.ImageUrl}' width='50'/>*/</li>";
                }
                body += "</ul>";

                await _emailService.SendEmailAsync(email, "Pokémons filtrados", body);

                TempData["Message"] = "Correo enviado correctamente!";
                return RedirectToAction("Index", new { search, species });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en SendPokemonEmail: {ex.Message}");
                return RedirectToAction("Index", new { search, species });
            }
        }

        //Página no encontrada
        public IActionResult NotFound()
        {
            return View();
        }

    }
}
