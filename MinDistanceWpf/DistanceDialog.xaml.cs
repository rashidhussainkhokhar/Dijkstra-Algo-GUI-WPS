using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MinDistanceWpf
{
    /// <summary>
    /// Interaction logic for DistanceDialog.xaml
    /// </summary>
    public partial class DistanceDialog : Window
    {
        private double _distance;
        public DistanceDialog()
        {
            InitializeComponent();
            _distance = 0.0;
        }

        private void okBtn_Click(object sender, RoutedEventArgs e)
        {
            if(!string.IsNullOrEmpty(distanceTextBox.Text) && double.TryParse(distanceTextBox.Text, out _distance) && _distance > 0.0)
                this.DialogResult = true;
            else
                MessageBox.Show("Please give a valid number greater than 0", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        public double Distance
        {
            get { return _distance; }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_distance == 0.0)
                this.DialogResult = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            distanceTextBox.Focus();
        }
    }
}
