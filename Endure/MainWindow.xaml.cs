using Endure.Pages;
using Endure.Settings;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


namespace Endure
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindowConfig MainConfig;
        public List<Page> MainPages;
        List<Page> SettingsTabPages;
        CreateNewChart InitChartPage;

        public MainWindow()
        {
            InitializeComponent();

            Initialize();
        }

        private void Initialize()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            MainConfig = new MainWindowConfig(this);

            MainPages = new List<Page>() { new ChartPage(MainConfig.Charts) { Owner = this }, new Page() { Background = Brushes.Black } };

            SettingsTabPages = new List<Page>() { new SettingsPage(MainConfig) };
            InitChartPage = new CreateNewChart(MainConfig.Charts, SettingsTabPages[0]);

            SettingsFrame.Visibility = Visibility.Collapsed;

            MainConfig.Ininitiolize();

            Background = MainConfig.BackgroundBrush;
            this.PreviewMouseDown += SettingsFrame_PreviewMouseDown;
        }

        public void EmptyTabContent(object sender)
        {
            if (sender is ChartsConfig)
            {
                MainTabFrame.Content = InitChartPage;
            }
        }

        public void RePopulateTebContent(object sender)
        {
            if (sender is ChartsConfig && MainTabFrame.Content.Equals(InitChartPage))
            {
                MainTabFrame.Content = MainPages[0];
            }
        }

        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int i = ((sender as TabControl).SelectedItem as TabItem).TabIndex;
            if (!MainConfig.Charts.GotChartData && i == 0)
            {
                MainTabFrame.Content = InitChartPage;
            }
            else
            {
                MainTabFrame.Content = MainPages[i];
            }

        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            if (SettingsFrame.Visibility == Visibility.Visible)
            {
                SettingsFrame.Visibility = Visibility.Collapsed;
                SettingsButton.Background = Brushes.Transparent;
                SettingsButton.BorderBrush = Brushes.Transparent;
            }
            else
            {
                SettingsFrame.Content = SettingsTabPages[(sender as Button).TabIndex];
                SettingsFrame.Visibility = Visibility.Visible;
                SettingsButton.Background = new SolidColorBrush(Color.FromArgb(0x44, 0x00, 0x00, 0x00));
                SettingsButton.BorderBrush = SettingsButton.Background;
            }
        }

        private void SettingsFrame_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (SettingsFrame.IsVisible && !SettingsFrame.IsMouseOver && !SettingsButton.IsMouseOver)
            {
                SettingsFrame.Visibility = Visibility.Collapsed;
                SettingsButton.Background = Brushes.Transparent;
                SettingsButton.BorderBrush = Brushes.Transparent;
            }
        }
    }
}
