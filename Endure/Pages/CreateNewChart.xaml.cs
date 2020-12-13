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

namespace Endure.Pages
{
    /// <summary>
    /// Interaction logic for CreateNewChart.xaml
    /// </summary>
    public partial class CreateNewChart : Page
    {
        public CreateNewChart()
        {
            InitializeComponent();

            var brush = ChartBorder.Background.GetAsFrozen();

            ChartR.Value = ((SolidColorBrush)brush).Color.R;
            ChartG.Value = ((SolidColorBrush)brush).Color.G;
            ChartB.Value = ((SolidColorBrush)brush).Color.B;
        }

        private TreeViewItem NewTreeItem(string name)
        {
            TreeViewItem item = new TreeViewItem();

            item.Header = name;
            item.Items.Add(new TextBox() { Text = "Ellipse" });
            item.Items.Add(NewInputSlider());
            item.Items.Add(NewInputSlider());
            item.Items.Add(NewInputSlider());
            item.Items.Add(new TextBox() { Text = "Line" });
            item.Items.Add(NewInputSlider());
            item.Items.Add(NewInputSlider());
            item.Items.Add(NewInputSlider());

            return item;
        }

        private Slider NewInputSlider()
        {
            Slider slider = new Slider()
            {
                Width = 100,
                Minimum = 0,
                Maximum = 255,
                Value = 0x88
            };

            slider.ValueChanged += Input_Slider_Value_Change;

            return slider;
        }

        private void Input_Slider_Value_Change(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int i = (((sender as Slider).Parent as TreeViewItem).Parent as TreeView).Items.IndexOf((sender as Slider).Parent as TreeViewItem);
            int j = ((sender as Slider).Parent as TreeViewItem).Items.IndexOf(sender);
            
            if(j < 4)
            {
                ((SolidColorBrush)(((VisualInput.Children[i] as StackPanel).Children[1] as StackPanel).Children[0] as Ellipse).Fill).Color = Color.FromArgb(0xff,
                                (byte)(((sender as Slider).Parent as TreeViewItem).Items[1] as Slider).Value,
                                (byte)(((sender as Slider).Parent as TreeViewItem).Items[2] as Slider).Value,
                                (byte)(((sender as Slider).Parent as TreeViewItem).Items[3] as Slider).Value);
            }
            else
            {
                ((SolidColorBrush)(((VisualInput.Children[i] as StackPanel).Children[1] as StackPanel).Children[1] as Border).Background).Color = Color.FromArgb(0xff,
                                (byte)(((sender as Slider).Parent as TreeViewItem).Items[5] as Slider).Value,
                                (byte)(((sender as Slider).Parent as TreeViewItem).Items[6] as Slider).Value,
                                (byte)(((sender as Slider).Parent as TreeViewItem).Items[7] as Slider).Value);
            }
            
        }

        private StackPanel WrapTheStack(string name)
        {
            StackPanel panel = new StackPanel();

            panel.Children.Add(new TextBlock() { Text = name, HorizontalAlignment = HorizontalAlignment.Center });

            Brush brush = new SolidColorBrush(Color.FromArgb(0xff, 0x88, 0x88, 0x88));
            StackPanel dotedLine = new StackPanel() { Orientation = Orientation.Horizontal, Background = Brushes.Transparent };   // index 1
            dotedLine.Children.Add(new Ellipse() { Width = 5, Height = 5, Fill = brush });
            dotedLine.Children.Add(new Border() { Height = 2, Width = 90, Background = new SolidColorBrush(Color.FromArgb(0xff, 0x88, 0x88, 0x88)) }); // border as line
            dotedLine.Children.Add(new Ellipse() { Width = 5, Height = 5, Fill = brush });

            panel.Children.Add(dotedLine);

            return panel;
        }

        private void AddToInputTree(string name)
        {
            VisualInput.Children.Add(WrapTheStack(name));
            InputTree.Items.Add(NewTreeItem(name));
        }

        private void Button_Click_Add_Input(object sender, RoutedEventArgs e)
        {
            if(NewInput.Text != string.Empty)
            {
                AddToInputTree(NewInput.Text);
                NewInput.Text = "";
            }
        }

        private void Button_Click_Save_Chart(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Chart_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ((SolidColorBrush)ChartBorder.Background).Color = Color.FromArgb(0xff, (byte)ChartR.Value, (byte)ChartG.Value, (byte)ChartB.Value);
        }
    }
}
