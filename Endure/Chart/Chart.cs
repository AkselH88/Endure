using System;
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
    public class Chart
    {
        readonly Common common = new Common();
        readonly TextOnCanvas textOnCanvas = new TextOnCanvas();
        readonly EllipsePoints ellipses = new EllipsePoints();
        readonly ChartLines Lines = new ChartLines();

        private bool Initialized = false;

        public bool IsInitialized { get { return Initialized; } }

        public bool DrawChartLines { private set; get; }

        public void Initialize(Canvas canvas)
        {
            if(!Initialized)
            {
                DrawChartLines = false;
                Lines.ApplyHeightAndWidthLine(common, canvas);
                textOnCanvas.Initialize(common, canvas);
                ellipses.Initialize(common);
                Initialized = true;

                OnSizeChange(canvas);
            }
        }

        public void HandelNewInput(string[] date, string inputText, Canvas canvas)
        {
            bool dot = inputText.Contains(".");
            bool comma = inputText.Contains(",");

            if (dot && comma)
            {
                //handle error massage
            }
            else if(dot)
            {
                string[] input = inputText.Split(".");

                if (IsNewCommenMax(input[0]))
                {
                    textOnCanvas.UpdateText();
                    ellipses.OnSizeChange(textOnCanvas.HorizontalTextPositions);
                }

                ellipses.Add(date, input, textOnCanvas.HorizontalTextPositions, canvas);
            }
            else if(comma)
            {
                string[] input = inputText.Split(",");
                if (IsNewCommenMax(input[0]))
                {
                    textOnCanvas.UpdateText();
                    ellipses.OnSizeChange(textOnCanvas.HorizontalTextPositions);
                }

                ellipses.Add(date, input, textOnCanvas.HorizontalTextPositions, canvas);
            }
            else
            {
                string[] input = { inputText, "00" };
                if (IsNewCommenMax(inputText))
                {
                    textOnCanvas.UpdateText();
                    ellipses.OnSizeChange(textOnCanvas.HorizontalTextPositions);
                }

                ellipses.Add(date, input, textOnCanvas.HorizontalTextPositions, canvas);
            }
        }

        private bool IsNewCommenMax(string number)
        {
            if (int.Parse(number) > common.CurrentMax)
            {
                switch (number.Length)
                {
                    case 1:
                        common.CurrentMax = 10;
                        break;
                    case 2:
                        common.CurrentMax = (int.Parse($"{number.ElementAt(0)}") + 1) * 10;
                        break;
                    case 3:
                        common.CurrentMax = int.Parse($"{number.ElementAt(0)}") * 100 + (int.Parse($"{number.ElementAt(1)}") + 1) * 10;
                        break;
                    case 4:
                        common.CurrentMax = int.Parse($"{number.ElementAt(0)}") * 1000 + (int.Parse($"{number.ElementAt(1)}") + 1) * 100;
                        break;
                    case 5:
                        common.CurrentMax = int.Parse($"{number.ElementAt(0)}") * 10000 + (int.Parse($"{number.ElementAt(1)}") + 1) * 1000;
                        break;
                }

                return true;
            }
            return false;
        }

        public void RemoveFromCanvas(Canvas canvas)
        {
            canvas.Children.Clear();
        }

        public void DrawOnCanvas(Canvas canvas)
        {
            if (DrawChartLines)
            {
                Lines.ReApplyLines(canvas);
            }
            Lines.ApplyHeightAndWidthLine(common, canvas);
            textOnCanvas.ReaplyTextOnCanvas(common, canvas);
            ellipses.ReDraw(textOnCanvas.HorizontalTextPositions, canvas);

            OnSizeChange(canvas);
        }

        public void OnSizeChange(Canvas canvas)
        {
            if (Initialized)
            {
                common.OnSizeChange(canvas);
                textOnCanvas.OnSizeChange();
                ellipses.OnSizeChange(textOnCanvas.HorizontalTextPositions);

                Lines.OnSizeChange(textOnCanvas.VertivalTextPosition, textOnCanvas.HorizontalTextPositions.Values.ToList());
            }
        }

        public double MinHeight
        {
            get { return textOnCanvas.MinHeight(); }
        }

        public double MinWidth
        {
            get { return textOnCanvas.MinWidth(); }
        }

        public void OnMoveBackward(Canvas canvas)
        {
            //textOnCanvas.OnMoveBackward(ellipses, canvas);
            ellipses.OnMove(textOnCanvas.OnMoveBackward(canvas), textOnCanvas.HorizontalTextPositions, canvas);
        }

        public void OnMoveForward(Canvas canvas)
        {
            //textOnCanvas.OnMoveForward(ellipses, canvas);
            ellipses.OnMove(textOnCanvas.OnMoveForward(canvas), textOnCanvas.HorizontalTextPositions, canvas);
        }

        public void AddLines(Canvas canvas)
        {
            canvas.Children.Clear();
            Lines.AddLines(canvas, textOnCanvas.VertivalTextPosition, textOnCanvas.HorizontalTextPositions.Values.ToList());
            
            DrawChartLines = Lines.DrawLines = true;
            Lines.ApplyHeightAndWidthLine(common, canvas);

            textOnCanvas.ReaplyTextOnCanvas(common, canvas);
            ellipses.ReDraw(textOnCanvas.HorizontalTextPositions, canvas);
        }

        public void RemoveLines(Canvas canvas)
        {
            Lines.RemoveLines(canvas);
            DrawChartLines = Lines.DrawLines = false;
        }
    }
}
