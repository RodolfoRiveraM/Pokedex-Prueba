using Pokemon2.Models;
using System.Collections.Generic;

namespace Pokemon2.Models
{
    public class PagedPokemonViewModel
    {
        public List<PokemonListItem> Pokemons { get; set; } = new();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
