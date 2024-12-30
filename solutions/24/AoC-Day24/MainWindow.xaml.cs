﻿using System.Drawing;
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
using System.Security.AccessControl;
using System.Windows.Media.Animation;
using System.Net.WebSockets;

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
        private readonly SolidColorBrush darkGreenBrush;
        private readonly SolidColorBrush goldBrush;

        public Dictionary<string, Path> connections;
        public Dictionary<string, TextBox> bits;
        public Circuit circuit;

        public Storyboard storyboard;

        public MainWindow()
        {
            InitializeComponent();

            consolasFamily = new FontFamily("Consolas");
            backgroundBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#0f0f23");
            elementBackgroundBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#10101a");
            greenBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#00cc00");
            darkGreenBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#009900");
            goldBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#ffff66");

            StyleButton(buttonSimulate);
            StyleButton(buttonRepair);

            MinWidth = spacing + (cellWidth + spacing) * 12;
            MaxWidth = MinWidth;
            Width = MinWidth;
            Height = Width;

            canvas.Width = spacing + (cellWidth + spacing) * 12;
            canvas.Height = spacing + 45.5 * (cellHeight + spacing) * 4;
            canvas.Background = backgroundBrush;

            circuit = new Circuit();
            circuit.Import();

            DrawCircuit();

            var answer = string.Empty;
            foreach (var wire in circuit.wires.Values)
                if (wire.suspicious)
                    answer += $"{wire.name},";

            answerLabel.FontFamily = consolasFamily;
            answerLabel.FontSize = cellHeight * 0.4;
            answerLabel.Content = $"Puzzle answer: {answer[..^1]}";
        }

        private void StyleButton(Button button)
        {
            button.Background = Brushes.Transparent;
            button.FontFamily = consolasFamily;
            button.FontSize = cellHeight * 0.4;
            button.Foreground = darkGreenBrush;
            button.BorderThickness = new Thickness(0);
        }

        private void DrawCircuit()
        {
            connections = [];
            bits = [];

            foreach (var gate in circuit.gates)
            {
                DrawConnection(gate.inputs[0], gate, $"{gate.inputs[0].name}{gate.op}");
                DrawConnection(gate.inputs[1], gate, $"{gate.inputs[1].name}{gate.op}");
                DrawConnection(gate, gate.output, $"{gate.op}{gate.output.name}", gate.suspicious);
            }

            storyboard = new Storyboard();

            foreach (var connection in connections)
                AnimateConnection(connection);

            var lastPath = connections.Values.Last();
            lastPath.Loaded += delegate (object sender, RoutedEventArgs e)
            {
                storyboard.Begin(this);
            };

            foreach (var gate in circuit.gates)
                DrawGate(gate);

            foreach (var wire in circuit.wires.Values)
                DrawWire(wire);
        }

        private void AnimateConnection(KeyValuePair<string, Path> connection)
        {
            var wireName = connection.Key[..3];

            if (connection.Key.StartsWith("AND") ||
                connection.Key.StartsWith("OR") ||
                connection.Key.StartsWith("XOR"))
                wireName = connection.Key[^3..];

            Wire wire = new(string.Empty, null);
            foreach (var wireOjbect in circuit.wires.Values)
                if (wireOjbect.name.Equals(wireName))
                    wire = wireOjbect;

            var bit = new TextBox
            {
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                FontFamily = consolasFamily,
                FontSize = cellHeight * 0.4,
                FontWeight = FontWeights.Light,
                Foreground = wire.influenced ? Brushes.Red : goldBrush,
                TextAlignment = TextAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,

            };
            bits.Add(connection.Key, bit);
            canvas.Children.Add(bit);

            if (wire != null && wire.value.HasValue)
                bit.Text = wire.value.Value ? "1" : "0";

            var animatedMatrixTransform = new MatrixTransform();
            bit.RenderTransform = animatedMatrixTransform;
            RegisterName(connection.Key, animatedMatrixTransform);

            var geometry = (PathGeometry)connection.Value.Data;
            var start = geometry.Figures[0].StartPoint;
            var segment = geometry.Figures[0].Segments.Last();

            Point end;
            if (segment is LineSegment)
                end = ((LineSegment)segment).Point;
            else
                end = ((BezierSegment)segment).Point3;

            var length = Math.Sqrt(Math.Pow(end.X - start.X, 2) + Math.Pow(end.Y - start.Y, 2));

            var animationPath = new PathGeometry();
            animationPath.Figures.Add(geometry.Figures[0]);

            var animation = new MatrixAnimationUsingPath()
            {
                PathGeometry = animationPath,
                Duration = new Duration(TimeSpan.FromSeconds(length / cellWidth)),
                RepeatBehavior = RepeatBehavior.Forever
            };

            Storyboard.SetTargetName(animation, connection.Key);
            Storyboard.SetTargetProperty(animation, new PropertyPath(MatrixTransform.MatrixProperty));

            storyboard.Children.Add(animation);
        }

        private void UpdateConnectionValue(string connectionKey)
        {
            var wireName = connectionKey[..3];

            if (connectionKey.StartsWith("AND") ||
                connectionKey.StartsWith("OR") ||
                connectionKey.StartsWith("XOR"))
                wireName = connectionKey[^3..];

            Wire wire = new(string.Empty, null);
            foreach (var wireOjbect in circuit.wires.Values)
                if (wireOjbect.name.Equals(wireName))
                    wire = wireOjbect;

            if (wire != null && wire.value.HasValue)
                bits[connectionKey].Text = wire.value.Value ? "1" : "0";
        }

        internal async void Simulate(object sender, RoutedEventArgs e)
        {
            foreach (var wire in circuit.wires.Values)
                wire.ResetValue();

            foreach (var wire in circuit.wires.Values)
                if (wire.name.StartsWith('x'))
                    await Process(wire);
        }

        internal async void Repair(object sender, RoutedEventArgs e)
        {
            storyboard.Stop();

            foreach (var connection in connections)
                UnregisterName(connection.Key);

            canvas.Children.Clear();

            circuit.RepairCrossedWires();

            DrawCircuit();
        }

        internal async Task Process(Wire wire)
        {
            foreach (var gate in circuit.gates)
            {
                if (!gate.ready && (gate.inputs[0] == wire || gate.inputs[1] == wire) && gate.inputs[0].value.HasValue && gate.inputs[1].value.HasValue)
                {
                    gate.Process();

                    UpdateConnectionValue($"{gate.inputs[0].name}{gate.op}");
                    UpdateConnectionValue($"{gate.inputs[1].name}{gate.op}");
                    UpdateConnectionValue($"{gate.op}{gate.output.name}");

                    await Task.Delay(500);

                    HighlightEndWires(gate.output);

                    MarkIfInfluenced(gate.inputs[0]);
                    MarkIfInfluenced(gate.inputs[1]);
                    MarkIfInfluenced(gate.output);

                    await Process(gate.output);
                }
            }
        }

        private void MarkIfInfluenced(Wire wire, bool input = false)
        {
            if (wire.influenced)
            {
                if (!wire.suspicious)
                    ((Rectangle)wire.uiElement).Stroke = Brushes.Orchid;

                foreach (var connection in connections)
                    if ((connection.Key.Contains(wire.name) && !wire.suspicious) ||
                        (connection.Key.StartsWith(wire.name) && wire.suspicious))
                        ((Path)connection.Value).Stroke = Brushes.Orchid;
            }
        }

        private void HighlightEndWires(Wire wire)
        {
            if (wire.name.StartsWith('z') && !wire.suspicious)
            {
                ((Rectangle)wire.uiElement).Stroke = Brushes.White;
                ((Rectangle)wire.uiElement).StrokeThickness = 1.5;
            }
        }

        private void DrawWire(Wire wire)
        {
            wire.uiElement = DrawWire(wire.name, wire.position.x, wire.position.y, wire.position.offset, wire.suspicious);
        }

        private UIElement DrawWire(string name, int xPos, int yPos, int offset, bool suspicious)
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

            return rectangle;
        }

        private void DrawGate(Gate gate)
        {
            var opSymbol = gate.op switch
            {
                "AND" => "&&",
                "OR" => "||",
                _ => "!="
            };

            gate.uiElement = DrawGate(opSymbol, gate.position.x, gate.position.y, gate.position.offset);
        }

        private UIElement DrawGate(string name, int xPos, int yPos, int offset)
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

            return circle;
        }

        private void DrawConnection(Element from, Element to, string name, bool suspicious = false)
        {
            var path = DrawConnection(from.position.x, from.position.y, from.position.offset, from is Gate, to.position.x, to.position.y, to.position.offset, to is Gate, suspicious);
            connections.Add(name, path);
        }

        private Path DrawConnection(int fromX, int fromY, int fromOffset, bool fromRound, int toX, int toY, int toOffset, bool toRound, bool suspicious = false)
        {
            var x1 = CalculateLeft(fromX) + cellWidth / 2;
            var y1 = CalculateTop(fromY, fromOffset) + cellHeight / 2;
            var x2 = CalculateLeft(toX) + cellWidth / 2;
            var y2 = CalculateTop(toY, toOffset) + cellHeight / 2;

            var figure = new PathFigure();
            PathFigure arrowFigure;
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

                    arrowFigure = DrawArrow(x2, y2, 1, suspicious);
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

                    arrowFigure = DrawArrow(x2, y2, 2, suspicious);
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

                    arrowFigure = DrawArrow(x2, y2, 1, suspicious);
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

                    arrowFigure = DrawArrow(x2, y2, 0, suspicious);
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

                arrowFigure = DrawArrow(x2, y2, 2, suspicious);
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

                arrowFigure = DrawArrow(x2, y2, direction, suspicious);
            }

            var geometry = new PathGeometry();
            geometry.Figures.Add(figure);
            geometry.Figures.Add(arrowFigure);
            geometry.Freeze();

            var path = new Path
            {
                Data = geometry,
                Name = "",
                Stroke = suspicious ? Brushes.Red : greenBrush,
                StrokeThickness = suspicious ? 1.5 : 1
            };

            canvas.Children.Add(path);

            return path;
        }

        private PathFigure DrawArrow(double x, double y, int direction, bool suspicious = false)
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

            return figure;
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