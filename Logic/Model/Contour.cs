using System.Collections.Generic;

namespace Logic.Model
{
    public class Contour : List<Contour.Point>
    {
        private Contour(IEnumerable<Point> list) :base(list)
        {
        }

        public class Point
        {
            public double X { get; set; }
            public double Y { get; set; }

            #region
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

        public static Contour FromIEnumerable(IEnumerable<Contour.Point> list)
        {
            return new Contour(list);
        }
    }
}