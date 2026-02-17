using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace SlotMachine
{
    public partial class MainWindow : Window
    {
        private string[] obrazy =
        {
            "Images/apple.png",      
            "Images/bar.png",        
            "Images/seven.png",      
            "Images/watermelon.png", 
            "Images/coin.png",       
            "Images/cherry.png"      
        };

        private Random los = new Random();
        private DispatcherTimer zegar;

        private int balans = 100;
        private int ostatni1, ostatni2, ostatni3;
        private bool czyKreciSie = false;

        public MainWindow()
        {
            InitializeComponent();
            AktualizujBalans();
            KomunikatText.Text = "Witaj w kasynie";
        }

        private async void Spin_Click(object sender, RoutedEventArgs e)
        {
            if (czyKreciSie) return;

            if (!int.TryParse(StawkaBox.Text, out int stawka) || stawka <= 0)
            {
                KomunikatText.Text = "Podaj poprawną stawkę!";
                return;
            }

            if (balans < stawka)
            {
                KomunikatText.Text = "Brak środków!";
                return;
            }

            czyKreciSie = true;
            PrzyciskZakrec.IsEnabled = false;

            balans -= stawka;
            AktualizujBalans();
            KomunikatText.Text = "";

            zegar = new DispatcherTimer();
            zegar.Interval = TimeSpan.FromMilliseconds(100);
            zegar.Tick += Zegar_Tick;
            zegar.Start();

            await Task.Delay(2000);

            zegar.Stop();

            UstawLosowyObraz(Beben1, out ostatni1);
            UstawLosowyObraz(Beben2, out ostatni2);
            UstawLosowyObraz(Beben3, out ostatni3);

            SprawdzWygrana(stawka);
            AktualizujBalans();

            PrzyciskZakrec.IsEnabled = true;
            czyKreciSie = false;
        }

        private void Zegar_Tick(object sender, EventArgs e)
        {
            UstawLosowyObraz(Beben1, out _);
            UstawLosowyObraz(Beben2, out _);
            UstawLosowyObraz(Beben3, out _);
        }

        private void UstawLosowyObraz(System.Windows.Controls.Image reel, out int indexOut)
        {
            int index = los.Next(obrazy.Length);
            indexOut = index;
            reel.Source = new BitmapImage(new Uri(obrazy[index], UriKind.Relative));
        }

        private void SprawdzWygrana(int stawka)
        {
            int mala = 3 * stawka;
            int duza = 10 * stawka;
            int jackpot = 50 * stawka;

            if (ostatni1 == 2 && ostatni2 == 2 && ostatni3 == 2)
            {
                balans += jackpot;
                KomunikatText.Text = $"🎉 JACKPOT!!! +{jackpot}$ 🎉";
                return;
            }

            if (ostatni1 == 1 && ostatni2 == 1 && ostatni3 == 1)
            {
                balans += duza;
                KomunikatText.Text = $"🍺 Wygrywasz 3 drinki w barze +{duza}$!";
                return;
            }

            if ((ostatni1 == 1 && ostatni2 == 1) ||
                (ostatni1 == 1 && ostatni3 == 1) ||
                (ostatni2 == 1 && ostatni3 == 1))
            {
                balans += mala;
                KomunikatText.Text = $"🍹 Wygrywasz 1 drink w barze +{mala}$!";
                return;
            }

            if (ostatni1 == ostatni2 && ostatni2 == ostatni3)
            {
                balans += duza;
                KomunikatText.Text = $"Duża wygrana! +{duza}$";
            }
            else if (ostatni1 == ostatni2 || ostatni1 == ostatni3 || ostatni2 == ostatni3)
            {
                balans += mala;
                KomunikatText.Text = $"Mała wygrana! +{mala}$";
            }
            else
            {
                KomunikatText.Text = "Brak wygranej";
            }
        }

        private void AktualizujBalans()
        {
            BalansText.Text = $"Balans: {balans}$";
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            balans = 100;
            StawkaBox.Text = "10";
            AktualizujBalans();
            KomunikatText.Text = "Gra zresetowana!";
        }
    }
}
