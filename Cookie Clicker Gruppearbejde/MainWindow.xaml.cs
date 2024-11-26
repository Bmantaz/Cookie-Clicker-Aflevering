using System;
using System.ComponentModel;
using System.Media;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace CookieClicker
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        // Game Variables
        private int currentCookies;
        private int totalCookies;
        private int clickPower = 1;
        private int clickPowerCost = 5;
        private int autoClickLevel;
        private int autoClickCost = 10;
        private int criticalStrikeLevel;
        private int criticalStrikeCost = 20;
        private const int CriticalStrikeMaxLevel = 12;

        // Night Mode Variables
        private bool isNightMode;
        private Brush backgroundColor = Brushes.White;
        private Brush textColor = Brushes.Black;

        // Sound Variables
        private bool isMuted;

        // Auto Click Timer
        private DispatcherTimer autoClickTimer;
        private Random random = new Random();

        public event PropertyChangedEventHandler PropertyChanged;

        // Display Properties
        public Brush BackgroundColor
        {
            get => backgroundColor;
            set
            {
                backgroundColor = value;
                OnPropertyChanged(nameof(BackgroundColor));
            }
        }

        public Brush TextColor
        {
            get => textColor;
            set
            {
                textColor = value;
                OnPropertyChanged(nameof(TextColor));
            }
        }

        public bool IsMuted
        {
            get => isMuted;
            set
            {
                isMuted = value;
                OnPropertyChanged(nameof(IsMuted));
            }
        }

        public string CurrentCookiesDisplay => $"Current Cookies: {currentCookies}";
        public string TotalCookiesDisplay => $"Total Cookies: {totalCookies}";
        public string ClickPowerButtonText => $"Upgrade Click Power (Cost: {clickPowerCost})";
        public string AutoClickButtonText => $"Upgrade Auto Click (Cost: {autoClickCost})";
        public string CriticalStrikeButtonText => $"Upgrade Critical Strike ({criticalStrikeLevel}/{CriticalStrikeMaxLevel}) (Cost: {criticalStrikeCost})";

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            autoClickTimer = new DispatcherTimer();
            autoClickTimer.Tick += AutoClickTimer_Tick;
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Cookie Button Click
        private void CookieButton_Click(object sender, RoutedEventArgs e)
        {
            int addedCookies = IsCriticalStrike() ? clickPower * 5 : clickPower;
            AddCookies(addedCookies);
            PlaySound("Assets/CoinCollecter.wav");
        }

        private void AddCookies(int amount)
        {
            currentCookies += amount;
            totalCookies += amount;
            OnPropertyChanged(nameof(CurrentCookiesDisplay));
            OnPropertyChanged(nameof(TotalCookiesDisplay));
        }

        private bool IsCriticalStrike()
        {
            if (criticalStrikeLevel == 0) return false;

            int chance = Math.Min(20 + criticalStrikeLevel * 5, 80);
            return random.Next(0, 100) < chance;
        }

        // Upgrades
        private void UpgradeClickPower_Click(object sender, RoutedEventArgs e)
        {
            if (currentCookies >= clickPowerCost)
            {
                currentCookies -= clickPowerCost;
                clickPower++;
                clickPowerCost *= 2;

                OnPropertyChanged(nameof(ClickPowerButtonText));
                OnPropertyChanged(nameof(CurrentCookiesDisplay));
                PlaySound("Assets/Upgrade Explosion.wav");
            }
        }

        private void UpgradeAutoClick_Click(object sender, RoutedEventArgs e)
        {
            if (currentCookies >= autoClickCost)
            {
                currentCookies -= autoClickCost;
                autoClickLevel++;
                autoClickCost *= 2;

                OnPropertyChanged(nameof(AutoClickButtonText));
                OnPropertyChanged(nameof(CurrentCookiesDisplay));
                UpdateAutoClickTimer();
                PlaySound("Assets/Upgrade Explosion.wav");
            }
        }

        private void UpgradeCriticalStrike_Click(object sender, RoutedEventArgs e)
        {
            if (currentCookies >= criticalStrikeCost && criticalStrikeLevel < CriticalStrikeMaxLevel)
            {
                currentCookies -= criticalStrikeCost;
                criticalStrikeLevel++;
                criticalStrikeCost *= 2;

                OnPropertyChanged(nameof(CriticalStrikeButtonText));
                OnPropertyChanged(nameof(CurrentCookiesDisplay));
                PlaySound("Assets/Upgrade Explosion.wav");
            }
        }

        private void UpdateAutoClickTimer()
        {
            int clicksPerSecond = Math.Min((int)Math.Pow(2, autoClickLevel), 100);
            autoClickTimer.Interval = TimeSpan.FromSeconds(1.0 / clicksPerSecond);

            if (!autoClickTimer.IsEnabled) autoClickTimer.Start();
        }

        private void AutoClickTimer_Tick(object sender, EventArgs e)
        {
            AddCookies(clickPower);
        }

        // Night Mode Toggle
        private void ToggleNightMode_Click(object sender, RoutedEventArgs e)
        {
            isNightMode = !isNightMode;
            BackgroundColor = isNightMode ? Brushes.Black : Brushes.White;
            TextColor = isNightMode ? Brushes.White : Brushes.Black;
        }

        // Sound Management
        private void PlaySound(string soundPath)
        {
            if (isMuted) return;

            try
            {
                new SoundPlayer(soundPath).Play();
            }
            catch { /* Handle missing sound file */ }
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
    }
}