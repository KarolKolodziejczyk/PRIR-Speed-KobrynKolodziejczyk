using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Speed.Backend
{
    class Game
    {
        private string IPPrzeciwnika;
        public GameNetworking network;
        public List<Karta> Talia = new List<Karta>();
        public List<Karta> RękaGracza = new List<Karta>();
        public List<Karta> RękaPrzeciwnika = new List<Karta>();

        public Game(string IPprzeciwnika) {
            this.IPPrzeciwnika = IPPrzeciwnika;
            network  =new GameNetworking(IPprzeciwnika);
        }
    }
}
