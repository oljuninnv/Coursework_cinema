using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Cinema
{
    /// <summary>
    /// Логика взаимодействия для SellerWindow.xaml
    /// </summary>
    public partial class SellerWindow : Window
    {
        public SellerWindow()
        {
            InitializeComponent();
        }

        private void Exit_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Films_Button_Click(object sender, RoutedEventArgs e)
        {
            var window = new FilmsWindow();
            window.Show();
            this.Close();
        }

        private void Seans_Button_Click(object sender, RoutedEventArgs e)
        {
            var window = new SessionWindow();
            window.Show();
            this.Close();
        }
    }
}
