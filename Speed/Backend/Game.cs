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
        public int seed;
        public Game(string IPprzeciwnika) {
            this.IPPrzeciwnika = IPPrzeciwnika;
            network  =new GameNetworking(IPprzeciwnika);
        }
        public void Init()
        {
            StworzTalie();
            Random rng = new Random();
            seed = rng.Next();
            TasujTalie(seed);

            RozdajKarty(5);
            RozdajKartyPrzeciwnikowi(5);
            //network.SendToOpponent(Talia);

        }
        //Dodac SEED
        public void TasujTalie(int Seed)
        {
            Random rng = new Random(Seed); 
            int n = Talia.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                Karta value = Talia[k];
                Talia[k] = Talia[n];
                Talia[n] = value;
            }
        }
        public void StworzTalie()
        {
            for (int i = 2; i <= 9; i++)
            {
                // Dodaj karty od 1 do 10 dla każdego koloru
                Talia.Add(new Karta(i, $"club{i}", Color.Kier));
                Talia.Add(new Karta(i, $"diam{i}", Color.Karo));
                Talia.Add(new Karta(i, $"heart{i}", Color.Trefl));
                Talia.Add(new Karta(i, $"spade{i}", Color.Pik));
            }

            // Dodaj figury (Król, Królowa, Walet) dla każdego koloru
            Talia.Add(new Karta(11, $"heartJ", Color.Kier));
            Talia.Add(new Karta(11, $"diamJ", Color.Karo));
            Talia.Add(new Karta(11, $"heartJ", Color.Trefl));
            Talia.Add(new Karta(11, $"spadeJ", Color.Pik));

            Talia.Add(new Karta(12, $"heartQ", Color.Kier));
            Talia.Add(new Karta(12, $"diamQ", Color.Karo));
            Talia.Add(new Karta(12, $"heartQ", Color.Trefl));
            Talia.Add(new Karta(12, $"spadeQ", Color.Pik));

            Talia.Add(new Karta(13, $"heartK", Color.Kier));
            Talia.Add(new Karta(13, $"diamK", Color.Karo));
            Talia.Add(new Karta(13, $"heartK", Color.Trefl));
            Talia.Add(new Karta(13, $"spadeK", Color.Pik));
            for(int i = 0; i!=2;i++)
            {
                Talia.Add(new Karta(13, $"superFreeze", Color.Special));
                Talia.Add(new Karta(13, $"superPeek", Color.Special));
                Talia.Add(new Karta(13, $"superSwap", Color.Special));
            }
        }
        private void RozdajKarty(int liczbaKart)
        {
            for (int i = 0; i < liczbaKart; i++)
            {
                if (Talia.Count > 0) {
                    RękaGracza.Add(Talia.Last());
                    Talia.Remove(Talia.Last());
                }
            }

        }

        private void RozdajKartyPrzeciwnikowi(int liczbaKart)
        {
            for (int i = 0; i < liczbaKart; i++)
            {
                if (Talia.Count > 0)
                {
                    RękaPrzeciwnika.Add(Talia.Last());
                    Talia.Remove(Talia.Last());
                }
            }
        }
        public void RzucKarte(int numer)
        {

            RękaGracza.RemoveAt(numer-1);
            RozdajKarty(1);

        }

        public Karta LosujKarteZTalii()
        {
            if (Talia.Count == 0)
                throw new InvalidOperationException("Talia jest pusta.");

            // Losowanie indeksu karty
            Random rng = new Random();
            int index = rng.Next(Talia.Count);

            // Pobranie karty z talii i usunięcie jej
            Karta wylosowanaKarta = Talia[index];
            Talia.RemoveAt(index);

            return wylosowanaKarta;
        }
    }
}
