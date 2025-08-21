using Pokemon2.Models;
using System.Net.Http;
using System.Text.Json;

namespace Pokemon2.Services
{
    public class PokeService : IPokeService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PokeService> _logger;

        public PokeService(HttpClient httpClient, ILogger<PokeService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<PokemonListItem>> GetPokemonsAsync()
        {
            var url = "https://pokeapi.co/api/v2/pokedex/2/";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error al obtener datos de la API: {StatusCode}", response.StatusCode);
                return new List<PokemonListItem>();
            }

            var content = await response.Content.ReadAsStringAsync();

            // Logear JSON en consola
            _logger.LogInformation("Respuesta de la API: {Content}", content);

            var data = JsonSerializer.Deserialize<PokemonListResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var list = data?.PokemonEntries.Select(p => new PokemonListItem
            {
                Name = p.PokemonSpecies.Name,
                ImageUrl = $"https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/{p.EntryNumber}.png"
            }).ToList();

            return list ?? new List<PokemonListItem>();
        }
    }
}
