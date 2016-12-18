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
            Pts = new ObservableCollection<Point>(ps.Contour.Pts.Select(point => new Point()
            {
                X = point.X,
                Y = point.Y,
            }));
        }

        public ParkingSlotVM()
        {
            Pts = new ObservableCollection<Point>();
        }

        public bool? IsOccupied { get; set; }
        public int Id { get; set; }
        public ObservableCollection<Point> Pts { get; set; }
    }
}