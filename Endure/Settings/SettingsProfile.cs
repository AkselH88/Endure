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
using Endure.Pages;

namespace Endure.Settings
{
    public class MainWindowConfig
    {
        public Brush BackgroundBrush { get; private set; }
        public ChartsConfig Charts { get; private set; }

        public object Owner { set; get; }

        public MainWindowConfig()
        {
            BackgroundBrush = new SolidColorBrush(Color.FromArgb(0xff, 0x8f, 0xbc, 0x8f));
            Charts = new ChartsConfig();
        }

        public MainWindowConfig(object Owner)
        {
            this.Owner = Owner;
            BackgroundBrush = new SolidColorBrush(Color.FromArgb(0xff, 0x8f, 0xbc, 0x8f));
            Charts = new ChartsConfig();
        }

        public void Ininitiolize()
        {
            Charts.Owner = (Owner as MainWindow).MainPages[0];
            Charts.Parent = this;
        }
    }
    public class SubWindowConfig
    {

    }
    public class StartupProfile
    {

    }

    public class ChartsConfig
    {
        public Dictionary<string, ChartConfigProfile> Charts { get; private set; }
        public object Owner { set; get; }
        public object Parent { set; get; }

        public bool GotChartData { get; private set; }
        public ChartsConfig()
        {
            Ininitiolize();
        }

        private void Ininitiolize()
        {
            Charts = new Dictionary<string, ChartConfigProfile>();

            if (SQLiteDataAccess.InitDB())
            {
                GetTables();
            }
            else
            {
                CreateTable();
                GotChartData = true;
            }
        }

        // returns gray if somthing wrong hapends
        private Color FromString(string toColor)
        {
            if(toColor.Contains("#"))
            {
                toColor = toColor.Remove(0,1);

                return Color.FromArgb(Convert.ToByte(toColor.Substring(0, 2), 16),
                                      Convert.ToByte(toColor.Substring(2, 2), 16),
                                      Convert.ToByte(toColor.Substring(4, 2), 16),
                                      Convert.ToByte(toColor.Substring(6, 2), 16));
            }

            return Color.FromArgb(0xff, 0x88, 0x88, 0x88);
        }

        private static class ChartColors
        {
            static public string Table = "Inputs";
            static public List<string> Colomns = new List<string> { "Chart", "Input", "Ellipse", "Line" };
            static public int EllipseIndex = 2;
            static public int LineIndex = 3;

            static public string CanvasTable = "CanvasColor";
            static public List<string> CanvasColomns = new List<string> { "Chart", "Color" };
        }

