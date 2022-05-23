using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PokeApi.Model
{
    public class Ability
    {
        [JsonConstructor]
        public Ability(JObject ability)
        {
            Name = ability["name"].ToString();
            Url = ability["url"].ToString();
        }

        [JsonIgnore]
        public string Name { get; set; }
        [JsonProperty("is_hidden")]
        public bool IsHidden { get; set; }
        [JsonIgnore]
        public string Url { get; set; }

        public override string ToString()
        {
            return Name + (IsHidden ? "(hidden)": "");
        }
    }
}