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

using System.Diagnostics;



namespace Endure
{
    
    public class Chart
    {
        readonly Common common = new Common();
        public StackPanelInput panelInput = new StackPanelInput();
        readonly TextOnCanvas textOnCanvas = new TextOnCanvas();
        readonly Dictionary<string, EllipsePoints> ellipses = new Dictionary<string, EllipsePoints>();
        readonly ChartLines Lines = new ChartLines();

        private bool Initialized = false;

        public bool IsInitialized { get { return Initialized; } }

        public bool DrawChartLines { private set; get; }

        public void Initialize(Canvas canvas)
        {
            if (!Initialized)
            {
                DrawChartLines = false;
                Lines.ApplyHeightAndWidthLine(common, canvas);
                textOnCanvas.Initialize(common, canvas);
                //ellipses.Initialize(common);
                Initialized = true;

                OnSizeChange(canvas);
            }
        }

        public void AddInput(string name)
        {
            if (!ellipses.ContainsKey(name))
            {
                panelInput.Add(name);
                ellipses.Add(name, new EllipsePoints());
                ellipses[name].Initialize(common);
            }
        }

        public List<string> Inputs
        {
            get
            {
                List<string> list = new List<string>();
                list.Add("Date");
                foreach (var set in panelInput.Elements)
                {
                    list.Add(set.Item1.Name);
                }
                return list;
            }
        }

        public bool HandelInputFromDB(List<List<string>> rows, Canvas canvas)
        {
            EvaluateUpdate evaluateUpdate = new EvaluateUpdate();

            foreach (List<string> row in rows)
            {
                string[] date = row[0].Split(".");

                int i = 1;
                foreach (var set in panelInput.Elements)
                {
                    if (row[i] != string.Empty)
                    {
                        if (row[i].Contains("."))
                        {
                            string[] input = row[i].Split(".");

                            ellipses[set.Item1.Text].Add(date, input, textOnCanvas.HorizontalText, textOnCanvas.HorizontalTextPositions, canvas);
                        }
                        else if (row[i].Contains(","))
                        {
                            string[] input = row[i].Split(",");

                            ellipses[set.Item1.Text].Add(date, input, textOnCanvas.HorizontalText, textOnCanvas.HorizontalTextPositions, canvas);
                        }
                        else
                        {
                            string[] input = { row[i], "00" };

                            ellipses[set.Item1.Text].Add(date, input, textOnCanvas.HorizontalText, textOnCanvas.HorizontalTextPositions, canvas);
                        }
                        evaluateUpdate.evaluate(ellipses[set.Item1.Text].Update, ellipses[set.Item1.Text].changeMax, ellipses[set.Item1.Text].changeMin,
                                                ellipses[set.Item1.Text].CurrentMax, ellipses[set.Item1.Text].CurrentMin);

                    }



                    i++;
                }
            }

            if (evaluateUpdate.update(common))
            {
                UpdateChartForeNewInput();
            }

            return true;
        }

        class EvaluateUpdate
        {
            int max = 0;
            int min = int.MaxValue;
            bool changeMax = false;
            bool changeMin = false;

            public void evaluate(bool update, bool chMax, bool chMin, int cuMax, int cuMin)
            {
                changeMax = changeMax || chMax;
                changeMin = changeMin || chMin;
                if (max < cuMax)
                {
                    max = cuMax;
                }

                if (min > cuMin)
                {
                    min = cuMin;
                }
            }

            public bool update(Common common)
            {
                if (changeMax)
                {
                    common.CurrentMax = max;
                }
                if (changeMin)
                {
                    common.CurrentMin = min;
                }

                return changeMax || changeMin;
            }
        }

        public bool HandelNewInput(string[] date, out List<string> inputs, out bool DateExists, Canvas canvas)
        {
            inputs = new List<string>();
            EvaluateUpdate evaluateUpdate = new EvaluateUpdate();
            bool OneOreMore = false;
            DateExists = false;

            foreach (var set in panelInput.Elements)
            {
                DateExists = DateExists || ellipses[set.Item1.Text].ContainsDate(date);
                if (set.Item2.Text != string.Empty)
                {
                    if (set.Item2.Text.Contains("."))
                    {
                        string[] input = set.Item2.Text.Split(".");
                        ellipses[set.Item1.Text].Add(date, input, textOnCanvas.HorizontalText, textOnCanvas.HorizontalTextPositions, canvas);
                    }
                    else if (set.Item2.Text.Contains(","))
                    {
                        string[] input = set.Item2.Text.Split(",");
                        ellipses[set.Item1.Text].Add(date, input, textOnCanvas.HorizontalText, textOnCanvas.HorizontalTextPositions, canvas);
                    }
                    else
                    {
                        string[] input = { set.Item2.Text, "00" };
                        ellipses[set.Item1.Text].Add(date, input, textOnCanvas.HorizontalText, textOnCanvas.HorizontalTextPositions, canvas);

                        string toDebug = $"{set.Item1.Text} : {set.Item2.Text}";
                        Debug.WriteLine(toDebug);
                    }
                    OneOreMore = true;
                }

                evaluateUpdate.evaluate(ellipses[set.Item1.Text].Update, ellipses[set.Item1.Text].changeMax, ellipses[set.Item1.Text].changeMin,
                                            ellipses[set.Item1.Text].CurrentMax, ellipses[set.Item1.Text].CurrentMin);

                inputs.Add(set.Item2.Text);
                set.Item2.Text = "";
            }

            if(evaluateUpdate.update(common))
            {
                UpdateChartForeNewInput();
            }

            return OneOreMore;
        }

