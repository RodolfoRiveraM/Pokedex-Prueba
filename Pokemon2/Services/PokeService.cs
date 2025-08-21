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

        // Obtener todos los Pokémon nacionales usando la API de https://pokeapi.co/api
        public async Task<List<PokemonListItem>> GetPokemonsAsync()
        {
            var url = "https://pokeapi.co/api/v2/pokedex/1";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error al obtener datos de la API: {StatusCode}", response.StatusCode);
                return new List<PokemonListItem>();
            }

            var content = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<PokemonListResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            Console.WriteLine("data");
            Console.WriteLine(data);
            Console.WriteLine("data");

            var list = data?.PokemonEntries.Select(p => new PokemonListItem
            {
                Id = p.EntryNumber,
                Name = p.PokemonSpecies.Name,
                ImageUrl = $"https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/{p.EntryNumber}.png"
            }).ToList();

            return list ?? new List<PokemonListItem>();
        }


        // Obtener detalle del Pokémon con la api https://pokeapi.co/api/v2/pokemon-species/{id}
        public async Task<PokemonDetail?> GetPokemonDetailAsync(int id)
        {
            var url = $"https://pokeapi.co/api/v2/pokemon-species/{id}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error al obtener detalle del Pokémon {Id}: {StatusCode}", id, response.StatusCode);
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(content);
            var root = doc.RootElement;

            var pokemon = new PokemonDetail
            {
                Id = root.GetProperty("id").GetInt32(),
                Name = root.GetProperty("name").GetString() ?? "",
                Color = root.GetProperty("color").GetProperty("name").GetString() ?? "",
                Habitat = root.TryGetProperty("habitat", out var habitatEl) && habitatEl.ValueKind != JsonValueKind.Null
                          ? habitatEl.GetProperty("name").GetString() ?? ""
                          : "Desconocido",
                Generation = root.GetProperty("generation").GetProperty("name").GetString() ?? "",
                EggGroups = new List<string>()
            };

            if (root.TryGetProperty("egg_groups", out var eggsEl))
            {
                foreach (var egg in eggsEl.EnumerateArray())
                {
                    pokemon.EggGroups.Add(egg.GetProperty("name").GetString() ?? "");
                }
            }

            return pokemon;
        }


        // Obtener todas las especies de Pokémon usando https://pokeapi.co/api/v2/pokemon-species de la API de Pokémon.
        public async Task<List<EspeciesListItem>> GetAllSpeciesAsync()
        {
            var url = "https://pokeapi.co/api/v2/pokemon-species?limit=2000";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error al obtener tipos de Pokémon: {StatusCode}", response.StatusCode);
                return new List<EspeciesListItem>();
            }

            var content = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<TypeResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (data?.Results == null)
                return new List<EspeciesListItem>();

            return data.Results.Select(t => new EspeciesListItem { Name = t.Name }).ToList();
        }


    }
}
