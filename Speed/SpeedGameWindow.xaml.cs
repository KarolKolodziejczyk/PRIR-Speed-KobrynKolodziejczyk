using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using Speed.Backend;

namespace Speed
{
    public partial class SpeedGameWindow : Window
    {
        private SpeedGameApp APP;
        private int HoverPixelAmount { get; } = 20;
        private bool CzyHost = false;
        private List<Button> playerCardButtons;
        private bool CzyFreeze = false;
        private bool CzyPeek = false;

        public SpeedGameWindow(string IP, bool s)
        {

            playerCardButtons = new List<Button> { BtnPlayerCard1, BtnPlayerCard2, BtnPlayerCard3, BtnPlayerCard4, BtnPlayerCard5 };

            InitializeComponent();
            //Aktualizuj("1");
            CzyHost = s;
            APP = new SpeedGameApp(IP);
            if (CzyHost)
            {
                // Aktualizuj("JESTEM HOSTEM");
                APP.game.Init();
                APP.network.SendToOpponent("[SG]?" + this.APP.game.seed);
                //for (int i = 0; i != 5; i++)
                //    APP.network.SendToOpponent("[TG]?" + this.APP.game.RękaPrzeciwnika[i].SerializeToJson());


                aktualizujKarty();
            }
            // Aktualizuj(this.APP.game.RękaGracza[0].ImagePath);

            LblEnemyCard1.Content = LoadCardImage("reverse");
            LblEnemyCard2.Content = LoadCardImage("reverse");
            LblEnemyCard3.Content = LoadCardImage("reverse");
            LblEnemyCard4.Content = LoadCardImage("reverse");
            LblEnemyCard5.Content = LoadCardImage("reverse");
            LblEnemy.Content = LoadCardImage("opponent");

            LblDeck1.Content = LoadCardImage("reverse");
            LblDeck2.Content = LoadCardImage("reverse");
            LblTableChangeTop.Content = LoadCardImage("tableChangeIcon");
            LblTableChangeBottom.Content = LoadCardImage("tableChangeIcon");
            LblTableChangeTop.Visibility = Visibility.Hidden;
            LblTableChangeBottom.Visibility = Visibility.Hidden;

            //LblTableCard.Content = LoadCardImage("noCard");
            LblTableCard.Content = LoadCardImage("diam6");

            // UI info setup
            TaliaCount(40);
            SetPlayerPoints(0);
            SetEnemyPoints(0);
            this.APP.network.MessageReceived += OnMessageReceived; // Subskrybuj zdarzenie odbioru wiadomości
        }

        private Image LoadCardImage(string cardSymbol, string cardNumber = "")
        {
            Image image = new Image();
            var uri = new Uri($"pack://application:,,,/cards/{cardSymbol.ToLower()}{cardNumber}.png");
            var bitmap = new BitmapImage(uri);
            image.Source = bitmap;
            return image;
        }
        private void SetPlayerPoints(int count)
        {
            LblPlayerPoints.Content = "Points: " + count;
        }
        private void TaliaCount(int count)
        {
            LblDeckRemaining.Content = "x"+count;
            SetPlayerPoints(this.APP.game.PunktyGracz);
            SetEnemyPoints(this.APP.game.PunktyPrzeciwnik);
            AktualizujCzyLocked();

        }

        private void SetEnemyPoints(int count)
        {
            LblEnemyPoints.Content = "Enemy Points: " + count;
        }
        private async void BtnTest_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Wysylam");
            await APP.network.SendToOpponent("Wiadomość: hehe");
        }

        private void MoveButton(object sender, int pixelAmount)
        {
            if (sender is Button button)
            {
                button.Margin = new Thickness(button.Margin.Left, button.Margin.Top - pixelAmount, button.Margin.Right, button.Margin.Bottom + pixelAmount);
            }
        }

        private void BtnPlayerCard_MouseEnter(object sender, MouseEventArgs e)
        {
            MoveButton(sender, HoverPixelAmount);

        }

