using System;
using System.ComponentModel;
using System.Media;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using System.Collections.ObjectModel; 

namespace CookieClicker
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        // Spilvariabler
        private int currentCookies; // Antal nuværende cookies
        private int totalCookies; // Total antal cookies samlet
        private int clickPower = 1; // Startværdi for klik-kraft
        private int clickPowerCost = 5; // Startpris for at opgradere klik-kraft
        private int autoClickLevel; // Niveau for Auto Click opgradering
        private int autoClickCost = 10; // Startpris for Auto Click opgradering
        private int criticalStrikeLevel; // Niveau for Critical Strike opgradering
        private int criticalStrikeCost = 20; // Startpris for Critical Strike opgradering
        private const int CriticalStrikeMaxLevel = 12; // Maksimalt niveau for Critical Strike

        // Night Mode Variabler
        private bool isNightMode; // Boolean for at angive om Night Mode er aktiveret
        private Brush backgroundColor = Brushes.White; // Standard baggrundsfarve
        private Brush textColor = Brushes.Black; // Standard tekstfarve

        // Lyd Variabler
        private bool isMuted; // Boolean for at indikere om lyd er slået fra

        // Timer til Auto Click
        private DispatcherTimer autoClickTimer; // Timer til at udføre Auto Clicks
        private Random random = new Random(); // Tilfældighedsgenerator til Critical Strike

        public event PropertyChangedEventHandler PropertyChanged; // Event til dataopdatering i UI

        // Display Properties
        public Brush BackgroundColor
        {
            get => backgroundColor; // Getter til baggrundsfarve
            set
            {
                backgroundColor = value; // Setter til baggrundsfarve
                OnPropertyChanged(nameof(BackgroundColor)); // Opdatering af UI
            }
        }

        public Brush TextColor
        {
            get => textColor; // Getter til tekstfarve
            set
            {
                textColor = value; // Setter til tekstfarve
                OnPropertyChanged(nameof(TextColor)); // Opdatering af UI
            }
        }

        public bool IsMuted
        {
            get => isMuted; // Getter til lydstatus
            set
            {
                isMuted = value; // Setter til lydstatus
                OnPropertyChanged(nameof(IsMuted)); // Opdatering af UI
            }
        }

        public string CurrentCookiesDisplay => $"Current Cookies: {currentCookies}"; // Display for nuværende cookies
        public string TotalCookiesDisplay => $"Total Cookies: {totalCookies}"; // Display for totale cookies
        public string ClickPowerButtonText => $"Upgrade Click Power (Cost: {clickPowerCost})"; // Tekst til klik-kraft opgraderingsknap
        public string AutoClickButtonText => $"Upgrade Auto Click (Cost: {autoClickCost})"; // Tekst til Auto Click opgraderingsknap
        public string CriticalStrikeButtonText => $"Upgrade Critical Strike ({criticalStrikeLevel}/{CriticalStrikeMaxLevel}) (Cost: {criticalStrikeCost})"; // Tekst til Critical Strike opgraderingsknap

 

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); // Udløs PropertyChanged event for UI-opdatering
        }







                     // Håndtering af klik på cookie-knappen
        private void CookieButton_Click(object sender, RoutedEventArgs e)
        {
            int addedCookies = IsCriticalStrike() ? clickPower * 5 : clickPower; // Tjek for Critical Strike
            AddCookies(addedCookies); // Tilføj cookies
            PlaySound("Assets/CoinCollecter.wav"); // Spil klik-lyd
        }

        private void AddCookies(int amount)
        {
            currentCookies += amount; // Tilføj til nuværende cookies
            totalCookies += amount; // Tilføj til totale cookies
            OnPropertyChanged(nameof(CurrentCookiesDisplay)); // Opdater UI for nuværende cookies
            OnPropertyChanged(nameof(TotalCookiesDisplay)); // Opdater UI for totale cookies
        }

        private bool IsCriticalStrike()
        {
            if (criticalStrikeLevel == 0) return false; // Ingen Critical Strike hvis niveau er 0

            int chance = Math.Min(20 + criticalStrikeLevel * 5, 80); // Beregn chance med maks på 80%
            
            return random.Next(0, 100) < chance; // Returner true hvis inden for chancegrænse
            
        }

                        









                        //Opgradering af Click Power
        private void UpgradeClickPower_Click(object sender, RoutedEventArgs e)
        {
            if (currentCookies >= clickPowerCost) // Tjek om nok cookies til opgradering
            {
                currentCookies -= clickPowerCost; // Reducer cookies
                clickPower++; // Øg klik-kraft
                clickPowerCost *= 2; // Forøg opgraderingsomkostning eksponentielt

                OnPropertyChanged(nameof(ClickPowerButtonText)); // Opdater UI
                OnPropertyChanged(nameof(CurrentCookiesDisplay)); // Opdater cookies display
                PlaySound("Assets/Upgrade Explosion.wav"); // Spil opgraderingslyd


                // Add log entry
                UpgradeLog.Insert(0, $"Upgraded Click Power to {clickPower} (Cost: {clickPowerCost / 2})");
            }
        }











                    //Auto Clicker Funktion

        public MainWindow()
        {
            InitializeComponent(); // Initialiser UI-komponenter
            DataContext = this; // Sæt DataContext for binding i XAML

            autoClickTimer = new DispatcherTimer(); // Opret en ny DispatcherTimer
            autoClickTimer.Tick += AutoClickTimer_Tick; // Tilføj event handler for timerens tick
        }


                    //Opgradering af AutoClick
        private void UpgradeAutoClick_Click(object sender, RoutedEventArgs e)
        {
            if (currentCookies >= autoClickCost) // Tjek om nok cookies til opgradering
            {
                currentCookies -= autoClickCost; // Reducer cookies
                autoClickLevel++; // Øg Auto Click niveau
                autoClickCost *= 2; // Forøg omkostning eksponentielt

                OnPropertyChanged(nameof(AutoClickButtonText)); // Opdater UI
                OnPropertyChanged(nameof(CurrentCookiesDisplay)); // Opdater cookies display
                UpdateAutoClickTimer(); // Opdater Auto Click timer
                PlaySound("Assets/Upgrade Explosion.wav"); // Spil opgraderingslyd


                // Add log entry
                UpgradeLog.Insert(0, $"Upgraded Auto Click to Level {autoClickLevel} (Cost: {autoClickCost / 2})");
            }
        }


                     private void UpdateAutoClickTimer()
                        {
                          int clicksPerSecond = Math.Min((int)Math.Pow(2, autoClickLevel), 100); // Beregn Auto Click pr. sekund med maks på 100
                         autoClickTimer.Interval = TimeSpan.FromSeconds(1.0 / clicksPerSecond); // Sæt timer-interval

                         if (!autoClickTimer.IsEnabled) autoClickTimer.Start(); // Start timer hvis ikke allerede kørende
                     }

                              private void AutoClickTimer_Tick(object sender, EventArgs e)
                              {
                                 AddCookies(clickPower); // Tilføj cookies baseret på klik-kraft
                              }










                //Opgradering af Critical Strike chance

        private void UpgradeCriticalStrike_Click(object sender, RoutedEventArgs e)
        {
            if (currentCookies >= criticalStrikeCost && criticalStrikeLevel < CriticalStrikeMaxLevel) // Tjek for nok cookies og niveaugrænse
            {
                currentCookies -= criticalStrikeCost; // Reducer cookies
                criticalStrikeLevel++; // Øg Critical Strike niveau
                criticalStrikeCost *= 2; // Forøg omkostning eksponentielt

                OnPropertyChanged(nameof(CriticalStrikeButtonText)); // Opdater UI
                OnPropertyChanged(nameof(CurrentCookiesDisplay)); // Opdater cookies display
                PlaySound("Assets/Upgrade Explosion.wav"); // Spil opgraderingslyd

                // Add log entry
                UpgradeLog.Insert(0, $"Upgraded Critical Strike to Level {criticalStrikeLevel}/{CriticalStrikeMaxLevel} (Cost: {criticalStrikeCost / 2})");
            }
        }












        // Håndtering af Night Mode
        private void ToggleNightMode_Click(object sender, RoutedEventArgs e)
        {
            isNightMode = !isNightMode; // Skift Night Mode status
            BackgroundColor = isNightMode ? Brushes.Black : Brushes.White; // Skift baggrundsfarve
            TextColor = isNightMode ? Brushes.White : Brushes.Black; // Skift tekstfarve
        }

        // Håndtering af lyd
        private void PlaySound(string soundPath)
        {
            if (isMuted) return; // Spil ikke lyd hvis slået fra

            try
            {
                new SoundPlayer(soundPath).Play(); // Spil lydfil
            }
            catch { /* Håndter fejl hvis fil mangler */ }
        }

        // Strategi-tips Button Click
        private void ShowStrategiTips_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(                
                "Optimize Upgrades:\n" +
                "• Investing in Auto Click can exponentially increase your passive cookie income.\n" +
                "• Upgrading Critical Strike can lead to significant cookie gains when combined with high Click Power.\n\n" +
                "Balance Investments:\n" +
                "• Diversify your upgrades between Click Power, Auto Click, and Critical Strike for maximum efficiency.\n" +
                "• Plan your spending wisely due to the escalating costs of the doubling mechanism.\n\n" +
                "Monitor Upgrade Levels:\n" +
                "• Keep an eye on your Critical Strike upgrade level (e.g., 5/12) to assess your current chance of triggering the effect.",
                "Strategi-tips",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
     
        }

        public ObservableCollection<string> UpgradeLog { get; } = new ObservableCollection<string>();
    }



}