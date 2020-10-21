using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Endure
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Dictionary<string, Chart> charts = new Dictionary<string, Chart>();

        string selectedDate = string.Empty;
        string currentChart;// = string.Empty;
        
        public MainWindow()
        {
            InitializeComponent();
            InitializeCharts();

            ChartSelect.ItemsSource = charts.Keys;
            ChartSelect.SelectedItem = charts.First().Key;
            ChartSelect.SelectionChanged += ChartSelect_SelectionChanged;
        }

        public void InitializeCharts()
        {
            charts.Add("Bench", new Chart());
            charts.Add("Pull Up", new Chart());
            charts.Add("Dead Lift", new Chart());
            charts.Add("Squats", new Chart());

            currentChart = charts.First().Key;
        }

        private void ChartSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;

            charts[currentChart].RemoveFromCanvas(canvas);

            currentChart = comboBox.SelectedItem.ToString();
            if(charts[currentChart].IsInitialized)
            {
                charts[currentChart].DrawOnCanvas(canvas);
            }
            else
            {
                charts[currentChart].Initialize(canvas);
            }

            if(charts[currentChart].DrawChartLines)
            {
                LineCheck.IsChecked = true;
            }
            else
            {
                LineCheck.IsChecked = false;
            }
        }

        private void AddToChart_Click(object sender, RoutedEventArgs e)
        {
            string[] date = chosen_date.Text.Split(" ")[0].Split(".");

            charts[currentChart].HandelNewInput(date, inputWeight.Text, canvas);
        }

        private void Canvas_Loaded(object sender, RoutedEventArgs e)
        {
            charts[currentChart].Initialize(canvas);

            canvas.Background = Brushes.LightGray;
        }

        private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            charts[currentChart].OnSizeChange(canvas);
            canvas.MinHeight = charts[currentChart].MinHeight;
            canvas.MinWidth = charts[currentChart].MinWidth;
        }

        private void DropCalender_Loaded(object sender, RoutedEventArgs e)
        {
            selectedDate = DateTime.Now.ToString();
            chosen_date.Text = selectedDate;

            DropCalender.SelectedItem = chosen_date;
        }

        private void DropCalender_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            comboBox.SelectedItem = chosen_date;
        }

        private void Calender_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            Calendar calendar = sender as Calendar;
            selectedDate = calendar.SelectedDate.ToString();
            chosen_date.Text = selectedDate;


            DropCalender.SelectedItem = chosen_date;
            DropCalender.IsDropDownOpen = false;
        }

        private void scroll_left_Click(object sender, RoutedEventArgs e)
        {
            charts[currentChart].OnMoveBackward(canvas);
        }

        private void scroll_right_Click(object sender, RoutedEventArgs e)
        {
            charts[currentChart].OnMoveForward(canvas);
        }

        private void Canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Window window = new Window();
            window.Show();

            this.Close();
        }

        private void Line_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if(!charts[currentChart].DrawChartLines)
                charts[currentChart].AddLines(canvas);
        }

        private void Line_CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if(charts[currentChart].DrawChartLines)
                charts[currentChart].RemoveLines(canvas);
        }
    }
}
