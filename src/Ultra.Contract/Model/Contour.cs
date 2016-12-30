using System.Collections.Generic;

namespace Ultra.Contract.Model
{
    public class Contour : List<Contour.Point>
    {
        public Contour()
        {
        }

        public Contour(IEnumerable<Point> list) : base(list)
        {
        }

        public struct Point
        {
            public double X { get; set; }
            public double Y { get; set; }
        }
    }
}