        private void BtnPlayerCard_MouseLeave(object sender, MouseEventArgs e)
        {
            MoveButton(sender, -HoverPixelAmount);

        }
        private async Task Freeze()
        {
            CzyFreeze = true;
            await Dispatcher.InvokeAsync(() =>
            {
                Background = new SolidColorBrush(Colors.LightBlue);
            });

            await Task.Delay(10000); // Oczekuje 10 sekund asynchronicznie

            await Dispatcher.InvokeAsync(() =>
            {
                WyłączMrożenie();
                string hexColor = "#FFE0FFA0";
                Background = new SolidColorBrush((System.Windows.Media.Color)ColorConverter.ConvertFromString(hexColor));
            });
        }
        
        private void PeekAktualizuj()
        {
            var game = this.APP.game;

            LblEnemyCard1.Content = LoadCardImage(game.RękaPrzeciwnika[0].ImagePath);
            LblEnemyCard2.Content = LoadCardImage(game.RękaPrzeciwnika[1].ImagePath);
            LblEnemyCard3.Content = LoadCardImage(game.RękaPrzeciwnika[2].ImagePath);
            LblEnemyCard4.Content = LoadCardImage(game.RękaPrzeciwnika[3].ImagePath);
            LblEnemyCard5.Content = LoadCardImage(game.RękaPrzeciwnika[4].ImagePath);
        }
        private async Task Peek()
        {
            this.CzyPeek = true;
            var game = this.APP.game;
            await Dispatcher.InvokeAsync(() =>
            {
                LblEnemyCard1.Content = LoadCardImage(game.RękaPrzeciwnika[0].ImagePath);
                LblEnemyCard2.Content = LoadCardImage(game.RękaPrzeciwnika[1].ImagePath);
                LblEnemyCard3.Content = LoadCardImage(game.RękaPrzeciwnika[2].ImagePath);
                LblEnemyCard4.Content = LoadCardImage(game.RękaPrzeciwnika[3].ImagePath);
                LblEnemyCard5.Content = LoadCardImage(game.RękaPrzeciwnika[4].ImagePath);
            });
            await Task.Delay(10000); // Oczekuje 10 sekund asynchronicznie
            this.CzyPeek = false;
            await Dispatcher.InvokeAsync(() =>
            {
                LblEnemyCard1.Content = LoadCardImage("reverse");
                LblEnemyCard2.Content = LoadCardImage("reverse");
                LblEnemyCard3.Content = LoadCardImage("reverse");
                LblEnemyCard4.Content = LoadCardImage("reverse");
                LblEnemyCard5.Content = LoadCardImage("reverse");
            });
        }
        private  DateTime ConvertStringToDateTime(string dateString)
        {
            return DateTime.ParseExact(
                dateString,
                "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                CultureInfo.InvariantCulture.DateTimeFormat,
                DateTimeStyles.AssumeUniversal
            );
        }
 