        public bool HandelNewInput(string[] date, List<(string, string)> inputs, out bool DateExists, Canvas canvas)
        {
            EvaluateUpdate evaluateUpdate = new EvaluateUpdate();
            bool OneOreMore = false;
            DateExists = false;

            foreach (var set in inputs)
            {
                DateExists = DateExists || ellipses[set.Item1].ContainsDate(date);
                if (set.Item2 != string.Empty)
                {
                    if (set.Item2.Contains("."))
                    {
                        string[] input = set.Item2.Split(".");
                        ellipses[set.Item1].Add(date, input, textOnCanvas.HorizontalText, textOnCanvas.HorizontalTextPositions, canvas);
                    }
                    else if (set.Item2.Contains(","))
                    {
                        string[] input = set.Item2.Split(",");
                        ellipses[set.Item1].Add(date, input, textOnCanvas.HorizontalText, textOnCanvas.HorizontalTextPositions, canvas);
                    }
                    else
                    {
                        string[] input = { set.Item2, "00" };
                        ellipses[set.Item1].Add(date, input, textOnCanvas.HorizontalText, textOnCanvas.HorizontalTextPositions, canvas);

                        string toDebug = $"{set.Item1} : {set.Item2}";
                        Debug.WriteLine(toDebug);
                    }
                    OneOreMore = true;
                }

                evaluateUpdate.evaluate(ellipses[set.Item1].Update, ellipses[set.Item1].changeMax, ellipses[set.Item1].changeMin,
                                            ellipses[set.Item1].CurrentMax, ellipses[set.Item1].CurrentMin);
            }

            if (evaluateUpdate.update(common))
            {
                UpdateChartForeNewInput();
            }

            return OneOreMore;
        }

        private void UpdateChartForeNewInput()
        {
            textOnCanvas.UpdateText();
            textOnCanvas.OnSizeChange();
            Lines.OnSizeChange(textOnCanvas.VertivalTextPosition, textOnCanvas.HorizontalTextPositions);

            foreach (var set in panelInput.Elements)
            {
                ellipses[set.Item1.Text].OnSizeChange(textOnCanvas.HorizontalText, textOnCanvas.HorizontalTextPositions);
            }
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
            foreach (var set in panelInput.Elements)
            {
                ellipses[set.Item1.Text].ReDraw(textOnCanvas.HorizontalText, canvas);
            }
            OnSizeChange(canvas);
        }

        public void OnSizeChange(Canvas canvas)
        {
            if (Initialized)
            {
                common.OnSizeChange(canvas);
                textOnCanvas.OnSizeChange();
                foreach (var set in panelInput.Elements)
                {
                    ellipses[set.Item1.Text].OnSizeChange(textOnCanvas.HorizontalText, textOnCanvas.HorizontalTextPositions);
                }
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

        public bool FindDate(double position, out string date)
        {
            if(position > common.VerticalOffset)
            {
                int i = (int)(position / textOnCanvas.HorizontalPixelSeperator);
                date = textOnCanvas.HorizontalText[i];
                return true;
            }

            date = string.Empty;
            return false;
        }

        public void OnMoveForward(Canvas canvas)
        {
            EvaluateUpdate evaluateUpdate = new EvaluateUpdate();
            string[] moveText = textOnCanvas.OnMoveForward(canvas);

            foreach (var set in panelInput.Elements)
            {
                ellipses[set.Item1.Text].OnMove(moveText, true, textOnCanvas.HorizontalText, textOnCanvas.HorizontalTextPositions, canvas);
                evaluateUpdate.evaluate(ellipses[set.Item1.Text].Update, ellipses[set.Item1.Text].changeMax, ellipses[set.Item1.Text].changeMin,
                                        ellipses[set.Item1.Text].CurrentMax, ellipses[set.Item1.Text].CurrentMin);
            }

            if (evaluateUpdate.update(common))
            {
                UpdateChartForeNewInput();
            }
        }

        public void OnMoveBackward(Canvas canvas)
        {
            EvaluateUpdate evaluateUpdate = new EvaluateUpdate();
            string[] moveText = textOnCanvas.OnMoveBackward(canvas);

            foreach (var set in panelInput.Elements)
            {
                ellipses[set.Item1.Text].OnMove(moveText, false, textOnCanvas.HorizontalText, textOnCanvas.HorizontalTextPositions, canvas);
                evaluateUpdate.evaluate(ellipses[set.Item1.Text].Update, ellipses[set.Item1.Text].changeMax, ellipses[set.Item1.Text].changeMin,
                                        ellipses[set.Item1.Text].CurrentMax, ellipses[set.Item1.Text].CurrentMin);
            }

            if (evaluateUpdate.update(common))
            {
                UpdateChartForeNewInput();
            }
        }

        public void AddLines(Canvas canvas)
        {
            canvas.Children.Clear();
            Lines.AddLines(canvas, textOnCanvas.VertivalTextPosition, textOnCanvas.HorizontalTextPositions);
            
            DrawChartLines = Lines.DrawLines = true;
            Lines.ApplyHeightAndWidthLine(common, canvas);

            textOnCanvas.ReaplyTextOnCanvas(common, canvas);
            foreach (var set in panelInput.Elements)
            {
                ellipses[set.Item1.Text].ReDraw(textOnCanvas.HorizontalText, canvas);
            }
        }

        public void RemoveLines(Canvas canvas)
        {
            Lines.RemoveLines(canvas);
            DrawChartLines = Lines.DrawLines = false;
        }
    }
}
