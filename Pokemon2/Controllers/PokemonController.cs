using Microsoft.AspNetCore.Mvc;
using Pokemon2.Models;
using Pokemon2.Services;

namespace Pokemon2.Controllers
{
    public class PokemonController : Controller
    {
        private readonly IPokeService _pokeService;
        private readonly IEmailService _emailService;
        private const int PageSize = 10;

        public PokemonController(IPokeService pokeService, IEmailService emailService)
        {
            _pokeService = pokeService;
            _emailService = emailService;
        }

        //muestra el detalle del Pokemon tomando de pokemon-species/{id}
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var pokemon = await _pokeService.GetPokemonDetailAsync(id);

                if (pokemon == null)
                    return RedirectToAction("Error", "Home");

                // Prueba
                Console.WriteLine($"ID: {pokemon.Id}");
                Console.WriteLine($"Nombre: {pokemon.Name}");
                Console.WriteLine($"Color: {pokemon.Color}");

                return View(pokemon);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en Details: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        // Envío de correo con los datos del Pokémon
        [HttpPost]
        public async Task<IActionResult> SendPokemonEmail(string email, int id, string? search, string? habitat)
        {
            try
            {
                var pokemons = await _pokeService.GetPokemonsAsync();

                if (!string.IsNullOrEmpty(search))
                    pokemons = pokemons.Where(p => p.Name != null && p.Name.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

                // crear cuerpo en html
                string body = "<h3>Pokémon:</h3><ul>";
                foreach (var p in pokemons)
                {
                    body += $"<li>Id: {id} Nombre: {p.Name} Hábitat: {habitat} <img src='{p.ImageUrl}' width='50'/></li>";
                }
                body += "</ul>";

                await _emailService.SendEmailAsync(email, "Pokémons filtrados", body);

                return RedirectToAction("Details", new { id });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en SendPokemonEmail: {ex.Message}");
                return RedirectToAction("Details", new { id });
            }
        }

    }
}
