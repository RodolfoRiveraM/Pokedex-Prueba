using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Pokemon2.Models
{
    public class PokemonListResponse
    {
        [JsonPropertyName("pokemon_entries")]
        public List<PokemonEntry> ?PokemonEntries { get; set; }
    }

    public class PokemonEntry
    {
        [JsonPropertyName("entry_number")]
        public int EntryNumber { get; set; }

        [JsonPropertyName("pokemon_species")]
        public PokemonSpecies ?PokemonSpecies { get; set; }
    }

    public class PokemonSpecies
    {
        [JsonPropertyName("name")]
        public string ?Name { get; set; }
    }
}
