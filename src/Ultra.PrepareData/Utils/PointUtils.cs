using System.Windows;

namespace Ultra.PrepareData.Utils
{
    public static class PointUtils
    {
        public static double DistanceTo(this Point p1, Point p2)
        {
            return Point.Subtract(p1, p2).Length;
        }
    }
}