﻿using System;
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
using Endure.Settings;

namespace Endure.Pages
{
    /// <summary>
    /// Interaction logic for ChartPage.xaml
    /// </summary>
    public partial class ChartPage : Page
    {
        public Dictionary<string, Chart> charts = new Dictionary<string, Chart>();
        ChartsConfig ChartsConfig;
        string selectedDate = string.Empty;
        string currentChart;

        public object Owner;

        public ChartPage(ChartsConfig chartsConfig)
        {
            InitializeComponent();
            Initiolize(chartsConfig);
        }

        public void Initiolize(ChartsConfig chartsConfig)
        {
            ChartsConfig = chartsConfig;
            InitializeCharts();

            ChartSelect.SelectedItem = currentChart;
            ChartSelect.SelectionChanged += ChartSelect_SelectionChanged;

            selectedDate = DateTime.Now.Date.ToString();
            chosen_date.Text = selectedDate;

            DropCalender.Text = chosen_date.Text;
            DropCalender.SelectedItem = chosen_date;
        }

        public void InitializeCharts()
        {
            if (ChartsConfig.GotChartData)
            {
                foreach (string table in ChartsConfig.Charts.Keys)
                {
                    charts.Add(table, new Chart(ChartsConfig.Charts[table]));
                    ChartSelect.Items.Add(table);

                    List<string> colomns = new List<string>() { "Date" };
                    foreach (string colomn in ChartsConfig.Charts[table].ChartInputs)
                    {
                        colomns.Add(colomn);
                        charts[table].AddInput(colomn);
                    }

                    charts[table].Initialize(canvas);
                    charts[table].HandelInputFromDB(SQLiteDataAccess.LoadTable(table, colomns), canvas);
                }

                currentChart = charts.First().Key;

                InputFrame.Content = ChartsConfig.Charts[currentChart].WrapItems;

                charts[currentChart].RemoveFromCanvas(canvas);
                charts[currentChart].DrawOnCanvas(canvas);
            }
        }

        public void UpdateChartSelect(string name, bool add)
        {
            if(add)
            {
                charts.Add(name, new Chart(ChartsConfig.Charts[name]));
                ChartSelect.Items.Add(name);
                currentChart = name;
                ChartSelect.SelectedItem = currentChart;
            }
            else
            {
                charts.Remove(name);
                ChartSelect.Items.Remove(name);
            }
        }

        

        private void ChartSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;

            if (charts.ContainsKey(currentChart))
            {
                charts[currentChart].RemoveFromCanvas(canvas);
                currentChart = comboBox.SelectedItem.ToString();

                InputFrame.Content = ChartsConfig.Charts[currentChart].WrapItems;

                if (charts[currentChart].IsInitialized)
                {
                    charts[currentChart].DrawOnCanvas(canvas);
                }
                else
                {
                    foreach (string colomn in ChartsConfig.Charts[currentChart].ChartInputs)
                    {
                        charts[currentChart].AddInput(colomn);
                    }
                    charts[currentChart].Initialize(canvas);
                }

                canvas.Background = ChartsConfig.Charts[currentChart].CanvasBackGround;

                if (charts[currentChart].DrawChartLines)
                {
                    LineCheck.IsChecked = true;
                }
                else
                {
                    LineCheck.IsChecked = false;
                }
            }
            else
            {
                if(charts.Count > 0)
                {
                    currentChart = charts.First().Key;
                    comboBox.SelectedItem = currentChart;
                    ChartSelect_SelectionChanged(sender, e);
                }
            }
        }

        private void AddToChart_Click(object sender, RoutedEventArgs e)
        {
            string[] date = chosen_date.Text.Split(".");

            if (!charts[currentChart].HandelNewInput(date, out List<string> textFialds, out bool DateExists, canvas))
            {
                ErrorWindow errorWindow = new ErrorWindow("Input Error", "You need to insert a number. Ex: <4>, <12.3>, <5,1> etz...");
                if (Owner is Window)
                    errorWindow.Owner = (Owner as Window);
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
                        if (textFialds[i] != string.Empty)
                        {
                            colomn.Add(ChartsConfig.Charts[currentChart].ChartInputs[i]);
                            values.Add(textFialds[i]);
                        }
                    }
                    SQLiteDataAccess.SaveToTableAtSpesifiedRow(currentChart, colomn, values);
                }
            }
        }

        private void Canvas_Loaded(object sender, RoutedEventArgs e)
        {
            canvas.Background = ChartsConfig.Charts[currentChart].CanvasBackGround;
            charts[currentChart].Initialize(canvas);
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

            DropCalender.Text = chosen_date.Text;
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

                LeftClickOnCanvasWindow clickOnCanvas = new LeftClickOnCanvasWindow("Add to Chart", date, ChartsConfig.Charts[currentChart].ChartInputs, output);
                if (Owner is Window)
                    clickOnCanvas.Owner = (Owner as Window);

                clickOnCanvas.ShowDialog();

                if (output.Count > 0)
                {
                    if (!charts[currentChart].HandelNewInput(date.Split("."), output, out bool DateExists, canvas))
                    {
                        ErrorWindow errorWindow = new ErrorWindow("Input Error", "You need to insert a number. Ex: <4>, <12.3>, <5,1> etz...");
                        if (Owner is Window)
                            errorWindow.Owner = (Owner as Window);
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
            if(Owner is Window)
                errorWindow.Owner = (Owner as Window);
            errorWindow.ShowDialog();
        }

        private void Line_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (!charts[currentChart].DrawChartLines)
                charts[currentChart].AddLines(canvas);
        }

        private void Line_CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (charts[currentChart].DrawChartLines)
                charts[currentChart].RemoveLines(canvas);
        }

        private void ChartSelect_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if(e.OriginalSource is ScrollViewer)
            {
                ScrollViewer viewer = (e.OriginalSource as ScrollViewer);
                viewer.MaxHeight = ((sender as ComboBox).ActualHeight) * 5;
            }
        }
    }
}
