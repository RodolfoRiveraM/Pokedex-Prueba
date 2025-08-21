using Pokemon2.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pokemon2.Services
{
    public interface IPokeService
    {
        Task<List<PokemonListItem>> GetPokemonsAsync();
    }
}
