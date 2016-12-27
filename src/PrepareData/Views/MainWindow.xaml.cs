﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace PrepareData.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ImageListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = sender as ListBox;
            listBox?.ScrollIntoView(listBox.SelectedItem);
        }
    }
}
