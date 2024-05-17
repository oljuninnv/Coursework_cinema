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
using System.Windows.Navigation;
using System.Windows.Shapes;
using BCrypt.Net;

namespace Cinema
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            UsernameLabel.Visibility = Visibility.Collapsed;
            PasswordLabel.Visibility = Visibility.Collapsed;
        }

        private void Input_Button_Click(object sender, RoutedEventArgs e)
        {
            UsernameLabel.Content = "";
            PasswordLabel.Content = "";

            if (UsernameTextBox.Text == "")
            {
                UsernameLabel.Content = "Введите логин";
                UsernameLabel.Visibility = Visibility.Visible;
                return;
            }

            if (PasswordBox.Password == "")
            {
                PasswordLabel.Content = "Введите пароль";
                PasswordLabel.Visibility = Visibility.Visible;
                return;
            }

            Employee user = new Employee();

            string password = GetHash.hashPassword(PasswordBox.Password);

            user = CinemaEntities.GetContext().Employee.Where(u => u.Username == UsernameTextBox.Text && u.Password == password).FirstOrDefault();

            if (user == null)
            {
                user =  CinemaEntities.GetContext().Employee.Where(u => u.Username == UsernameTextBox.Text && u.Password == PasswordBox.Password).FirstOrDefault();
                if (user == null)
                {
                    MessageBox.Show("Такого пользователя не существует или вы ввели неправильно имя или пароль");
                    return;
                }
            }

            if (user.Post == "admin")
            {
                var window = new AdminWindow();
                window.Show();
                this.Close();
            }
            else if (user.Post == "seller")
            {
                var window = new SellerWindow();
                window.Show();
                this.Close();
            }
            else
            {
                var window = new UserWindow();
                window.Show();
                this.Close();
            }

        }

        private void Register_Button_Click(object sender, RoutedEventArgs e)
        {
            var window = new Registration();
            window.Show();
            this.Close();
        }
    }
}
