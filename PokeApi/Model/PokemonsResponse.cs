using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokeApi.Model
{
    public class PokemonsResponse
    {
        public int Count { get; set; }
        [JsonProperty("next")]
        public string NextUrl { get; set; } = null!;
        [JsonProperty("prev")]
        public string PrevUrl { get; set; } = null!;
        public List<PokemonResult> Results { get; set; } = null!;
    }
}
