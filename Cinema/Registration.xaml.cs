using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для Registration.xaml
    /// </summary>
    public partial class Registration : Window
    {

        public Registration()
        {
            InitializeComponent();
            UsernameLabel.Visibility = Visibility.Collapsed;
            PasswordLabel.Visibility = Visibility.Collapsed;
            NameLabel.Visibility = Visibility.Collapsed;
            SurnameLabel.Visibility = Visibility.Collapsed;
            LastNameLabel.Visibility = Visibility.Collapsed;
        }

        private void Reg_Click(object sender, RoutedEventArgs e)
        {
            UsernameLabel.Content = "";
            PasswordLabel.Content = "";
            NameLabel.Content = "";
            SurnameLabel.Content = "";
            LastNameLabel.Content = "";

            Regex usernameRegex = new Regex(@"^[a-zA-Zа-яА-Я]{3,20}$");
            Regex nameRegex = new Regex(@"^[a-zA-Zа-яА-Я]{3,20}$");
            Regex surnameRegex = new Regex(@"^[a-zA-Zа-яА-Я]{3,20}$");
            Regex passwordRegex = new Regex(@"^.{10,20}$");


            if (!nameRegex.IsMatch(Name.Text))
            {
                NameLabel.Content = "Введите своё имя";
                NameLabel.Visibility = Visibility.Visible;
                return;
            }
            if (!surnameRegex.IsMatch(Surname.Text))
            {
                SurnameLabel.Content = "Введите свою фамилию";
                SurnameLabel.Visibility = Visibility.Visible;
                return;
            
            }
            if (!usernameRegex.IsMatch(UsernameTextBox.Text))
            {
                UsernameLabel.Content = "Имя пользователя должно содержать \nот 3 до 20 символов (латинские или русские буквы)";
                UsernameLabel.Visibility = Visibility.Visible;
                return;
            }
            if (!passwordRegex.IsMatch(PasswordBox.Password))
            {
                PasswordLabel.Content = "Пароль должен содержать от 10 до 20 символов";
                PasswordLabel.Visibility = Visibility.Visible;
                return;
            }
            
            int UserNumber = CinemaEntities.GetContext().Employee.ToList().Max(i => i.ID_employee);

            if (UserNumber == 0)
            {
                UserNumber = 1;
            }
            else if (UserNumber == 1)
            {
                UserNumber++;
            }

            string password = GetHash.hashPassword(PasswordBox.Password);

            Employee employee = new Employee()
            {
                ID_employee = UserNumber + 1,
                Name = Name.Text,
                Surname = Surname.Text,
                LastName = !string.IsNullOrEmpty(LastName.Text) ? " " : LastName.Text,
                Username = UsernameTextBox.Text,
                Password = password,
                Post = "user",
            };

            CinemaEntities.GetContext().Employee.Add(employee);
            try
            {
                CinemaEntities.GetContext().SaveChanges();
                MessageBox.Show("Успешная регистрация");
                var window = new MainWindow();
                window.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }

        }
    }
}
