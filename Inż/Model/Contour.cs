using System;
using System.Collections.Generic;

namespace In¿.Model
{
    public class Contour
    {
        public Contour()
        {
            Pts= new List<Point>();
        }

        public int Id { get; set; }
        public List<Point> Pts { get; set; }

        public class Point
        {
            public double X { get; set; }
            public double Y { get; set; }
        }
    }
}