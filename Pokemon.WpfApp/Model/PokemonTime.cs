using PokeApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokeDex.WpfApp.Model
{
    public class PokemonTime
    {
        public PokemonTime() { }

        public PokemonTime(string name)
        {
            Name = name;
            Time = DateTime.Now;
        }

        public static implicit operator PokemonTime(Pokemon p) => new PokemonTime(p.Name);

        public string Name { get; set; }
        public DateTime Time { get; set; }
    }
}