        private void GetTables()
        {
            List<string> tables = SQLiteDataAccess.GetTables();
            if (tables.Count > 0)
            {
                List<List<string>> inputs = SQLiteDataAccess.LoadTable(ChartColors.Table, ChartColors.Colomns);
                GotChartData = true;
                foreach (string table in tables)
                {
                    if(table != ChartColors.Table && table != ChartColors.CanvasTable)
                    {
                        Charts.Add(table, new ChartConfigProfile(
                            FromString(
                                SQLiteDataAccess.LoadValueFromTable(
                                    ChartColors.CanvasTable,
                                    ChartColors.CanvasColomns[1],
                                    ChartColors.CanvasColomns[0],
                                    table
                                    )
                                )));

                        foreach (string colomn in SQLiteDataAccess.GetColomns(table))
                        {
                            if (colomn != "Date")
                            {
                                foreach(var input in inputs)
                                {
                                    if(input.Contains(table) && input.Contains(colomn))
                                    {
                                        Charts[table].Add(colomn,
                                            new SolidColorBrush(FromString(input[ChartColors.EllipseIndex])),
                                            new SolidColorBrush(FromString(input[ChartColors.LineIndex])));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                GotChartData = false;
            }
        }

        private void CreateTable()
        {
            Charts.Add("Bench", new ChartConfigProfile());
            Charts["Bench"].Add("Weight", new SolidColorBrush(Color.FromArgb(0xff, 0x62, 0xa1, 0xb1)), new SolidColorBrush(Color.FromArgb(0xff, 0x6f, 0xaf, 0xbf)));
            Charts["Bench"].Add("Reps", new SolidColorBrush(Color.FromArgb(0xff, 0xc2, 0x41, 0x7f)), new SolidColorBrush(Color.FromArgb(0xff, 0xb2, 0x4f, 0x80)));

            Charts.Add("Pull Up", new ChartConfigProfile());
            Charts["Pull Up"].Add("Weight", new SolidColorBrush(Color.FromArgb(0xff, 0x62, 0xa1, 0xb1)), new SolidColorBrush(Color.FromArgb(0xff, 0x6f, 0xaf, 0xbf)));
            Charts["Pull Up"].Add("Reps", new SolidColorBrush(Color.FromArgb(0xff, 0xc2, 0x41, 0x7f)), new SolidColorBrush(Color.FromArgb(0xff, 0xb2, 0x4f, 0x80)));

            Charts.Add("Dead Lift", new ChartConfigProfile());
            Charts["Dead Lift"].Add("Weight", new SolidColorBrush(Color.FromArgb(0xff, 0x62, 0xa1, 0xb1)), new SolidColorBrush(Color.FromArgb(0xff, 0x6f, 0xaf, 0xbf)));
            Charts["Dead Lift"].Add("Reps", new SolidColorBrush(Color.FromArgb(0xff, 0xc2, 0x41, 0x7f)), new SolidColorBrush(Color.FromArgb(0xff, 0xb2, 0x4f, 0x80)));

            Charts.Add("Squats", new ChartConfigProfile());
            Charts["Squats"].Add("Weight", new SolidColorBrush(Color.FromArgb(0xff, 0x62, 0xa1, 0xb1)), new SolidColorBrush(Color.FromArgb(0xff, 0x6f, 0xaf, 0xbf)));
            Charts["Squats"].Add("Reps", new SolidColorBrush(Color.FromArgb(0xff, 0xc2, 0x41, 0x7f)), new SolidColorBrush(Color.FromArgb(0xff, 0xb2, 0x4f, 0x80)));

            SQLiteDataAccess.CreateTable(ChartColors.Table, ChartColors.Colomns);
            SQLiteDataAccess.CreateTable(ChartColors.CanvasTable, ChartColors.CanvasColomns);
            foreach (var chart in Charts)
            {
                SQLiteDataAccess.CreateTable(chart.Key, chart.Value.ChartInputs.Prepend("Date").ToList());
                SQLiteDataAccess.SaveToTable(ChartColors.CanvasTable, new List<string> { chart.Key, chart.Value.CanvasBackGround.ToString() });

                for (int i = 0; i < chart.Value.ChartInputs.Count; i++)
                {
                    SQLiteDataAccess.SaveToTable(ChartColors.Table, new List<string>
                    {
                        chart.Key,
                        chart.Value.ChartInputs[i],
                        chart.Value.Ellipses[i].ToString(),
                        chart.Value.Lines[i].ToString()
                    });
                }
            }
        }

        public void UpdateChartColor(string name, Brush brush)
        {
            SQLiteDataAccess.UpdateTable(ChartColors.CanvasTable, ChartColors.CanvasColomns[1], brush.ToString(), ChartColors.CanvasColomns[0], name);
        }

        //static public List<string> Colomns = new List<string> { "Chart", "Input", "Ellipse", "Line" };
        /// <summary>
        /// identifier = 2 for ellipse and 3 for line deafult 2
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="input"></param>
        /// <param name="identifier"></param>
        /// <param name="brush"></param>
        public void UpdateChartInputColor(string chart, string input, Brush brush, int identifier = 2)
        {
            SQLiteDataAccess.UpdateTable(ChartColors.Table, ChartColors.Colomns[identifier], brush.ToString(), ChartColors.Colomns[0], chart, ChartColors.Colomns[1], input);
        }

        public void OpenCreateTableWindow()
        {

        }

        public void AddChart(string name, Brush canvasBackGround, List<string> inputs, List<Brush> ellipseBrush, List<Brush> lineBrush)
        {
            if (Owner is ChartPage)
            {
                Charts.Add(name, new ChartConfigProfile(canvasBackGround, inputs, ellipseBrush, lineBrush));
                (Owner as ChartPage).UpdateChartSelect(name, true);
                SQLiteDataAccess.CreateTable(name, inputs.Prepend("Date").ToList());
                SQLiteDataAccess.SaveToTable(ChartColors.CanvasTable, new List<string> { name, canvasBackGround.ToString() });
                for (int i = 0; i < inputs.Count; i++)
                {
                    SQLiteDataAccess.SaveToTable(ChartColors.Table, new List<string>
                    {
                        name,
                        inputs[i],
                        ellipseBrush[i].ToString(),
                        lineBrush[i].ToString()
                    });
                }
                
                if (!GotChartData)
                {
                    GotChartData = true;
                    ((Parent as MainWindowConfig).Owner as MainWindow).RePopulateTebContent(this);
                }
            }
        }

        public void AddInputToChart(string name, string input, Brush ellipseBrush, Brush lineBrush)
        {
            SQLiteDataAccess.SaveToTable(ChartColors.Table, new List<string>
            {
                name,
                input,
                ellipseBrush.ToString(),
                lineBrush.ToString()
            });
            SQLiteDataAccess.AddColomn(name, input);
            Charts[name].Add(input, ellipseBrush, lineBrush);
            (Owner as ChartPage).charts[name].AddInput(input);
        }

        public void RemoveChart(string name)
        {
            if (Owner is ChartPage)
            {
                Charts.Remove(name);
                SQLiteDataAccess.RemoveTable(name);
                SQLiteDataAccess.RemoveRow(ChartColors.Table, ChartColors.Colomns[0], name);
                SQLiteDataAccess.RemoveRow(ChartColors.CanvasTable, ChartColors.CanvasColomns[0], name);
                
                if (Charts.Count == 0)
                {
                    GotChartData = false;
                    ((Parent as MainWindowConfig).Owner as MainWindow).EmptyTabContent(this);
                }
                (Owner as ChartPage).UpdateChartSelect(name, false);
            }
        }

    }
    public class ChartConfigProfile
    {
        public WrapPanel WrapItems = new WrapPanel() { Orientation = Orientation.Horizontal, MaxWidth = 160 };
        public Brush CanvasBackGround { get; }
        public List<string> ChartInputs { get; }
        public List<Brush> Ellipses { get; }
        public List<Brush> Lines { get; }
        public ChartConfigProfile()
        {
            CanvasBackGround = new SolidColorBrush(Color.FromArgb(0xff, 0x52, 0x6f, 0x65));
            ChartInputs = new List<string>();
            Ellipses = new List<Brush>();
            Lines = new List<Brush>();
        }

        public ChartConfigProfile(Color canvasColor)
        {
            CanvasBackGround = new SolidColorBrush(canvasColor);
            ChartInputs = new List<string>();
            Ellipses = new List<Brush>();
            Lines = new List<Brush>();
        }

        public ChartConfigProfile(Brush canvasBackGround, List<string> inputs, List<Brush> ellipseBrush, List<Brush> lineBrush)
        {
            CanvasBackGround = canvasBackGround;
            ChartInputs = inputs;
            Ellipses = ellipseBrush;
            Lines = lineBrush;

            for(int i = 0; i < ChartInputs.Count; i++)
            {
                AddToInputPanel(ChartInputs[i]);
            }
        }

        public void AddToInputPanel(string name)
        {
            StackPanel Items = new StackPanel()
            { Margin = new Thickness() { Left = 5, Right = 5 }, Orientation = Orientation.Vertical, Children = { CreateTextBlock(name), CreateTextBox(name) } };

            WrapItems.Children.Add(Items);
        }

        public void AddBrush(string name, Brush ellipse, Brush line)
        {

        }

        public void Add(string name, Brush ellipse, Brush line)
        {
            ChartInputs.Add(name);
            Ellipses.Add(ellipse);
            Lines.Add(line);
            StackPanel Items = new StackPanel() { Margin = new Thickness() { Left = 5, Right = 5 }, Orientation = Orientation.Vertical, Children = { CreateTextBlock(name), CreateTextBox(name) } };
            
            WrapItems.Children.Add(Items);
        }

        private TextBox CreateTextBox(string name)
        {
            TextBox box = new TextBox
            {
                Name = name,
                Width = 70,
                Height = 25,
                VerticalAlignment = VerticalAlignment.Center
            };

            box.PreviewTextInput += TextBoxNumberValidation;

            return box;
        }

        private TextBlock CreateTextBlock(string name)
        {
            return new TextBlock
            {
                Name = name,
                Text = name,
                FontSize = 14,
                FontWeight = FontWeights.DemiBold,
                HorizontalAlignment = HorizontalAlignment.Center
            };
        }

        private void TextBoxNumberValidation(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox.Text.Contains('.') || textBox.Text.Contains(',') || textBox.Text == string.Empty)
            {
                Regex regex = new Regex("[^0-9]+");
                e.Handled = regex.IsMatch(e.Text);
            }
            else
            {
                Regex regex = new Regex("[^0-9,.]+");
                e.Handled = regex.IsMatch(e.Text);
            }
        }
    }
}
