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

namespace Endure
{
    public class StackPanelInput
    {
        public List<(TextBlock, TextBox)> Elements { get; private set; }
        public List<string> ElementNames { get; private set; }
        public StackPanelInput()
        {
            Elements = new List<(TextBlock, TextBox)>();
            ElementNames = new List<string>();
        }

        public void Add(string name)
        {
            Elements.Add((CreateTextBlock(name), CreateTextBox(name)));
            ElementNames.Add(name);
        }

        public void Remove(TextBlock block, TextBox box)
        {
            Elements.Remove((block, box));
            ElementNames.Remove(box.Name);
        }

        private TextBox CreateTextBox(string name)
        {
            TextBox box = new TextBox
            {
                Name = name,
                Width = 70,
                Height = 30,
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
