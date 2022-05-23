using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PokeApi.Model
{
    public class Type
    {
        [JsonConstructor]
        public Type(JObject type)
        {
            Name = type["name"].ToString();
            Url = type["url"].ToString();
        }

        [JsonIgnore]
        public string Name { get; set; }
        [JsonIgnore]
        public string Url { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
