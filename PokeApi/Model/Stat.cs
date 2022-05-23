using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PokeApi.Model
{
    public class Stat
    {
        [JsonConstructor]
        public Stat(JObject stat)
        {
            Name = stat["name"].ToString();
            Url = stat["url"].ToString();
        }

        [JsonIgnore]
        public string Name { get; set; }
        [JsonIgnore]
        public string Url { get; set; }
        [JsonProperty("base_stat")]
        public int Base { get; set; }
        public int Effort { get; set; }

        public override string ToString()
        {
            return $"{Name}: {Base}";
        }
    }
}
