using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        int value;
        string imagePath;
        Color color;
        public Karta(int value, string imagePath, Color color)
        {
            this.value = value;
            this.imagePath = imagePath;
            this.color = color;
        }


    }
}
