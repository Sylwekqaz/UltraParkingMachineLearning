using System.Collections.Generic;

namespace In�.Model
{
    public class Contour
    {
        public Contour()
        {
            Pts = new List<Point>();
        }

        public int Id { get; set; }
        public List<Point> Pts { get; set; }

        public class Point
        {
            public double X { get; set; }
            public double Y { get; set; }

            #region

            public static implicit operator System.Windows.Point(Point src)
            {
                return new System.Windows.Point(src.X, src.Y);
            }

            public static implicit operator Point(System.Windows.Point src)
            {
                return new Point() {X = src.X, Y = src.Y};
            }

            public static implicit operator OpenCvSharp.Point(Point src)
            {
                return new OpenCvSharp.Point(src.X, src.Y);
            }

            public static implicit operator Point(OpenCvSharp.Point src)
            {
                return new Point() {X = src.X, Y = src.Y};
            }

            #endregion
        }
    }
}