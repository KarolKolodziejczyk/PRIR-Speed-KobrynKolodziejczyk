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
        public Karta KartaNaStole =new Karta(6, $"diam{6}", Color.Karo);
        public int seed;
        public bool CzyLocked = false;
        public DateTime Czas = new DateTime(0);
        public int PunktyPrzeciwnik = 0;
        public int PunktyGracz = 0;
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
        public bool LegalnyRuch(int numer)
        {
            var KartaGracza = RękaGracza[numer - 1];
            return ((Math.Abs(KartaNaStole.Value - KartaGracza.Value) <= 1) || KartaNaStole.Color == KartaGracza.Color || KartaGracza.Color == Color.Special || KartaNaStole.Color==Color.Special) && KartaGracza.Color!=Color.None; 
        }
        //Dodac SEED
        public void SprawdzLock()
        {
            for (int i = 0; i != 5; i++)
                if (LegalnyRuch(i)) return;
            CzyLocked= true;
        }
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
            Talia.Add(new Karta(10, $"clubJ", Color.Kier));
            Talia.Add(new Karta(10, $"diamJ", Color.Karo));
            Talia.Add(new Karta(10, $"heartJ", Color.Trefl));
            Talia.Add(new Karta(10, $"spadeJ", Color.Pik));

            Talia.Add(new Karta(11, $"clubQ", Color.Kier));
            Talia.Add(new Karta(11, $"diamQ", Color.Karo));
            Talia.Add(new Karta(11, $"heartQ", Color.Trefl));
            Talia.Add(new Karta(11, $"spadeQ", Color.Pik));

            Talia.Add(new Karta(12, $"clubK", Color.Kier));
            Talia.Add(new Karta(12, $"diamK", Color.Karo));
            Talia.Add(new Karta(12, $"heartK", Color.Trefl));
            Talia.Add(new Karta(12, $"spadeK", Color.Pik));
            for(int i = 0; i!=2;i++)
            {
                Talia.Add(new Karta(13, $"superFreeze", Color.Special));
                Talia.Add(new Karta(13, $"superPeek", Color.Special));
                Talia.Add(new Karta(13, $"superSwap", Color.Special));
            }
        }
        public void RozdajKarty(int liczbaKart)
        {
            for (int i = 0; i < liczbaKart; i++)
            {
                if (Talia.Count >= 0)
                {
                    RękaGracza.Add(Talia.Last());
                    Talia.Remove(Talia.Last());
                }
                else
                    RękaGracza.Add(new Karta(-5, $"reverse", Color.None));
            }

        }

        public void RozdajKartyPrzeciwnikowi(int liczbaKart)
        {
            for (int i = 0; i < liczbaKart; i++)
            {
                if (Talia.Count >= 0)
                {
                    RękaPrzeciwnika.Add(Talia.Last());
                    Talia.Remove(Talia.Last());
                }
           
            }
        }
        public void RzucKarte(int numer)
        {
            KartaNaStole = RękaGracza[numer - 1];
            this.PunktyGracz += RękaGracza[numer - 1].Value;
            RękaGracza.RemoveAt(numer-1);
            if (Talia.Count > 0)
            RozdajKarty(1);
            else
                RękaGracza.Add(new Karta(-5, $"reverse", Color.None));
        }
        public void RzucKartePrzeciwnik(int numer)
        {
            KartaNaStole = RękaPrzeciwnika[numer - 1];
            this.PunktyPrzeciwnik += RękaPrzeciwnika[numer - 1].Value;
            RękaPrzeciwnika.RemoveAt(numer - 1);
            if(Talia.Count>0)
            RozdajKartyPrzeciwnikowi(1);
            else
                RękaPrzeciwnika.Add(new Karta(-5, $"reverse", Color.None));
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
