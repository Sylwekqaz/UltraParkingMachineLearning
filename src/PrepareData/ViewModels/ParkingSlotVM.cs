using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Logic.Model;
using System.Windows;
using System.Windows.Media;
using PropertyChanged;

namespace PrepareData.ViewModels
{
    [ImplementPropertyChanged]
    public class ParkingSlotVM
    {
        public ParkingSlotVM(ParkingSlot ps) : this()
        {
            IsOccupied = ps.IsOccupied;
            Id = ps.Contour.Id;
            Pts = new PointCollection(ps.Contour.Pts.Select(point => new Point()
            {
                X = point.X,
                Y = point.Y,
            }));
        }

        public ParkingSlotVM()
        {
            Pts = new PointCollection();
        }

        public bool? IsOccupied { get; set; }
        public int Id { get; set; }
        public PointCollection Pts { get; set; }
    }
}