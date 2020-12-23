using System;
using System.Collections.Generic;
using System.Text;
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
using System.Threading;
using System.Threading.Tasks;


using Endure.DataAccess;
using Endure.SubWindows;
using Endure.Settings;

namespace Endure.Pages
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        MainWindowConfig Config;
        public MainWindow Owner;
        public Brush Frozen { get; set; }

        public SettingsPage(MainWindowConfig config)
        {
            InitializeComponent();
            Config = config;
            Background = Config.BackgroundBrush;
            ContentBilder();

            Frozen = new SolidColorBrush(((SolidColorBrush)Config.BackgroundBrush).Color);

            this.Loaded += SettingsPage_Loaded;
        }

        private void SettingsPage_Loaded(object sender, RoutedEventArgs e)
        {
            Color color = ((SolidColorBrush)Frozen.GetAsFrozen()).Color;
            R.Value = color.R;
            G.Value = color.G;
            B.Value = color.B;
        }

        private void ContentBilder()
        {
            Alterebals();
            Removables();
        }

        private TreeViewItem CreateAlterChartViewItem(string name, ChartConfigProfile config)
        {
            TreeViewItem toAlter = new TreeViewItem() { Header = name, Tag = name };
            Border border = new Border() { BorderThickness = new Thickness(2), BorderBrush = Brushes.Black, CornerRadius = new CornerRadius(3) };

            StackPanel toTree = new StackPanel() { Background = config.CanvasBackGround.Clone() };

            TreeViewItem canvasColor = new TreeViewItem { Header = "Canvas Color" };

            Color color = ((SolidColorBrush)config.CanvasBackGround.GetAsFrozen()).Color;
            double[] rgb = new double[] { color.R, color.G, color.B };
            for (int i = 0; i < rgb.Length; i++)
            {
                Slider slider = new Slider() { Width = 100, Minimum = 0, Maximum = 255, Value = rgb[i] };
                slider.ValueChanged += On_ValueChanged;
                canvasColor.Items.Add(slider);
            }

            toTree.Children.Add(canvasColor);
            TreeViewItem removeInputs = new TreeViewItem() { Header = "Remove Inputs", Name = "remove" };

            foreach (var input in config.ChartInputs)
            {
                int i = config.ChartInputs.IndexOf(input);
                toTree.Children.Add(AddColorOptions(input, config.Ellipses[i].Clone(), config.Lines[i].Clone()));
                removeInputs.Items.Add(new CheckBox()
                {
                    IsChecked = false,
                    Content = input
                });
            }

            TreeViewItem addInputs = new TreeViewItem() { Header = "Add Inputs" };
            {
                addInputs.Items.Add(AddColorOptions("Input Colors"));
                addInputs.Items.Add(new TextBox() { MinWidth = 70, MaxWidth = 150 });

                Button add = new Button() { Content = "Add" };
                add.Click += Add_Click;
                addInputs.Items.Add(add);
            }

            toTree.Children.Add(addInputs);

            border.Child = toTree;
            toAlter.Items.Add(border);

            toAlter.Items.Add(removeInputs);

            Button save = new Button() { Content = "Save Changes" };
            save.Click += Save_Click;
            toAlter.Items.Add(save);

            return toAlter;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            foreach(var item in (((((sender as Button).Parent as TreeViewItem).Parent as StackPanel).Parent as Border).Parent as TreeViewItem).Items)
            {
                if(item is TreeViewItem)
                {
                    if((item as TreeViewItem).Name == "remove")
                    {
                        if((((sender as Button).Parent as TreeViewItem).Items[1] as TextBox).Text != string.Empty)
                        {
                            bool add = true;
                            foreach(CheckBox checkBox in (item as TreeViewItem).Items)
                            {
                                if(checkBox.Content.ToString() == (((sender as Button).Parent as TreeViewItem).Items[1] as TextBox).Text)
                                {
                                    add = false;
                                    break;
                                }
                            }

                            if(add)
                            {
                                (item as TreeViewItem).Items.Add(new CheckBox()
                                {
                                    IsChecked = false,
                                    Content = (((sender as Button).Parent as TreeViewItem).Items[1] as TextBox).Text
                                });
                            }
                        }
                    }
                }
                if(item is Border)
                {
                    StackPanel stackPanel = ((item as Border).Child as StackPanel);
                    stackPanel.Children.Insert(
                        stackPanel.Children.IndexOf(
                            ((sender as Button).Parent as TreeViewItem)),
                            AddColorOptions((((sender as Button).Parent as TreeViewItem).Items[1] as TextBox).Text,
                            (((((sender as Button).Parent as TreeViewItem).Items[0] as TreeViewItem).Items[1] as StackPanel).Children[0] as Ellipse).Fill.Clone(),
                            (((((sender as Button).Parent as TreeViewItem).Items[0] as TreeViewItem).Items[1] as StackPanel).Children[1] as Border).Background.Clone()
                            ));
                }
            }

            (((sender as Button).Parent as TreeViewItem).Items[1] as TextBox).Text = "";
        }

        private TreeViewItem FindCreatedAlterChartViewItem(string name)
        {
            foreach(TreeViewItem item in AlterChart.Items)
            {
                if (item.Header as string == name)
                    return item;
            }

            return null;
        }

        private void Alterebals()
        {
            foreach(var item in Config.Charts.Charts)
            {
                AlterChart.Items.Add(CreateAlterChartViewItem(item.Key, item.Value));
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string chartName = ((sender as Button).Parent as TreeViewItem).Header.ToString();
            StackPanel chartContent = ((((sender as Button).Parent as TreeViewItem).Items[0] as Border).Child as StackPanel);
            TreeViewItem toRemove = (((sender as Button).Parent as TreeViewItem).Items[1] as TreeViewItem);

            for(int i = 0; i < toRemove.Items.Count; i++)
            {
                if ((bool)(toRemove.Items[i] as CheckBox).IsChecked)
                {
                    for(int j = 1; j < chartContent.Children.Count -1; j++)
                    {
                        if ((chartContent.Children[j] as TreeViewItem).Header as string == (toRemove.Items[i] as CheckBox).Content as string)
                        {
                            string input = (chartContent.Children[j] as TreeViewItem).Header.ToString();
                            Config.Charts.RemoveInput(chartName, input);
                            chartContent.Children.Remove((chartContent.Children[j] as TreeViewItem));
                            break;
                        }
                    }
                    toRemove.Items.RemoveAt(i);
                    i--;
                }
            }

            if(!((SolidColorBrush)Config.Charts.Charts[chartName].CanvasBackGround).Color.Equals(((SolidColorBrush)chartContent.Background).Color))
            {
                ((SolidColorBrush)Config.Charts.Charts[chartName].CanvasBackGround).Color = ((SolidColorBrush)chartContent.Background).Color;
                Config.Charts.UpdateChartColor(chartName, chartContent.Background);
                Debug.WriteLine("background is updated");
            }

            for(int i = 1; i < chartContent.Children.Count - 1; i++)
            {
                string input = (chartContent.Children[i] as TreeViewItem).Header.ToString();
                Brush ellipse = (((chartContent.Children[i] as TreeViewItem).Items[1] as StackPanel).Children[0] as Ellipse).Fill;
                Brush line = (((chartContent.Children[i] as TreeViewItem).Items[1] as StackPanel).Children[1] as Border).Background;

                if(Config.Charts.Charts[chartName].ChartInputs.Contains(input))
                {
                    int index = Config.Charts.Charts[chartName].ChartInputs.IndexOf(input);
                    if (!((SolidColorBrush)Config.Charts.Charts[chartName].Ellipses[index]).Color.Equals(((SolidColorBrush)ellipse).Color) &&
                        !((SolidColorBrush)Config.Charts.Charts[chartName].Lines[index]).Color.Equals(((SolidColorBrush)line).Color))
                    {
                        ((SolidColorBrush)Config.Charts.Charts[chartName].Ellipses[index]).Color = ((SolidColorBrush)ellipse).Color;
                        ((SolidColorBrush)Config.Charts.Charts[chartName].Lines[index]).Color = ((SolidColorBrush)line).Color;
                        Config.Charts.UpdateChartInputColor(chartName, input, ellipse);
                        Config.Charts.UpdateChartInputColor(chartName, input, line, 3);
                    }
                    else if(!((SolidColorBrush)Config.Charts.Charts[chartName].Ellipses[index]).Color.Equals(((SolidColorBrush)ellipse).Color))
                    {
                        ((SolidColorBrush)Config.Charts.Charts[chartName].Ellipses[index]).Color = ((SolidColorBrush)ellipse).Color;
                        Config.Charts.UpdateChartInputColor(chartName, input, ellipse);
                    }
                    else if(!((SolidColorBrush)Config.Charts.Charts[chartName].Lines[index]).Color.Equals(((SolidColorBrush)line).Color))
                    {
                        ((SolidColorBrush)Config.Charts.Charts[chartName].Lines[index]).Color = ((SolidColorBrush)line).Color;
                        Config.Charts.UpdateChartInputColor(chartName, input, line, 3);
                    }
                }
                else
                {
                    Config.Charts.AddInputToChart(chartName, input, ellipse, line);
                }
                
            }

            Debug.WriteLine(chartName);
        }

        private void Removables()
        {
            StackPanel remowabals = new StackPanel();
            foreach (string chart in Config.Charts.Charts.Keys)
            {
                remowabals.Children.Add(new CheckBox()
                {
                    IsChecked = false,
                    Content = chart
                });
            }
            Button remove = new Button() { Content = "Remove" };
            remove.Click += Remove_Click;
            remowabals.Children.Add(remove);

            RemoveCharts.Content = remowabals;
        }

        /* #################  ADD  ########################## */

        private void AddInputButton_Click(object sender, RoutedEventArgs e)
        {
            if (AddInputBox.Text != string.Empty)
            {
                /*if(!Config.Charts.Charts.ContainsKey(AddInputBox.Text))
                {
                    AddInputPanel.Children.Add(AddColorOptions(AddInputBox.Text));
                }*/
                bool add = true;
                foreach (TreeViewItem tree in AddInputPanel.Children)
                {
                    if (tree.Header.ToString() == AddInputBox.Text)
                    {
                        add = false;
                        break;
                    }
                }
                if (add)
                    AddInputPanel.Children.Add(AddColorOptions(AddInputBox.Text));
                AddInputBox.Text = "";
            }
        }

        private void On_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(sender is Slider)
            {
                if((sender as Slider).Parent is TreeViewItem)
                {
                    if(((sender as Slider).Parent as TreeViewItem).Parent is StackPanel)
                    {
                        ((SolidColorBrush)(((sender as Slider).Parent as TreeViewItem).Parent as StackPanel).Background).Color = Color.FromArgb(0xff, 
                            (byte)(((sender as Slider).Parent as TreeViewItem).Items[0] as Slider).Value,
                            (byte)(((sender as Slider).Parent as TreeViewItem).Items[1] as Slider).Value,
                            (byte)(((sender as Slider).Parent as TreeViewItem).Items[2] as Slider).Value);
                    }
                }
            }
        }

        private TreeViewItem AddColorOptions(string name)
        {
            TreeViewItem colorpalette = new TreeViewItem() { Header = name };

            colorpalette.Items.Add(new CheckBox() { Content = "Duplicate", IsChecked = false }); // index 0

            Brush brush = new SolidColorBrush(Color.FromArgb(0xff, 0x88, 0x88, 0x88));
            StackPanel dotedLine = new StackPanel() { Orientation = Orientation.Horizontal, Background = Brushes.Transparent};   // index 1
            dotedLine.Children.Add(new Ellipse() { Width = 5, Height = 5, Fill = brush });
            dotedLine.Children.Add(new Border() { Height = 2, Width = 90, Background = new SolidColorBrush(Color.FromArgb(0xff, 0x88, 0x88, 0x88)) }); // border as line
            dotedLine.Children.Add(new Ellipse() { Width = 5, Height = 5, Fill = brush });

            colorpalette.Items.Add(dotedLine);

            // index 2 => TextBlock Ellipse
            // index 3 => Slider Red
            // index 4 => Slider Green
            // index 5 => Slider Blue

            // index 6 => TextBlock Line
            // index 7 => Slider Red
            // index 8 => Slider Green
            // index 9 => Slider Blue

            for (int i = 0; i < 2; i++)
            {
                colorpalette.Items.Add((i == 0) ? new TextBlock() { Text = "Ellipse" } : new TextBlock() { Text = "Line" });

                for (int j = 0; j < 3; j++)
                {
                    Slider slider = new Slider() { Minimum = 0, Maximum = 255, Width = 100, Value = 0x88 };
                    slider.ValueChanged += Color_ValueChanged;
                    colorpalette.Items.Add(slider);
                }
            }

            return colorpalette;
        }

        private TreeViewItem AddColorOptions(string name, Brush ellipse, Brush line)
        {
            TreeViewItem colorpalette = new TreeViewItem() { Header = name };

            colorpalette.Items.Add(new CheckBox() { Content = "Duplicate", IsChecked = false }); // index 0

            //Brush brush = new SolidColorBrush(Color.FromArgb(0xff, 0x88, 0x88, 0x88));
            StackPanel dotedLine = new StackPanel() { Orientation = Orientation.Horizontal, Background = Brushes.Transparent };   // index 1
            dotedLine.Children.Add(new Ellipse() { Width = 5, Height = 5, Fill = ellipse });
            dotedLine.Children.Add(new Border() { Height = 2, Width = 90, Background = line/*new SolidColorBrush(Color.FromArgb(0xff, 0x88, 0x88, 0x88))*/ }); // border as line
            dotedLine.Children.Add(new Ellipse() { Width = 5, Height = 5, Fill = ellipse });

            colorpalette.Items.Add(dotedLine);

            for (int i = 0; i < 2; i++)
            {
                colorpalette.Items.Add((i == 0) ? new TextBlock() { Text = "Ellipse" } : new TextBlock() { Text = "Line" });

                Color color = ((SolidColorBrush)((i == 0) ? ellipse.GetAsFrozen() : line.GetAsFrozen())).Color;
                double[] toValue = new double[] { color.R, color.G, color.B };

                for (int j = 0; j < 3; j++)
                {
                    Slider slider = new Slider() { Minimum = 0, Maximum = 255, Width = 100, Value = toValue[j] };
                    slider.ValueChanged += Color_ValueChanged;
                    colorpalette.Items.Add(slider);
                }
            }

            return colorpalette;
        }

        private enum ColorSliders
        {
            EllipseRed = 3,
            EllipseGreen = 4,
            EllibseBlue = 5,

            LineRed = 7,
            LineGreen = 8,
            LineBlue = 9
        }

        private void Color_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            
            if((bool)(((sender as Slider).Parent as TreeViewItem).Items[0] as CheckBox).IsChecked)
            {
                switch(((sender as Slider).Parent as TreeViewItem).Items.IndexOf(sender))
                {
                    case 3:
                        (((sender as Slider).Parent as TreeViewItem).Items[7] as Slider).Value = (((sender as Slider).Parent as TreeViewItem).Items[3] as Slider).Value;
                        break;
                    case 4:
                        (((sender as Slider).Parent as TreeViewItem).Items[8] as Slider).Value = (((sender as Slider).Parent as TreeViewItem).Items[4] as Slider).Value;
                        break;
                    case 5:
                        (((sender as Slider).Parent as TreeViewItem).Items[9] as Slider).Value = (((sender as Slider).Parent as TreeViewItem).Items[5] as Slider).Value;
                        break;
                    case 7:
                        (((sender as Slider).Parent as TreeViewItem).Items[3] as Slider).Value = (((sender as Slider).Parent as TreeViewItem).Items[7] as Slider).Value;
                        break;
                    case 8:
                        (((sender as Slider).Parent as TreeViewItem).Items[4] as Slider).Value = (((sender as Slider).Parent as TreeViewItem).Items[8] as Slider).Value;
                        break;
                    case 9:
                        (((sender as Slider).Parent as TreeViewItem).Items[5] as Slider).Value = (((sender as Slider).Parent as TreeViewItem).Items[9] as Slider).Value;
                        break;
                    default:
                        break;
                }

            }

            /*      Ellipse     */
            ((SolidColorBrush)((((sender as Slider).Parent as TreeViewItem).Items[1] as StackPanel).Children[0] as Ellipse).Fill).Color = Color.FromArgb(0xff,
                (byte)(((sender as Slider).Parent as TreeViewItem).Items[3] as Slider).Value,
                (byte)(((sender as Slider).Parent as TreeViewItem).Items[4] as Slider).Value,
                (byte)(((sender as Slider).Parent as TreeViewItem).Items[5] as Slider).Value);

            /*      Line        */
            ((SolidColorBrush)((((sender as Slider).Parent as TreeViewItem).Items[1] as StackPanel).Children[1] as Border).Background).Color = Color.FromArgb(0xff,
                (byte)(((sender as Slider).Parent as TreeViewItem).Items[7] as Slider).Value,
                (byte)(((sender as Slider).Parent as TreeViewItem).Items[8] as Slider).Value,
                (byte)(((sender as Slider).Parent as TreeViewItem).Items[9] as Slider).Value);
        }

        private void AddChartButton_Click(object sender, RoutedEventArgs e)
        {
            string name = string.Empty;
            List<string> newImputs = new List<string>();
            List<Brush> ellipse = new List<Brush>();
            List<Brush> line = new List<Brush>();
            foreach (object obj in ((sender as Button).Parent as StackPanel).Children)
            {
                if (obj is TextBox && name == string.Empty)
                {
                    name = (obj as TextBox).Text;
                    (obj as TextBox).Text = "";
                }
                else if (obj is TreeViewItem)
                {
                    foreach(object item in (obj as TreeViewItem).Items)
                    {
                        if(item is StackPanel)
                        {
                            //newImputs = new List<string>();
                            //ellipse = new List<Brush>();
                            //line = new List<Brush>();

                            foreach (object child in (item as StackPanel).Children)
                            {
                                if(child is TreeViewItem)
                                {
                                    newImputs.Add((child as TreeViewItem).Header.ToString());
                                    ellipse.Add(new SolidColorBrush(
                                        Color.FromArgb(0xff,
                                        (byte)((child as TreeViewItem).Items[3] as Slider).Value,
                                        (byte)((child as TreeViewItem).Items[4] as Slider).Value,
                                        (byte)((child as TreeViewItem).Items[5] as Slider).Value)));

                                    line.Add(new SolidColorBrush(
                                        Color.FromArgb(0xff,
                                        (byte)((child as TreeViewItem).Items[7] as Slider).Value,
                                        (byte)((child as TreeViewItem).Items[8] as Slider).Value,
                                        (byte)((child as TreeViewItem).Items[9] as Slider).Value)));
                                }
                            }
                            
                            (item as StackPanel).Children.Clear();
                        }
                    }
                }
            }

            if(name != string.Empty)
            {
                if(!Config.Charts.Charts.ContainsKey(name))
                {
                    Config.Charts.AddChart(name, new SolidColorBrush(((SolidColorBrush)Frozen).Color), newImputs, ellipse, line);
                    AlterChart.Items.Add(CreateAlterChartViewItem(name, Config.Charts.Charts[name]));
                    (RemoveCharts.Content as StackPanel).Children.Insert((RemoveCharts.Content as StackPanel).Children.Count - 1, new CheckBox()
                    {
                        IsChecked = false,
                        Content = name
                    });
                }
            }

        }

        public void AddContent(string name, ChartConfigProfile chart)
        {
            AlterChart.Items.Add(CreateAlterChartViewItem(name, Config.Charts.Charts[name]));
            (RemoveCharts.Content as StackPanel).Children.Insert((RemoveCharts.Content as StackPanel).Children.Count - 1, new CheckBox()
            {
                IsChecked = false,
                Content = name
            });
        }
        /* #################  ADD END  ########################## */



        /* #################  Alter  ########################## */
        /* #################  Alter END  ########################## */



        /* #################  Remove  ########################## */

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            List<CheckBox> toRemove = new List<CheckBox>();
            foreach(var item in ((sender as Button).Parent as StackPanel).Children)
            {
                if(item is CheckBox)
                {
                    if((bool)(item as CheckBox).IsChecked)
                    {
                        toRemove.Add(item as CheckBox);
                    }
                }
            }

            foreach(var remove in toRemove)
            {
                ((sender as Button).Parent as StackPanel).Children.Remove(remove);
                Config.Charts.RemoveChart(remove.Content.ToString());
                TreeViewItem alter = FindCreatedAlterChartViewItem(remove.Content.ToString());
                if(AlterChart.Items.Contains(alter))
                {
                    AlterChart.Items.Remove(alter);
                }
            }
        }


        /* #################  Remove END  ########################## */
    }
}
