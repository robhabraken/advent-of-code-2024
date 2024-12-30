using AoC_Day24.Device;
using AoC_Day24.Visualization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Label = System.Windows.Controls.Label;
using Point = System.Windows.Point;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace AoC_Day24
{
    public partial class MainWindow : Window
    {

        private readonly FontFamily consolasFamily;
        private readonly SolidColorBrush backgroundBrush;
        private readonly SolidColorBrush elementBackgroundBrush;
        private readonly SolidColorBrush greenBrush;
        private readonly SolidColorBrush darkGreenBrush;
        private readonly SolidColorBrush goldBrush;

        private readonly static int cellWidth = 60;
        private readonly static int cellHeight = 40;
        private readonly static int spacing = 10;

        public Circuit circuit;

        public Dictionary<string, Path> connections;
        public Dictionary<string, TextBox> bits;

        public Storyboard storyboard;

        public MainWindow()
        {
            InitializeComponent();

            consolasFamily = new FontFamily("Consolas");
            backgroundBrush = BrushFromHex("#0f0f23");
            elementBackgroundBrush = BrushFromHex("#10101a");
            greenBrush = BrushFromHex("#00cc00");
            darkGreenBrush = BrushFromHex("#009900");
            goldBrush = BrushFromHex("#ffff66");

            StyleLabel(answerLabel);
            StyleButton(buttonAnimate);
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

            answerLabel.Content = $"Puzzle answer: {answer[..^1]}";
        }

        private static SolidColorBrush BrushFromHex(string hex)
        {
            var brush = new BrushConverter().ConvertFrom(hex);
            return brush != null ? (SolidColorBrush)brush : Brushes.White;
        }

        private void StyleButton(Button button)
        {
            button.Background = Brushes.Transparent;
            button.FontFamily = consolasFamily;
            button.FontSize = cellHeight * 0.4;
            button.Foreground = darkGreenBrush;
            button.BorderThickness = new Thickness(0);
        }

        private void StyleLabel(Label label)
        {
            label.FontFamily = consolasFamily;
            label.FontSize = cellHeight * 0.4;
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

            Wire? wire = null;
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
                Foreground = wire != null && wire.influenced ? Brushes.Red : goldBrush,
                TextAlignment = TextAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Visibility = Visibility.Collapsed
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
            var segment = geometry.Figures[0].Segments.Last(); // only use line itself, not the arrow

            Point end;
            if (segment is LineSegment line)
                end = line.Point;
            else
                end = ((BezierSegment)segment).Point3;

            var pathLength = Math.Sqrt(Math.Pow(end.X - start.X, 2) + Math.Pow(end.Y - start.Y, 2));

            var animationPath = new PathGeometry();
            animationPath.Figures.Add(geometry.Figures[0]);

            var animation = new MatrixAnimationUsingPath()
            {
                PathGeometry = animationPath,
                Duration = new Duration(TimeSpan.FromSeconds(pathLength / cellWidth)),
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

            Wire? wire = null;
            foreach (var wireOjbect in circuit.wires.Values)
                if (wireOjbect.name.Equals(wireName))
                    wire = wireOjbect;

            if (wire != null && wire.value.HasValue)
                bits[connectionKey].Text = wire.value.Value ? "1" : "0";
        }

        private void Animate(object sender, RoutedEventArgs e)
        {
            foreach (var bit in bits.Values)
                bit.Visibility = Visibility.Visible;

            storyboard.Begin(this);
        }

        private async void Simulate(object sender, RoutedEventArgs e)
        {
            foreach (var wire in circuit.wires.Values)
                wire.ResetValue();

            foreach (var wire in circuit.wires.Values)
                if (wire.name.StartsWith('x'))
                    await Process(wire);
        }

        private void Repair(object sender, RoutedEventArgs e)
        {
            storyboard.Stop();

            foreach (var connection in connections)
                UnregisterName(connection.Key);

            canvas.Children.Clear();

            circuit.RepairCrossedWires();

            DrawCircuit();
        }

        private async Task Process(Wire wire)
        {
            foreach (var gate in circuit.gates)
            {
                if (!gate.ready && (gate.inputs[0] == wire || gate.inputs[1] == wire)
                    && gate.inputs[0].value.HasValue && gate.inputs[1].value.HasValue)
                {
                    gate.Process();

                    UpdateConnectionValue($"{gate.inputs[0].name}{gate.op}");
                    UpdateConnectionValue($"{gate.inputs[1].name}{gate.op}");
                    UpdateConnectionValue($"{gate.op}{gate.output.name}");

                    await Task.Delay(500);

                    HighlightEndWire(gate.output);

                    MarkIfInfluenced(gate.inputs[0]);
                    MarkIfInfluenced(gate.inputs[1]);
                    MarkIfInfluenced(gate.output);

                    await Process(gate.output);
                }
            }
        }

        private static void HighlightEndWire(Wire wire)
        {
            if (wire.name.StartsWith('z') && !wire.suspicious)
            {
                ((Rectangle)wire.uiElement).Stroke = Brushes.White;
                ((Rectangle)wire.uiElement).StrokeThickness = 1.5;
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

        private void DrawWire(Wire wire)
        {
            var rectangle = new Rectangle
            {
                Fill = elementBackgroundBrush,
                Stroke = wire.suspicious ? Brushes.Red : Brushes.Silver,
                StrokeThickness = wire.suspicious ? 1.5 : 1,
                RadiusX = cellHeight / 10,
                RadiusY = cellHeight / 10,
                Width = cellWidth,
                Height = cellHeight
            };
            Canvas.SetLeft(rectangle, CalculateLeft(wire.position.x));
            Canvas.SetTop(rectangle, CalculateTop(wire.position.y, wire.position.offset));
            canvas.Children.Add(rectangle);

            var label = new TextBox
            {
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                FontFamily = consolasFamily,
                FontSize = cellHeight * 0.4,
                FontWeight = FontWeights.Light,
                Foreground = wire.suspicious ? Brushes.Red : Brushes.Silver,
                Text = wire.name,
                Width = cellWidth,
                Height = cellHeight,
                TextAlignment = TextAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center
            };
            Canvas.SetLeft(label, CalculateLeft(wire.position.x));
            Canvas.SetTop(label, CalculateTop(wire.position.y, wire.position.offset));
            canvas.Children.Add(label);

            wire.uiElement = rectangle;
        }

        private void DrawGate(Gate gate)
        {
            var opSymbol = gate.op switch
            {
                "AND" => "&&",
                "OR" => "||",
                _ => "!="
            };

            var circle = new Ellipse
            {
                Fill = elementBackgroundBrush,
                Stroke = Brushes.Silver,
                Width = cellHeight,
                Height = cellHeight
            };
            Canvas.SetLeft(circle, CalculateLeft(gate.position.x, true));
            Canvas.SetTop(circle, CalculateTop(gate.position.y, gate.position.offset));
            canvas.Children.Add(circle);

            var label = new TextBox
            {
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                FontFamily = consolasFamily,
                FontSize = cellHeight * 0.4,
                FontWeight = FontWeights.Light,
                Foreground = Brushes.Silver,
                Text = opSymbol,
                Width = cellWidth,
                Height = cellHeight,
                TextAlignment = TextAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center
            };
            Canvas.SetLeft(label, CalculateLeft(gate.position.x));
            Canvas.SetTop(label, CalculateTop(gate.position.y, gate.position.offset));
            canvas.Children.Add(label);

            gate.uiElement = circle;
        }

        private void DrawConnection(Element from, Element to, string name, bool suspicious = false)
        {
            var x1 = CalculateLeft(from.position.x) + cellWidth / 2;
            var y1 = CalculateTop(from.position.y, from.position.offset) + cellHeight / 2;
            var x2 = CalculateLeft(to.position.x) + cellWidth / 2;
            var y2 = CalculateTop(to.position.y, to.position.offset) + cellHeight / 2;

            var lineFigure = new PathFigure();
            PathFigure arrowFigure;
            if (x1 < x2 && y1 < y2)
            {
                if (from.position.offset == to.position.offset)
                {
                    y1 += cellHeight / 2;
                    x2 -= to is Gate ? cellHeight / 2 : cellWidth / 2;

                    lineFigure.StartPoint = new Point(x1, y1);
                    lineFigure.Segments.Add(
                        new BezierSegment(
                            new Point(x1, y1 + cellHeight),
                            new Point(x2 - cellWidth, y2),
                            new Point(x2, y2),
                            true));

                    arrowFigure = CreateArrowFigure(x2, y2, 1);
                }
                else
                {
                    x1 += from is Gate ? cellHeight / 2 : cellWidth / 2;
                    y2 -= cellHeight / 2;

                    lineFigure.StartPoint = new Point(x1, y1);
                    lineFigure.Segments.Add(
                        new BezierSegment(
                            new Point(x1 + cellWidth, y1),
                            new Point(x2, y2 - cellHeight),
                            new Point(x2, y2),
                            true));

                    arrowFigure = CreateArrowFigure(x2, y2, 2);
                }
            }
            else if (x1 < x2 && y1 > y2)
            {
                if (to.position.y == 0)
                {
                    y1 -= cellHeight / 2;
                    x2 -= to is Gate ? cellHeight / 2 : cellWidth / 2;

                    lineFigure.StartPoint = new Point(x1, y1);
                    lineFigure.Segments.Add(
                        new BezierSegment(
                            new Point(x1, y1 - cellHeight / 2),
                            new Point(x2 - cellWidth / 2, y2),
                            new Point(x2, y2),
                            true));

                    arrowFigure = CreateArrowFigure(x2, y2, 1);
                }
                else
                {
                    x1 += from is Gate ? cellHeight / 2 : cellWidth / 2;
                    y2 += cellHeight / 2;

                    lineFigure.StartPoint = new Point(x1, y1);
                    lineFigure.Segments.Add(
                        new BezierSegment(
                            new Point(x1 + cellWidth / 2, y1),
                            new Point(x2, y2 + cellHeight / 2),
                            new Point(x2, y2),
                            true));

                    arrowFigure = CreateArrowFigure(x2, y2, 0);
                }
            }
            else if (x1 > x2 && y1 < y2)
            {
                y1 += cellHeight / 2;
                y2 -= cellHeight / 2;

                lineFigure.StartPoint = new Point(x1, y1);
                lineFigure.Segments.Add(
                    new BezierSegment(
                        new Point(x1 + spacing, y1 + (y2 - y1) * 0.75),
                        new Point(x2 - spacing, y2 - (y2 - y1) * 0.75),
                        new Point(x2, y2),
                        true));

                arrowFigure = CreateArrowFigure(x2, y2, 2);
            }
            else
            {
                var direction = 0;
                if (x1 < x2)
                {
                    direction = 1;
                    x1 += from is Gate ? cellHeight / 2 : cellWidth / 2;
                    x2 -= to is Gate ? cellHeight / 2 : cellWidth / 2;
                }
                else if (x1 > x2)
                {
                    direction = 3;
                    x1 -= from is Gate ? cellHeight / 2 : cellWidth / 2;
                    x2 += to is Gate ? cellHeight / 2 : cellWidth / 2;
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

                lineFigure.StartPoint = new Point(x1, y1);
                lineFigure.Segments.Add(
                    new LineSegment(
                        new Point(x2, y2),
                        true));

                arrowFigure = CreateArrowFigure(x2, y2, direction);
            }

            var geometry = new PathGeometry();
            geometry.Figures.Add(lineFigure);
            geometry.Figures.Add(arrowFigure);
            geometry.Freeze();

            var path = new Path
            {
                Data = geometry,
                Stroke = suspicious ? Brushes.Red : greenBrush,
                StrokeThickness = suspicious ? 1.5 : 1
            };

            connections.Add(name, path);
            canvas.Children.Add(path);
        }

        private static PathFigure CreateArrowFigure(double x, double y, int direction)
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

            var figure = new PathFigure { StartPoint = new Point(x1, y1) };
            figure.Segments.Add(new LineSegment(new Point(x, y), true));
            figure.Segments.Add(new LineSegment(new Point(x3, y3), true));

            return figure;
        }

        private static double CalculateLeft(int xPos, bool circle = false)
        {
            var indent = circle ? (cellWidth - cellHeight) / 2 : 0d;
            return spacing + ++xPos * (cellWidth + spacing) + indent;
        }

        private static double CalculateTop(int yPos, int offset)
        {
            var expandOffset = offset * (cellHeight + spacing) * 4;
            return spacing + expandOffset + yPos * (cellHeight + spacing);
        }
    }
}