        private void WyłączMrożenie()
        {
            CzyFreeze = false;
        }
        private void OnMessageReceived(string message)
        {


            string[] parts = message.Split(new string[] { "?" }, StringSplitOptions.None);
            if (parts.Length == 2)
            {
                string command = parts[0];
                string parameter = parts[1];

                switch (command)
                {
                    case "[FRIZ]":
                        Application.Current.Dispatcher.Invoke((Action)delegate
                        {
                           // MessageBox.Show("sUPEFRIZZ");
                            Freeze();
                        });
                    break;
                    case "[SWAP]":
                        Application.Current.Dispatcher.Invoke((Action)delegate
                        {
                            Swap();
                            aktualizujKarty();
                        });
                     break;
                    case "[KG]":
                        Application.Current.Dispatcher.Invoke((Action)delegate
                        {
                            KoniecGry(Convert.ToBoolean(parameter));
                        });
                        break;

                    case "[UL]":
                        Application.Current.Dispatcher.Invoke((Action)delegate
                        {
                            EnemyLocked.Content = "";
                        });
                        break;

                    case "[L]":
                        Application.Current.Dispatcher.Invoke((Action)delegate
                        {
                            EnemyLocked.Content = "🔒";
                            if (GraczLocked.Content.ToString() == "🔒" && EnemyLocked.Content.ToString() == "🔒") KoniecGry();
                        });
                        break;

                    case "[ZAG]":
                        // Tu wysylamy co sie dzieje
                        // var Czas = ConvertStringToDateTime(parts[1]);
                        Application.Current.Dispatcher.Invoke((Action)delegate
                        {
                            // MessageBox.Show("ODEBRALEM" + message);
                            ObierzKarte(Convert.ToInt32(parameter));
                        });
                        // if (Czas > this.APP.game.Czas) this.APP.network.SendToOpponent("[Y]");
                        // else this.APP.network.SendToOpponent("[N]");
                        break;

                    case "[SG]":
                        this.APP.game.StworzTalie();
                        int liczba = Convert.ToInt32(parameter);
                        this.APP.game.TasujTalie(liczba);
                        // this.APP.game.Talia.RemoveRange(this.APP.game.Talia.Count - 11, 10);
                        Application.Current.Dispatcher.Invoke((Action)delegate
                        {
                            this.APP.game.RozdajKartyPrzeciwnikowi(5);
                            this.APP.game.RozdajKarty(5);
                            aktualizujKarty();
                        });
                        break;
                  
                    case "[TG]":
                        // Rekonstrukcja tali na nowo
                        try
                        {
                            Application.Current.Dispatcher.Invoke((Action)delegate
                            {
                                Karta karta = new Karta(parameter);
                                // MessageBox.Show($"Otrzymano wiadomość w Window: {karta.SerializeToJson()}");
                                this.APP.game.RękaGracza.Add(karta);

                                if (this.APP.game.RękaGracza.Count == 5)
                                    aktualizujKarty();
                                // MessageBox.Show("Talia otrzymana i zrekonstruowana na nowo.");
                            });
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Błąd podczas deserializacji tali: {ex.Message}");
                        }
                        break;

                    default:
                        MessageBox.Show($"Otrzymano wiadomość w Window: {message}");
                        break;
                }
            }
        }
        public void AktualizujCzyLocked()
        {
            this.APP.network.SendToOpponent(this.APP.game.CzyLocked ? "[L]?1" : "[UL]?1");
            GraczLocked.Content = this.APP.game.CzyLocked ? "🔒" : "";
            if (GraczLocked.Content.ToString() == "🔒" && EnemyLocked.Content.ToString() == "🔒") KoniecGry();
        }

