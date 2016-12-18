using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Logic.Model;
using System.Windows;
using System.Windows.Media;
using PropertyChanged;

namespace PrepareData.ViewModels
{
    public class ParkingSlotVM : INotifyPropertyChanged
    {
        private ObservableCollection<Point> _pts;
        private int _id;
        private bool? _isOccupied;

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

        public bool? IsOccupied
        {
            get { return _isOccupied; }
            set
            {
                _isOccupied = value;
                OnPropertyChanged(nameof(PtsCollection));
            }
        }

        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged(nameof(PtsCollection));
            }
        }

        public ObservableCollection<Point> Pts
        {
            get { return _pts; }
            set
            {
                _pts = value;
                value.CollectionChanged += (sender, args) => OnPropertyChanged(nameof(PtsCollection));
                OnPropertyChanged(nameof(PtsCollection));
                OnPropertyChanged(nameof(Pts));
            }
        }

        public PointCollection PtsCollection => new PointCollection(Pts);


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }
    }
}