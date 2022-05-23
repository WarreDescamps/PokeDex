using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PokeApi.Model;
using System.Net.Http;

namespace PokeApi
{
    public class PokeApiClient : IDisposable
    {
        private const string PokeApiUrl = "https://pokeapi.co/api/v2/pokemon";
        private readonly HttpClient _httpClient;

        public PokeApiClient()
        {
            _httpClient = new HttpClient();
        }

        private async Task<T> GetAsync<T>(string url) where T : class
        {
            var response = await _httpClient.GetStringAsync(url);
            if (string.IsNullOrEmpty(response))
                throw new PokeApiException("Request didn't return anything");
            return JsonConvert.DeserializeObject<T>(response) ?? throw new PokeApiException("Something went wrong");
        }

        public async Task<PokemonsResponse> GetPokemonAsync(int limit = 151)
        {
            return await GetAsync<PokemonsResponse>(PokeApiUrl + $"?limit={limit}&offset=0");
        }

        public async Task<Pokemon> GetPokemonAsync(string name)
        {
            var pokemon = await GetAsync<Pokemon>(PokeApiUrl + "/" + name);
            var species = await GetAsync<JObject>(PokeApiUrl + "-species/" + pokemon.Id);
            pokemon.FlavorText = species["flavor_text_entries"]?
                .First(jt => jt?["language"]?["name"]?.ToString() == "en" || jt?["version"]?["name"]?.ToString() == "red")?["flavor_text"]?
                .ToString().Replace("\n", " ").Replace("\f", " ") ?? "";
            pokemon.Genus = species["genera"]?
                .First(jt => jt?["language"]?["name"]?.ToString() == "en")?["genus"]?.ToString();
            return pokemon;
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}