        private void KoniecGry(bool surrender = false) {
            if (!this.APP.game.Koniec)
            {
                this.APP.game.Koniec = true;
                this.APP.network.SendToOpponent("[KG]?"+surrender);
                ResultWindow info;
                if (!surrender)
                {
                    if (this.APP.game.PunktyGracz > this.APP.game.PunktyPrzeciwnik)
                        info = new ResultWindow(ResultType.Win);
                    else
                        info = new ResultWindow(ResultType.Failure);
                }
                else
                    info = new ResultWindow(ResultType.Surrender);

                info.Owner = this;
                info.ShowDialog();
                this.Close();
                Application.Current.Shutdown();
            }
        }
        private void Swap()
        {
            var tk = this.APP.game.RękaGracza;
            this.APP.game.RękaGracza = this.APP.game.RękaPrzeciwnika;
            this.APP.game.RękaPrzeciwnika = tk;
            if (CzyPeek) PeekAktualizuj();

        }
        private void aktualizujKarty()
        {
            var game = this.APP.game;

            BtnPlayerCard1.Content = game.RękaGracza[0] != null ? LoadCardImage(game.RękaGracza[0].ImagePath) : LoadCardImage("reverse");
            BtnPlayerCard2.Content = game.RękaGracza[1] != null ? LoadCardImage(game.RękaGracza[1].ImagePath) : LoadCardImage("reverse");
            BtnPlayerCard3.Content = game.RękaGracza[2] != null ? LoadCardImage(game.RękaGracza[2].ImagePath) : LoadCardImage("reverse");
            BtnPlayerCard4.Content = game.RękaGracza[3] != null ? LoadCardImage(game.RękaGracza[3].ImagePath) : LoadCardImage("reverse");
            BtnPlayerCard5.Content = game.RękaGracza[4] != null ? LoadCardImage(game.RękaGracza[4].ImagePath) : LoadCardImage("reverse");
            AktualizujCzyLocked();
        }
        private void RzucKarteFront(int numer)
        {
            bool czySwap = false;

            if (!CzyFreeze)
            {
                if (this.APP.OnCardChosen(numer))
                {
                    TableChangeEffect();
                    this.TestowyLabel.Content = this.APP.game.RękaGracza[numer - 1].SerializeToJson();
                    LblTableCard.Content = LoadCardImage(this.APP.game.RękaGracza[numer - 1].ImagePath);
                    if (this.APP.game.RękaGracza[numer - 1].Value == 13)
                        this.APP.network.SendToOpponent("[FRIZ]?1");
                    if (this.APP.game.RękaGracza[numer - 1].Value == 14)
                        this.Peek();
                    if (this.APP.game.RękaGracza[numer - 1].Value == 15)
                    {
                        this.APP.network.SendToOpponent("[SWAP]?1");
                        czySwap = true;
                    }
                    this.APP.game.RzucKarte(numer);
                    if (czySwap) Swap();
                    this.TaliaCount(this.APP.game.Talia.Count());
                    aktualizujKarty();

                }
            }
        }
        private void ObierzKarte(int numer)
        {
                TableChangeEffect();
                //MessageBox.Show("Zagrywam" + this.APP.game.RękaPrzeciwnika[numer - 1].ImagePath);
                LblTableCard.Content = LoadCardImage(this.APP.game.RękaPrzeciwnika[numer - 1].ImagePath);
                this.TestowyLabel.Content = this.APP.game.RękaPrzeciwnika[numer - 1].SerializeToJson();
                this.APP.game.RzucKartePrzeciwnik(numer);
                this.TaliaCount(this.APP.game.Talia.Count());
                if (CzyPeek) PeekAktualizuj();

        }
        private void BtnPlayerCard1_Click(object sender, RoutedEventArgs e)
        {
            // Obsługa kliknięcia karty gracza
            RzucKarteFront(1);
        }
        private void BtnPlayerCard2_Click(object sender, RoutedEventArgs e)
        {
            RzucKarteFront(2);

        }

        private void BtnPlayerCard3_Click(object sender, RoutedEventArgs e)
        {
            RzucKarteFront(3);

        }
        private void BtnPlayerCard4_Click(object sender, RoutedEventArgs e)
        {
            RzucKarteFront(4);

        }
        private void BtnPlayerCard5_Click(object sender, RoutedEventArgs e)
        {
            RzucKarteFront(5);
        }

        private void BtnSurrender_Click(object sender, RoutedEventArgs e)
        {
            // OnPlayerSurrender -> safe delete + thread break

            ResultWindow info = new ResultWindow(ResultType.Surrender);
            info.Owner = this;
            info.ShowDialog();
            KoniecGry(true);
            APP.OnSurrender();

            this.Close();
        }
        private void AnimateTableThread(object obj)
        {
            if (obj is Label label)
            {
                label.Dispatcher.Invoke(() => label.Visibility = Visibility.Visible);
                Thread.Sleep(1000);
                label.Dispatcher.Invoke(() => label.Visibility = Visibility.Hidden);
            }
        }

        private void TableChangeEffect()
        {
            Thread t1 = new Thread(AnimateTableThread);
            Thread t2 = new Thread(AnimateTableThread);
            t1.Start(LblTableChangeTop);
            t2.Start(LblTableChangeBottom);
        }

    }
}
