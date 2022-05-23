using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokeApi.Model
{
    public class PokeApiException : Exception
    {
        public PokeApiException(string message) : base(message)
        {

        }
    }
}
