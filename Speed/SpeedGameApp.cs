using System;
using System.Windows;
using System.Windows.Threading;
using Speed.Backend;

namespace Speed
{
    internal class SpeedGameApp
    {
        public GameNetworking network;
        public Game game;
        public event Action<string> MessageReceived;

        public SpeedGameApp(string IP)
        {
            this.network = new GameNetworking(IP);
            this.game = new Game(IP);
            this.network.MessageReceived += OnMessageReceived;


            // Uruchom nasłuchiwanie na określonym porcie
            this.network.StartListening(7078);
        }

        public void OnMessageReceived(string message)
        {
            // Bezpieczne aktualizowanie interfejsu użytkownika z wątku innego niż główny wątek UI
            Application.Current.Dispatcher.Invoke(() =>
            {
               // MessageBox.Show("APP: "+message);
            });
            MessageReceived?.Invoke(message);

            // Aktualizacja interfejsu użytkownika na podstawie otrzymanej wiadomości
            // Tu można dodać dodatkową logikę do obsługi wiadomości
        }

        public bool OnCardChosen(int numer)
        {
            // Logika wyboru karty
            // Przykład wysłania wiadomości do przeciwnika
           
            if (this.game.LegalnyRuch(numer))
            {
     

                    network.SendToOpponent("[ZAG]?" + numer);
                    return true;
                
                }
                return false;

        }

        public void OnGameStart()
        {
            // Logika rozpoczęcia gry
            // Przykład wysłania wiadomości do przeciwnika
        }

        public void OnStop()
        {
            // Logika zatrzymania gry
            network.Stop();
        }

        public void OnSurrender()
        {
            // Logika poddania się
            // Przykład wysłania wiadomości do przeciwnika
            network.SendToOpponent("[SU]?Surrender");
            network.Stop();
        }
    }
}
