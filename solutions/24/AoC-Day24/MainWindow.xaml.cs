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

        public MainWindow()
        {
            InitializeComponent();

            consolasFamily = new FontFamily("Consolas");
            backgroundBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#0f0f23");

            var canvas = new Canvas();
            canvas.Width = Width;
            canvas.Height = 1500;
            canvas.Background = backgroundBrush;
            Canvas.SetTop(canvas, 0);
            Canvas.SetLeft(canvas, 0);

            DrawConnection(canvas, 0, 1, 0, 1, 1, 0);
            DrawConnection(canvas, 1, 3, 0, 2, 2, 0);
            DrawConnection(canvas, 1, 3, 0, 1, 1, 0);
            DrawConnection(canvas, 1, 1, 0, 3, 1, 0);
            DrawConnection(canvas, 2, 2, 0, 4, 2, 0);
            DrawConnection(canvas, 0, 1, 0, 2, 2, 0);
            DrawConnection(canvas, 3, 1, 0, 4, 0, 0);
            DrawConnection(canvas, 3, 1, 0, 5, 1, 0);
            DrawConnection(canvas, 4, 2, 0, 6, 2, 0);
            DrawConnection(canvas, 4, 2, 0, 6, 2, 0);
            DrawConnection(canvas, 4, 0, 0, 7, 0, 0);
            DrawConnection(canvas, 5, 1, 0, 9, 1, 0);
            DrawConnection(canvas, 6, 2, 0, 8, 2, 0);
            DrawConnection(canvas, 7, 0, 0, 6, 2, 0);
            DrawConnection(canvas, 8, 2, 0, 4, 0, 1);
            DrawConnection(canvas, 8, 2, 0, 5, 1, 1);

            DrawWire(canvas, "x02", 0, 1, 0);
            DrawGate(canvas, "!=", 1, 1, 0);

            DrawWire(canvas, "y02", 1, 3, 0);
            DrawGate(canvas, "&&", 2, 2, 0);

            DrawWire(canvas, "jpq", 3, 1, 0);
            DrawWire(canvas, "rjb", 4, 2, 0);

            DrawGate(canvas, "&&", 4, 0, 0);
            DrawGate(canvas, "!=", 5, 1, 0);
            DrawGate(canvas, "||", 6, 2, 0);
            DrawWire(canvas, "kwm", 7, 0, 0);
            DrawWire(canvas, "gfc", 8, 2, 0);
            DrawWire(canvas, "z02", 9, 1, 0);

            DrawGate(canvas, "&&", 4, 0, 1);
            DrawGate(canvas, "!=", 5, 1, 1);


            Content = canvas;
            Show();
        }

        public void DrawWire(Canvas canvas, string name, int xPos, int yPos, int offset)
        {
            var rectangle = new Rectangle
            {
                Fill = backgroundBrush,
                Stroke = Brushes.Silver,
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

        public void DrawGate(Canvas canvas, string name, int xPos, int yPos, int offset)
        {
            var circle = new Ellipse
            {
                Fill = backgroundBrush,
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

        // always draw from input to gate and from gate to output for logic to work
        public void DrawConnection(Canvas canvas, int fromX, int fromY, int fromOffset, int toX, int toY, int toOffset)
        {
            var x1 = CalculateLeft(fromX) + cellWidth / 2;
            var y1 = CalculateTop(fromY, fromOffset) + cellHeight / 2;
            var x2 = CalculateLeft(toX) + cellWidth / 2;
            var y2 = CalculateTop(toY, toOffset) + cellHeight / 2;

            var myPathFigure = new PathFigure();
            myPathFigure.StartPoint = new Point(x1, y1);
            if (x1 < x2 && y1 < y2)
            {
                myPathFigure.Segments.Add(
                    new BezierSegment(
                        new Point(x1 - cellWidth / 2, y1 + cellHeight * 1.5),
                        new Point(x2 - cellWidth, y2 + spacing),
                        new Point(x2, y2),
                        true /* IsStroked */ ));
            }
            else if (x1 < x2 && y1 > y2)
            {
                if (toY == 0)
                    myPathFigure.Segments.Add(
                        new BezierSegment(
                            new Point(x1 - spacing, y1 - cellHeight),
                            new Point(x2 - cellWidth, y2 - spacing),
                            new Point(x2, y2),
                            true /* IsStroked */ ));
                else
                    myPathFigure.Segments.Add(
                        new BezierSegment(
                            new Point(x1 + cellWidth, y1 + spacing),
                            new Point(x2 + spacing, y2 + cellHeight),
                            new Point(x2, y2),
                            true /* IsStroked */ ));
            }
            else if (x1 > x2 && y1 < y2)
            {
                myPathFigure.Segments.Add(
                    new BezierSegment(
                        new Point(x1 + cellWidth / 2, y1 + cellHeight * 2),
                        new Point(x2 - cellWidth / 2, y2 - cellHeight * 2),
                        new Point(x2, y2),
                        true /* IsStroked */ ));
            }
            else
            {
                myPathFigure.Segments.Add(
                    new LineSegment(
                        new Point(x2, y2),
                        true /* IsStroked */ ));
            }

            /// Create a PathGeometry to contain the figure.
            var myPathGeometry = new PathGeometry();
            myPathGeometry.Figures.Add(myPathFigure);

            // Display the PathGeometry.
            var myPath = new Path();
            myPath.Stroke = Brushes.Silver;
            myPath.StrokeThickness = 1;
            myPath.Data = myPathGeometry;

            canvas.Children.Add(myPath);
        }

        private static double CalculateLeft(int xPos, bool circle = false)
        {
            var indent = 0d;
            if (circle)
                indent = (cellWidth - cellHeight) / 2;

            return spacing + xPos * (cellWidth + spacing) + indent;
        }

        private static double CalculateTop(int yPos, int offset)
        {
            var expandOffset = offset * (cellHeight + spacing) * 4;

            return spacing + expandOffset + yPos * (cellHeight + spacing);
        }
    }
}