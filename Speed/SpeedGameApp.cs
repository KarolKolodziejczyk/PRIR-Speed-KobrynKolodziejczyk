using System.Windows;
using Speed.Backend;

namespace Speed
{
    internal class SpeedGameApp
    {
        private SpeedGameWindow GUI;
        public GameNetworking network;
        public Game game;
        public event Action<string> MessageReceived;

        public SpeedGameApp(SpeedGameWindow gameWindow, string IP)
        {
            this.GUI = gameWindow;
            this.network = new GameNetworking(IP);
            this.game = new Game(IP);
            this.network.MessageReceived += OnMessageReceived; 
        }

        public void OnMessageReceived(string message)
        {
            MessageReceived?.Invoke(message);
            // Aktualizacja interfejsu użytkownika na podstawie otrzymanej wiadomości
            GUI.Dispatcher.Invoke(() =>
            {
                MessageBox.Show(message);
            });
        }

        public void OnCardChosen()
        {
            // Logika wyboru karty
        }

        public void OnGameStart()
        {
            // Logika rozpoczęcia gry
        }

        public void OnStop()
        {
            // Logika zatrzymania gry
        }

        public void OnSurrender()
        {
            // Logika poddania się
        }
    }
}
