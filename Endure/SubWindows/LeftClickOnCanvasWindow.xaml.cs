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

namespace Endure.SubWindows
{
    /// <summary>
    /// Interaction logic for LeftClickOnCanvas.xaml
    /// </summary>
    public partial class LeftClickOnCanvasWindow : Window
    {
        List<(string, string)> text;
        public LeftClickOnCanvasWindow(string title, string msg, in List<string> inputNames, List<(string, string)> outputText)
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            this.Title = title;
            Date.Text = msg;
            Date.TextAlignment = TextAlignment.Center;
            Date.TextWrapping = TextWrapping.Wrap;

            Background = new SolidColorBrush(Color.FromArgb(0x80, 0x12, 0x41, 0x71));
            text = outputText;

            bool newRow = true;
            int rowCount = 0;
            foreach (string name in inputNames)
            {
                StackPanel input = new StackPanel()
                {
                    Children = {
                        CreateTextBlock(name),
                        CreateTextBox(name)
                    },
                    Margin = new Thickness(5)
                };
                Inputs.Children.Add(input);

                if(newRow)
                    rowCount++;

                newRow = !newRow;
            }

            this.MinHeight = this.MaxHeight = 130 + rowCount * 60;
            this.MinWidth = this.MaxWidth = 250;
        }

        private TextBox CreateTextBox(string name)
        {
            TextBox box = new TextBox
            {
                Width = 70,
                Height = 25,
                FontSize = 16,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            box.PreviewTextInput += TextBoxNumberValidation;

            return box;
        }

        private TextBlock CreateTextBlock(string name)
        {
            return new TextBlock
            {
                Text = name,
                FontSize = 14,
                FontWeight = FontWeights.DemiBold,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = new SolidColorBrush(Color.FromArgb(0xff, 0x62, 0xf1, 0x71))
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


        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            foreach(StackPanel panel in Inputs.Children)
            {
                text.Add(((panel.Children[0] as TextBlock).Text, (panel.Children[1] as TextBox).Text));
            }
            this.Close();
        }
    }
}
