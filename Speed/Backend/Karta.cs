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
        Kier,
        Karo,
        Trefl,
        Pik,
        Special
           
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
