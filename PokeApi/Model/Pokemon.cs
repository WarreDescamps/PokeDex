using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Drawing;
using System.IO;
using System.Net;

namespace PokeApi.Model
{
    public class Pokemon
    {
        [JsonIgnore]
        private Bitmap? _image = null;
        [JsonIgnore]
        private Bitmap? _silhouette = null;
        [JsonIgnore]
        private string _imageUrl;

        [JsonConstructor]
        public Pokemon(JObject sprites, int weight, int height)
        {
            _imageUrl = sprites["other"]["official-artwork"]["front_default"].ToString();

            Weight = weight / 10D + "kg";
            Height = height / 10D + "m";
        }

        public int Id { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public string Weight { get; set; }
        [JsonIgnore]
        public string Height { get; set; }
        [JsonIgnore]
        public string FlavorText { get; set; }
        [JsonIgnore]
        public string Genus { get; set; }
        [JsonIgnore]
        public Bitmap Image
        {
            get
            {
                if (_image != null)
                    return _image;

                string fileLocation = $"./Cache/{Id}.png";
                if (File.Exists(fileLocation))
                    return _image = new Bitmap(fileLocation);
                
                using (WebClient wc = new WebClient())
                {
                    using (Stream s = wc.OpenRead(_imageUrl))
                    {
                        _image = new Bitmap(s);
                        Directory.CreateDirectory("./Cache/");
                        _image.Save(fileLocation);
                        return _image;
                    }
                }
            }
        }
        [JsonIgnore]
        public Bitmap Silhouette
        {
            get
            {
                if (_silhouette != null)
                    return _silhouette;

                string fileLocation = $"./Cache/{Id}-blank.png";
                if (File.Exists(fileLocation))
                    return _silhouette = new Bitmap(fileLocation);

                Bitmap bitmap = new Bitmap(Image);
                for (int x = 0; x < bitmap.Width; x++)
                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        if (bitmap.GetPixel(x, y).A != byte.MinValue)
                        {
                             bitmap.SetPixel(x, y, Color.FromArgb(255, 0, 0, 0));
                        }
                    }
                bitmap.Save(fileLocation);
                return _silhouette = bitmap;
            }
        }
        public List<Stat> Stats { get; set; }
        public List<Type> Types { get; set; }
        public List<Ability> Abilities { get; set; }
    }
}
