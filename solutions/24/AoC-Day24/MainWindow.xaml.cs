using System.Drawing;
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
using static System.Net.Mime.MediaTypeNames;
using Point = System.Windows.Point;
using Rectangle = System.Windows.Shapes.Rectangle;
using AoC_Day24.Device;
using AoC_Day24.Visualization;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics;
using System.Xml.Linq;

namespace AoC_Day24
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly static int cellWidth = 60;
        private readonly static int cellHeight = 40;
        private readonly static int spacing = 10;
        private readonly FontFamily consolasFamily;
        private readonly SolidColorBrush backgroundBrush;
        private readonly SolidColorBrush elementBackgroundBrush;
        private readonly SolidColorBrush greenBrush;

        public MainWindow()
        {
            InitializeComponent();

            consolasFamily = new FontFamily("Consolas");
            backgroundBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#0f0f23");
            elementBackgroundBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#10101a");
            greenBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#00cc00");

            Width = spacing + (cellWidth + spacing) * 12;
            Height = Width;

            canvas.Width = spacing + (cellWidth + spacing) * 12;
            canvas.Height = spacing + 46 * (cellHeight + spacing) * 4;
            canvas.Background = backgroundBrush;

            var circuit = new Circuit();
            circuit.Import();

            foreach (var gate in circuit.gates)
            {
                DrawGate(canvas, gate);

                DrawConnection(canvas, gate.inputs[0], gate);
                DrawConnection(canvas, gate.inputs[1], gate);
                DrawConnection(canvas, gate, gate.output, gate.Suspicious);
            }

            foreach (var wire in circuit.wires.Values)
                DrawWire(canvas, wire, wire.suspicious);

            var answer = string.Empty;
            foreach (var wire in circuit.wires.Values)
                if (wire.suspicious)
                    answer += $"{wire.name},";

            DrawAnswer(canvas, $"Puzzle answer: {answer[..^1]}");
        }

        private void DrawAnswer(Canvas canvas, string answer)
        {
            var label = new TextBox
            {
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                FontFamily = consolasFamily,
                FontSize = cellHeight * 0.4,
                FontWeight = FontWeights.Light,
                Foreground = Brushes.Silver,
                Text = answer,
                Height = cellHeight,
                TextAlignment = TextAlignment.Left,
                VerticalContentAlignment = VerticalAlignment.Center
            };
            Canvas.SetLeft(label, spacing);
            Canvas.SetTop(label, spacing);
            canvas.Children.Add(label);
        }

        private void DrawWire(Canvas canvas, Wire wire, bool suspicious = false)
        {
            DrawWire(canvas, wire.name, wire.position.x, wire.position.y, wire.position.offset, suspicious);
        }

        private void DrawWire(Canvas canvas, string name, int xPos, int yPos, int offset, bool suspicious = false)
        {
            var rectangle = new Rectangle
            {
                Fill = elementBackgroundBrush,
                Stroke = suspicious ? Brushes.Red : Brushes.Silver,
                StrokeThickness = suspicious ? 1.5 : 1,
                RadiusX = cellHeight / 10,
                RadiusY = cellHeight / 10,
                Width = cellWidth,
                Height = cellHeight
            };
            Canvas.SetLeft(rectangle, CalculateLeft(xPos));
            Canvas.SetTop(rectangle, CalculateTop(yPos, offset));
            canvas.Children.Add(rectangle);

            var label = new TextBox
            {
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                FontFamily = consolasFamily,
                FontSize = cellHeight * 0.4,
                FontWeight = FontWeights.Light,
                Foreground = suspicious ? Brushes.Red : Brushes.Silver,
                Text = name,
                Width = cellWidth,
                Height = cellHeight,
                TextAlignment = TextAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center
            };
            Canvas.SetLeft(label, CalculateLeft(xPos));
            Canvas.SetTop(label, CalculateTop(yPos, offset));
            canvas.Children.Add(label);
        }

        private void DrawGate(Canvas canvas, Gate gate)
        {
            var opSymbol = gate.op switch
            {
                "AND" => "&&",
                "OR" => "||",
                _ => "!="
            };

            DrawGate(canvas, opSymbol, gate.position.x, gate.position.y, gate.position.offset);
        }

        private void DrawGate(Canvas canvas, string name, int xPos, int yPos, int offset)
        {
            var circle = new Ellipse
            {
                Fill = elementBackgroundBrush,
                Stroke = Brushes.Silver,
                Width = cellHeight,
                Height = cellHeight
            };
            Canvas.SetLeft(circle, CalculateLeft(xPos, true));
            Canvas.SetTop(circle, CalculateTop(yPos, offset));
            canvas.Children.Add(circle);

            var label = new TextBox
            {
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                FontFamily = consolasFamily,
                FontSize = cellHeight * 0.4,
                FontWeight = FontWeights.Light,
                Foreground = Brushes.Silver,
                Text = name,
                Width = cellWidth,
                Height = cellHeight,
                TextAlignment = TextAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center
            };
            Canvas.SetLeft(label, CalculateLeft(xPos));
            Canvas.SetTop(label, CalculateTop(yPos, offset));
            canvas.Children.Add(label);
        }

        private void DrawConnection(Canvas canvas, Element from, Element to, bool suspicious = false)
        {
            DrawConnection(canvas, from.position.x, from.position.y, from.position.offset, from is Gate, to.position.x, to.position.y, to.position.offset, to is Gate, suspicious);
        }

        private void DrawConnection(Canvas canvas, int fromX, int fromY, int fromOffset, bool fromRound, int toX, int toY, int toOffset, bool toRound, bool suspicious = false)
        {
            var x1 = CalculateLeft(fromX) + cellWidth / 2;
            var y1 = CalculateTop(fromY, fromOffset) + cellHeight / 2;
            var x2 = CalculateLeft(toX) + cellWidth / 2;
            var y2 = CalculateTop(toY, toOffset) + cellHeight / 2;

            var figure = new PathFigure();
            if (x1 < x2 && y1 < y2)
            {
                if (fromOffset == toOffset)
                {
                    y1 += cellHeight / 2;
                    x2 -= toRound ? cellHeight / 2 : cellWidth / 2;

                    figure.StartPoint = new Point(x1, y1);
                    figure.Segments.Add(
                        new BezierSegment(
                            new Point(x1, y1 + cellHeight),
                            new Point(x2 - cellWidth, y2),
                            new Point(x2, y2),
                            true));

                    DrawArrow(canvas, x2, y2, 1, suspicious);
                }
                else
                {
                    x1 += fromRound ? cellHeight / 2 : cellWidth / 2;
                    y2 -= cellHeight / 2;

                    figure.StartPoint = new Point(x1, y1);
                    figure.Segments.Add(
                        new BezierSegment(
                            new Point(x1 + cellWidth, y1),
                            new Point(x2, y2 - cellHeight),
                            new Point(x2, y2),
                            true));

                    DrawArrow(canvas, x2, y2, 2, suspicious);
                }
            }
            else if (x1 < x2 && y1 > y2)
            {
                if (toY == 0)
                {
                    y1 -= cellHeight / 2;
                    x2 -= toRound ? cellHeight / 2 : cellWidth / 2;

                    figure.StartPoint = new Point(x1, y1);
                    figure.Segments.Add(
                        new BezierSegment(
                            new Point(x1, y1 - cellHeight / 2),
                            new Point(x2 - cellWidth / 2, y2),
                            new Point(x2, y2),
                            true));

                    DrawArrow(canvas, x2, y2, 1, suspicious);
                }
                else
                {
                    x1 += fromRound ? cellHeight / 2 : cellWidth / 2;
                    y2 += cellHeight / 2;

                    figure.StartPoint = new Point(x1, y1);
                    figure.Segments.Add(
                        new BezierSegment(
                            new Point(x1 + cellWidth / 2, y1),
                            new Point(x2, y2 + cellHeight / 2),
                            new Point(x2, y2),
                            true));

                    DrawArrow(canvas, x2, y2, 0, suspicious);
                }
            }
            else if (x1 > x2 && y1 < y2)
            {
                y1 += cellHeight / 2;
                y2 -= cellHeight / 2;

                figure.StartPoint = new Point(x1, y1);
                figure.Segments.Add(
                    new BezierSegment(
                        new Point(x1 + spacing, y1 + (y2 - y1) * 0.75),
                        new Point(x2 - spacing, y2 - (y2 - y1) * 0.75),
                        new Point(x2, y2),
                        true));

                DrawArrow(canvas, x2, y2, 2, suspicious);
            }
            else
            {
                var direction = 0;
                if (x1 < x2)
                {
                    direction = 1;
                    x1 += fromRound ? cellHeight / 2 : cellWidth / 2;
                    x2 -= toRound ? cellHeight / 2 : cellWidth / 2;
                }
                else if (x1 > x2)
                {
                    direction = 3;
                    x1 -= fromRound ? cellHeight / 2 : cellWidth / 2;
                    x2 += toRound ? cellHeight / 2 : cellWidth / 2;
                }
                else if (y1 < y2)
                {
                    direction = 2;
                    y1 += cellHeight / 2;
                    y2 -= cellHeight / 2;
                }
                else if (y1 > y2)
                {
                    direction = 0;
                    y1 -= cellHeight / 2;
                    y2 += cellHeight / 2;
                }

                figure.StartPoint = new Point(x1, y1);
                figure.Segments.Add(
                    new LineSegment(
                        new Point(x2, y2),
                        true));

                DrawArrow(canvas, x2, y2, direction, suspicious);
            }

            var geometry = new PathGeometry();
            geometry.Figures.Add(figure);

            var path = new Path
            {
                Data = geometry,
                Stroke = suspicious ? Brushes.Red : greenBrush,
                StrokeThickness = suspicious ? 1.5 : 1
            };

            canvas.Children.Add(path);
        }

        private void DrawArrow(Canvas canvas, double x, double y, int direction, bool suspicious = false)
        {
            double x1, x3, y1, y3;
            switch(direction)
            {
                case 0:
                    x1 = x - spacing / 2;
                    x3 = x + spacing / 2;
                    y1 = y + spacing / 2;
                    y3 = y + spacing / 2;
                    break;
                case 1:
                    x1 = x - spacing / 2;
                    x3 = x - spacing / 2;
                    y1 = y - spacing / 2;
                    y3 = y + spacing / 2;
                    break;
                case 2:
                    x1 = x + spacing / 2;
                    x3 = x - spacing / 2;
                    y1 = y - spacing / 2;
                    y3 = y - spacing / 2;
                    break;
                default:
                    x1 = x + spacing / 2;
                    x3 = x + spacing / 2;
                    y1 = y + spacing / 2;
                    y3 = y - spacing / 2;
                    break;
            }

            var figure = new PathFigure
            {
                StartPoint = new Point(x1, y1)
            };

            figure.Segments.Add(
                new LineSegment(
                    new Point(x, y),
                    true));
            figure.Segments.Add(
                new LineSegment(
                    new Point(x3, y3),
                    true));

            var geometry = new PathGeometry();
            geometry.Figures.Add(figure);

            var path = new Path
            {
                Data = geometry,
                Stroke = suspicious ? Brushes.Red : greenBrush,
                StrokeThickness = suspicious ? 1.5 : 1
            };

            canvas.Children.Add(path);
        }

        private static double CalculateLeft(int xPos, bool circle = false)
        {
            var indent = 0d;
            if (circle)
                indent = (cellWidth - cellHeight) / 2;

            return spacing + ++xPos * (cellWidth + spacing) + indent;
        }

        private static double CalculateTop(int yPos, int offset)
        {
            var expandOffset = offset * (cellHeight + spacing) * 4;

            return spacing + expandOffset + yPos * (cellHeight + spacing);
        }
    }
}