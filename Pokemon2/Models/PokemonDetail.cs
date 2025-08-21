using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Pokemon2.Models
{
    public class PokemonDetail
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string ?Name { get; set; }

        [JsonPropertyName("color")]
        public string ?Color { get; set; }

        [JsonPropertyName("habitat")]
        public string ?Habitat { get; set; }

        [JsonPropertyName("generation")]
        public string ?Generation { get; set; }

        [JsonPropertyName("egg_groups")]
        public List<string> ?EggGroups { get; set; }
    }
}
