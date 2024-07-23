using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Speed.Backend
{
    enum Color
    {
        Kier = 0,
        Karo = 1,
        Trefl = 2,
        Pik = 3,
        Special = 4,
        None = 10
       
    }

    class Karta
    {
        public int Value { get; set; }
        public string ImagePath { get; set; }
        public Color Color { get; set; }
        public Karta(int value, string imagePath, Color color)
        {
            this.Value = value;
            this.ImagePath = imagePath;
            this.Color = color;
        }
        public string SerializeToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
        [JsonConstructor]
        public Karta(string json)
        {
            JsonConvert.PopulateObject(json, this);
        }



    }
}
