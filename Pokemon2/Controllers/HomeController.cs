using Microsoft.AspNetCore.Mvc;
using Pokemon2.Models;
using Pokemon2.Services;
using System.Threading.Tasks;

namespace Pokemon2.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPokeService _pokeService;
        private const int PageSize = 10; // pokemones por página

        public HomeController(IPokeService pokeService)
        {
            _pokeService = pokeService;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var pokemons = await _pokeService.GetPokemonsAsync();

            if (pokemons == null || pokemons.Count == 0)
                return View(new PagedPokemonViewModel());

            var totalItems = pokemons.Count;
            var items = pokemons
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            var model = new PagedPokemonViewModel
            {
                Pokemons = items,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize)
            };

            return View(model);
        }
    }
}
