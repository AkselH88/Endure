using Endure.SubWindows;
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

        public bool HandelNewInput(string[] date, string inputText, Canvas canvas)
        {
            if (inputText == string.Empty)
            {
                return false;
            }
            else if(inputText.Contains("."))
            {
                string[] input = inputText.Split(".");

                ellipses.Add(date, input, textOnCanvas.HorizontalText, textOnCanvas.HorizontalTextPositions, canvas);
            }
            else if(inputText.Contains(","))
            {
                string[] input = inputText.Split(",");

                ellipses.Add(date, input, textOnCanvas.HorizontalText, textOnCanvas.HorizontalTextPositions, canvas);
            }
            else
            {
                string[] input = { inputText, "00" };

                ellipses.Add(date, input, textOnCanvas.HorizontalText, textOnCanvas.HorizontalTextPositions, canvas);
            }

            if (ellipses.Update)
            {
                if (ellipses.changeMax)
                    common.CurrentMax = ellipses.CurrentMax;
                if (ellipses.changeMin)
                    common.CurrentMin = ellipses.CurrentMin;

                UpdateChartForeNewInput();
            }

            return true;
        }

        private void UpdateChartForeNewInput()
        {
            textOnCanvas.UpdateText();
            textOnCanvas.OnSizeChange();
            Lines.OnSizeChange(textOnCanvas.VertivalTextPosition, textOnCanvas.HorizontalTextPositions);
            ellipses.OnSizeChange(textOnCanvas.HorizontalText, textOnCanvas.HorizontalTextPositions);
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
            ellipses.ReDraw(textOnCanvas.HorizontalText, canvas);

            OnSizeChange(canvas);
        }

        public void OnSizeChange(Canvas canvas)
        {
            if (Initialized)
            {
                common.OnSizeChange(canvas);
                textOnCanvas.OnSizeChange();
                ellipses.OnSizeChange(textOnCanvas.HorizontalText, textOnCanvas.HorizontalTextPositions);

                Lines.OnSizeChange(textOnCanvas.VertivalTextPosition, textOnCanvas.HorizontalTextPositions);
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

        public void OnMoveForward(Canvas canvas)
        {
            ellipses.OnMove(textOnCanvas.OnMoveForward(canvas), true, textOnCanvas.HorizontalText, textOnCanvas.HorizontalTextPositions, canvas);
            if (ellipses.Update)
            {
                if (ellipses.changeMax)
                    common.CurrentMax = ellipses.CurrentMax;
                if (ellipses.changeMin)
                    common.CurrentMin = ellipses.CurrentMin;

                UpdateChartForeNewInput();
                //common.Update = false;
            }
        }

        public void OnMoveBackward(Canvas canvas)
        {
            ellipses.OnMove(textOnCanvas.OnMoveBackward(canvas), false, textOnCanvas.HorizontalText, textOnCanvas.HorizontalTextPositions, canvas);
            if (ellipses.Update)
            {
                if (ellipses.changeMax)
                    common.CurrentMax = ellipses.CurrentMax;
                if (ellipses.changeMin)
                    common.CurrentMin = ellipses.CurrentMin;

                UpdateChartForeNewInput();
                //common.Update = false;
            }
        }

        public void AddLines(Canvas canvas)
        {
            canvas.Children.Clear();
            Lines.AddLines(canvas, textOnCanvas.VertivalTextPosition, textOnCanvas.HorizontalTextPositions);
            
            DrawChartLines = Lines.DrawLines = true;
            Lines.ApplyHeightAndWidthLine(common, canvas);

            textOnCanvas.ReaplyTextOnCanvas(common, canvas);
            ellipses.ReDraw(textOnCanvas.HorizontalText, canvas);
        }

        public void RemoveLines(Canvas canvas)
        {
            Lines.RemoveLines(canvas);
            DrawChartLines = Lines.DrawLines = false;
        }
    }
}
