using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Text.Json.Serialization;
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
using System.ComponentModel.Design;
using System.Diagnostics;


using Endure.DataAccess;
using Endure.SubWindows;


namespace Endure
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Dictionary<string, Chart> charts = new Dictionary<string, Chart>();

        string selectedDate = string.Empty;
        string currentChart;
        bool InsertRightPanel = true;
        
        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeCharts();

            ChartSelect.ItemsSource = charts.Keys;
            ChartSelect.SelectedItem = charts.First().Key;
            ChartSelect.SelectionChanged += ChartSelect_SelectionChanged;
        }

        public void InitializeCharts()
        {
            if(SQLiteDataAccess.InitDB())
            {
                foreach(string table in SQLiteDataAccess.GetTables())
                {
                    charts.Add(table, new Chart());

                    charts[table].Initialize(canvas);

                    List<string> colomns = new List<string>();
                    foreach (string colomn in SQLiteDataAccess.GetColomns(table))
                    {
                        colomns.Add(colomn);
                        if(colomn != "Date")
                            charts[table].AddInput(colomn);
                    }

                    charts[table].HandelInputFromDB(SQLiteDataAccess.LoadTable(table, colomns), canvas);
                }

                currentChart = charts.First().Key;

                foreach(var set in charts[currentChart].panelInput.Elements)
                {
                    if(InsertRightPanel)
                    {
                        RightPanel.Children.Add(set.Item1);
                        RightPanel.Children.Add(set.Item2);
                        InsertRightPanel = false;
                    }
                    else
                    {
                        LeftPanel.Children.Add(set.Item1);
                        LeftPanel.Children.Add(set.Item2);
                        InsertRightPanel = true;
                    }
                }

                charts[currentChart].RemoveFromCanvas(canvas);
                charts[currentChart].DrawOnCanvas(canvas);
            }
            else
            {
                charts.Add("Bench", new Chart());
                charts.Add("Pull Up", new Chart());
                charts.Add("Dead Lift", new Chart());
                charts.Add("Squats", new Chart());

                foreach(var chart in charts)
                {
                    chart.Value.AddInput("Weight");
                    chart.Value.AddInput("Reps");

                    string[] test = chart.Value.Inputs.ToArray();

                    SQLiteDataAccess.CreateTable(chart.Key, test);
                }

                currentChart = charts.First().Key;

                foreach (var set in charts[currentChart].panelInput.Elements)
                {
                    if (InsertRightPanel)
                    {
                        RightPanel.Children.Add(set.Item1);
                        RightPanel.Children.Add(set.Item2);
                        InsertRightPanel = false;
                    }
                    else
                    {
                        LeftPanel.Children.Add(set.Item1);
                        LeftPanel.Children.Add(set.Item2);
                        InsertRightPanel = true;
                    }
                }
            }
        }

        private void ChartSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;

            charts[currentChart].RemoveFromCanvas(canvas);
            RightPanel.Children.Clear();
            LeftPanel.Children.Clear();
            currentChart = comboBox.SelectedItem.ToString();
            if(charts[currentChart].IsInitialized)
            {
                charts[currentChart].DrawOnCanvas(canvas);

                foreach(var set in charts[currentChart].panelInput.Elements)
                {
                    if(InsertRightPanel)
                    {
                        RightPanel.Children.Add(set.Item1);
                        RightPanel.Children.Add(set.Item2);
                        InsertRightPanel = false;
                    }
                    else
                    {
                        LeftPanel.Children.Add(set.Item1);
                        LeftPanel.Children.Add(set.Item2);
                        InsertRightPanel = true;
                    }
                }
            }
            else
            {
                charts[currentChart].Initialize(canvas);

                foreach (var set in charts[currentChart].panelInput.Elements)
                {
                    if (InsertRightPanel)
                    {
                        RightPanel.Children.Add(set.Item1);
                        RightPanel.Children.Add(set.Item2);
                        InsertRightPanel = false;
                    }
                    else
                    {
                        LeftPanel.Children.Add(set.Item1);
                        LeftPanel.Children.Add(set.Item2);
                        InsertRightPanel = true;
                    }
                }
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
            string[] date = chosen_date.Text.Split(".");

            if (!charts[currentChart].HandelNewInput(date, out List<string> textFialds, out bool DateExists, canvas))
            {
                ErrorWindow errorWindow = new ErrorWindow("Input Error", "You need to insert a number. Ex: <4>, <12.3>, <5,1> etz...");
                errorWindow.Owner = this;
                errorWindow.ShowDialog();
            }
            else
            {
                if (!DateExists)
                {
                    textFialds.Insert(0, chosen_date.Text);
                    SQLiteDataAccess.SaveToTable(currentChart, textFialds);
                }
                else
                {
                    List<string> colomn = new List<string>();
                    List<string> values = new List<string>();
                    colomn.Add("Date");
                    values.Add(chosen_date.Text);
                    for (int i = 0; i < textFialds.Count; i++)
                    {
                        if(textFialds[i] != string.Empty)
                        {
                            colomn.Add(charts[currentChart].panelInput.Elements[i].Item1.Text);
                            values.Add(textFialds[i]);
                        }
                    }
                    SQLiteDataAccess.SaveToTableAtSpesifiedRow(currentChart, colomn, values);
                }
            }
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
            selectedDate = DateTime.Now.ToString().Split(" ")[0];
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
            selectedDate = calendar.SelectedDate.ToString().Split(" ")[0];
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

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine(e.GetPosition(sender as Canvas).X);

            if (charts[currentChart].FindDate(e.GetPosition(sender as Canvas).X, out string date))
            {
                List<(string, string)> output = new List<(string, string)>();
                LeftClickOnCanvasWindow clickOnCanvas = new LeftClickOnCanvasWindow("Add to Chart", date, charts[currentChart].panelInput.ElementNames, output);
                clickOnCanvas.Owner = this;
                clickOnCanvas.ShowDialog();

                if (output.Count > 0)
                {
                    if (!charts[currentChart].HandelNewInput(date.Split("."), output, out bool DateExists, canvas))
                    {
                        ErrorWindow errorWindow = new ErrorWindow("Input Error", "You need to insert a number. Ex: <4>, <12.3>, <5,1> etz...");
                        errorWindow.Owner = this;
                        errorWindow.ShowDialog();
                    }
                    else
                    {
                        if (!DateExists)
                        {

                            List<string> values = new List<string>();
                            values.Add(date);
                            foreach (var value in output)
                            {
                                values.Add(value.Item2);
                            }
                            SQLiteDataAccess.SaveToTable(currentChart, values);
                        }
                        else
                        {
                            List<string> colomn = new List<string>();
                            List<string> values = new List<string>();
                            colomn.Add("Date");
                            values.Add(date);
                            for (int i = 0; i < output.Count; i++)
                            {
                                if (output[i].Item2 != string.Empty)
                                {
                                    colomn.Add(output[i].Item1);
                                    values.Add(output[i].Item2);
                                }
                            }
                            SQLiteDataAccess.SaveToTableAtSpesifiedRow(currentChart, colomn, values);
                        }
                    }
                }
                Debug.WriteLine(date);
            }
        }

        private void Canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ErrorWindow errorWindow = new ErrorWindow("On Right Click", "There is no soport for Right Click ATM");
            errorWindow.Owner = this;
            errorWindow.ShowDialog();
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

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            ErrorWindow settings = new ErrorWindow("Settings", "There is no soport for Settings ATM");
            settings.Owner = this;
            settings.ShowDialog();
        }
    }
}
