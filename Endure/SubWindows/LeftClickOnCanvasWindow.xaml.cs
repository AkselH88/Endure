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
        List<TextBox> boxes;
        public LeftClickOnCanvasWindow(string title, string msg, in List<string> inputNames, List<(string, string)> outputText)
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            this.Title = title;
            Date.Text = msg;
            Date.TextAlignment = TextAlignment.Center;
            Date.TextWrapping = TextWrapping.Wrap;

            text = outputText;

            boxes = new List<TextBox>();
            bool switching = true;
            foreach(string name in inputNames)
            {
                TextBox tempbox = CreateTextBox(name);
                if (switching)
                {
                    Left.Children.Add(CreateTextBlock(name));
                    Left.Children.Add(tempbox);
                }
                else
                {
                    Right.Children.Add(CreateTextBlock(name));
                    Right.Children.Add(tempbox);
                }
                switching = !switching;
                boxes.Add(tempbox);
            }
            this.MinHeight = this.MaxHeight = 130 + (inputNames.Count / 2) * 60;
            this.MinWidth = this.MaxWidth = 250;
        }

        private TextBox CreateTextBox(string name)
        {
            TextBox box = new TextBox
            {
                Name = name,
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
                Name = name,
                Text = name,
                FontSize = 14,
                FontWeight = FontWeights.DemiBold,
                VerticalAlignment = VerticalAlignment.Top,
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


        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            foreach(TextBox box in boxes)
            {
                text.Add((box.Name, box.Text));
            }
            this.Close();
        }
    }
}
