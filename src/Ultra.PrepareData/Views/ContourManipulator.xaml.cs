using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Ultra.PrepareData.Utils;

namespace Ultra.PrepareData.Views
{
    /// <summary>
    /// Interaction logic for ContourManipulator.xaml
    /// </summary>
    public partial class ContourManipulator
    {
        public ObservableCollection<Point> Contour
        {
            get { return (ObservableCollection<Point>) GetValue(ContourPropertyProperty); }
            set { SetValue(ContourPropertyProperty, value); }
        }
        public double Scale
        {
            get { return (double) GetValue(ScalePropertyProperty); }
            set { SetValue(ScalePropertyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContourPropertyProperty =
            DependencyProperty
                .Register("Contour",
                    typeof(ObservableCollection<Point>),
                    typeof(ContourManipulator),
                    new FrameworkPropertyMetadata((o, args) => { ((ContourManipulator) o).ContourChanged(); }));

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScalePropertyProperty =
            DependencyProperty
                .Register("Scale",
                    typeof(double),
                    typeof(ContourManipulator),
                    new FrameworkPropertyMetadata((o, args) => { ((ContourManipulator) o).FullRedraw(); }));


        private void ContourChanged()
        {
            ActivePointIndex = null;
            if (Contour != null)
                Contour.CollectionChanged += (sender, args) => FullRedraw();
            FullRedraw();
        }


        public ContourManipulator()
        {
            InitializeComponent();
            ContourChanged();
        }

        public void FullRedraw()
        {
            if (Scale == 0) //window not fully yet, skip redrawing
                return;

            RootCanvas.Children.Clear();
            var contour = Contour;
            if (contour == null) return;



            foreach (var point in contour)
            {
                AddPoint(point);
            }

            for (var i = 0; i < contour.Count - 1; i++)
            {
                var point1 = contour[i];
                var point2 = contour[i + 1];
                AddLine(point1, point2);
            }
            if (contour.Any())
                AddLine(contour.Last(), contour.First(), dashed: true);
        }

        private void AddPoint(Point point)
        {
            var e = new Ellipse
            {
                Height = 10 / Scale,
                Width = 10 / Scale,
                RenderTransform = new TranslateTransform(-5 / Scale, -5 / Scale),
                Stroke = Brushes.White,
                Fill = Brushes.Black,
                StrokeThickness = 1 / Scale,
            };
            e.MouseUp += RootCanvas_OnMouseUp;
            Canvas.SetLeft(e, point.X);
            Canvas.SetTop(e, point.Y);

            RootCanvas.Children.Add(e);
        }

        private void AddLine(Point point1, Point point2, bool dashed = false)
        {
            var e = new Line()
            {
                X1 = point1.X,
                Y1 = point1.Y,
                X2 = point2.X,
                Y2 = point2.Y,
                Stroke = Brushes.Magenta,
                StrokeThickness = 1 / Scale,
            };
            if (dashed)
            {
                e.StrokeDashArray = new DoubleCollection() { 2 / Scale, 2 / Scale };
            }

            e.MouseUp += RootCanvas_OnMouseUp;

            RootCanvas.Children.Add(e);
        }

        private void RootCanvas_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Contour == null) return;

            var clickPosition = e.GetPosition(RootCanvas);
            var point = Contour.FirstOrDefault(p => p.DistanceTo(clickPosition) < (12 / Scale));
            var index = Contour.IndexOf(point);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (index == -1)
                {
                    // no points get clicked
                    Contour.Add(clickPosition);
                    ActivePointIndex = Contour.Count - 1;
                }
                else if (index == 0 && (Keyboard.Modifiers & ModifierKeys.Control) > 0)
                {
                    //todo
                }
                else
                {
                    ActivePointIndex = index;
                }
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                if (index != -1)
                    Contour.RemoveAt(index);
            }
        }

        public int? ActivePointIndex { get; set; }

        private void RootCanvas_OnMouseMove(object sender, MouseEventArgs e)
        {
            var clickPosition = e.GetPosition(RootCanvas);
            var pointNearby = Contour?.Any(p => p.DistanceTo(clickPosition) < (12 / Scale)) ?? false;
            Cursor = pointNearby ? Cursors.SizeAll : Cursors.Arrow;

            if (ActivePointIndex != null)
            {
                Contour[ActivePointIndex.Value] = e.GetPosition(RootCanvas);
            }
        }

        private void RootCanvas_OnMouseUp(object sender, MouseEventArgs e)
        {
            ActivePointIndex = null;
        }
    }
}