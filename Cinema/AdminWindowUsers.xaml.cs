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
    /// Логика взаимодействия для AdminWindowUsers.xaml
    /// </summary>
    public partial class AdminWindowUsers : Window
    {
        public string action = "";
        List<Employee> items = null;
        public AdminWindowUsers()
        {
            InitializeComponent();

            items = CinemaEntities.GetContext().Employee.ToList();
            DataGridUser.ItemsSource = items;

            var context = CinemaEntities.GetContext();

            var users = context.Employee.Select(user => user.Post).Distinct().ToList();
            users.Insert(0, "Все роли");

            ComboBoxPost.ItemsSource = users;
            ComboBoxPost.SelectedIndex = 0;
        }

        public bool isDirty = true;

        private void Undo_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var context = CinemaEntities.GetContext();
            switch (action)
            {
                case "add":
                    var addedRecipe = context.Employee.Local.Last();
                    if (addedRecipe != null)
                    {
                        context.Employee.Remove(addedRecipe);
                        context.SaveChanges();
                        DataGridUser.ItemsSource = null;
                        DataGridUser.ItemsSource = context.Employee.ToList();
                        MessageBox.Show("Последнее добавление фильма было отменено");
                    }
                    break;
                case "undo":
                    DataGridUser.ItemsSource = null;
                    DataGridUser.ItemsSource = CinemaEntities.GetContext().Employee.ToList();
                    break;
                case "":
                    DataGridUser.ItemsSource = CinemaEntities.GetContext().Employee.ToList();
                    break;
            }

            MessageBox.Show("Отмена действий");
            isDirty = true;
            DataGridUser.IsReadOnly = true;
        }

        private void Undo_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !isDirty;
        }

        private void Edit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DataGridUser.ItemsSource = null;

            DataGridUser.ItemsSource = items;

            DataGridUser.IsReadOnly = false;
            DataGridUser.BeginEdit();
            action = "undo";
            isDirty = false;
            MessageBox.Show("Редактировать");
        }

        private void Edit_CanExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = isDirty;
        }

        private void New_Executed(object sender, ExecutedRoutedEventArgs e)
        {

            var items = CinemaEntities.GetContext().Employee.ToList();
            DataGridUser.ItemsSource = items;

            DataGridUser.IsReadOnly = false;
            int maxId = CinemaEntities.GetContext().Employee.ToList().Max(i => i.ID_employee);
            Employee employee = new Employee()
            {
                ID_employee = maxId + 1,
                Name = "Введите имя пользователя",
                Surname = "Введите фамилию пользователя",
                LastName = "Введите отчевство пользователя",
                Post = "Введите должность пользователя (seller или admin)",
                Username = "Введите username пользователя",
                Password = "Введите Password пользователя",
            };
            CinemaEntities.GetContext().Employee.Add(employee);
            try
            {
                CinemaEntities.GetContext().SaveChanges();
                DataGridUser.ItemsSource = null;
                items = CinemaEntities.GetContext().Employee.ToList();
                DataGridUser.ItemsSource = items;
                DataGridUser.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            action = "add";
            isDirty = false;
        }

        private void New_CanExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = isDirty;
        }

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CinemaEntities.GetContext().SaveChanges();
            DataGridUser.ItemsSource = CinemaEntities.GetContext().Employee.ToList();
            DataGridUser.Items.Refresh();
            DataGridUser.IsReadOnly = true;
            action = "";
            isDirty = true;
        }

        private void Save_CanExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !isDirty;
        }

        private void Find_CanExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = isDirty;
        }

        private void Delete_Executed(object sender, ExecutedRoutedEventArgs e)
        {

            var items = CinemaEntities.GetContext().Employee.ToList();

            Employee employee = DataGridUser.SelectedItem as Employee;

            MessageBoxResult result = MessageBox.Show("Удалить данные ",
           "Предупреждение", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                DataGridUser.SelectedIndex =
                DataGridUser.SelectedIndex == 0 ? 1 :
                DataGridUser.SelectedIndex - 1;
                CinemaEntities.GetContext().Employee.Remove(employee);
                CinemaEntities.GetContext().SaveChanges();
                DataGridUser.ItemsSource = CinemaEntities.GetContext().Employee.ToList();
                DataGridUser.Items.Refresh();
            }
            else
            {
                MessageBox.Show("Выберите строку для удаления");
            }
            action = "";
        }

        private void Delete_CanExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = isDirty;
        }

        private void Find_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (TextBoxUser.Text == "" && ComboBoxPost.SelectedValue.ToString() == "Все роли")
            {
                items = CinemaEntities.GetContext().Employee.ToList();
                DataGridUser.ItemsSource = items;
                return;
            }
            //Вывод всех пользователей с определённым именем
            else if (TextBoxUser.Text != "" && ComboBoxPost.SelectedValue.ToString() == "Все роли")
            {
                string searchName = TextBoxUser.Text;
                items = CinemaEntities.GetContext().Employee.Where(r => r.Name.StartsWith(searchName) || r.Surname.StartsWith(searchName) || r.LastName.StartsWith(searchName)).ToList();
                if (items.Count == 0)
                {
                    MessageBox.Show("Такого пользователя не существует, попробуйте снова");
                    return;
                }
                else
                {
                    DataGridUser.ItemsSource = items;
                    return;
                }
            }
            //Вывод всех пользователей определённой роли
            else if (TextBoxUser.Text == "" && ComboBoxPost.SelectedValue.ToString() != "Все роли")
            {
                items = CinemaEntities.GetContext().Employee.Where(r => r.Post == ComboBoxPost.SelectedValue.ToString()).ToList();
                if (items.Count == 0)
                {
                    MessageBox.Show($"Нету пользователей роли {ComboBoxPost.SelectedValue.ToString()}");
                    return;
                }
                else
                {
                    DataGridUser.ItemsSource = items;
                    return;
                }
            }
            //Вывод определённого пользователя определённого роли
            else if (TextBoxUser.Text != "" && ComboBoxPost.SelectedValue.ToString() != "Все роли")
            {
                string searchName = TextBoxUser.Text;
                string selectedBrand = ComboBoxPost.SelectedValue.ToString();
                items = CinemaEntities.GetContext().Employee.Where(r => r.Post.ToString() == selectedBrand && r.Name.StartsWith(searchName) || r.Surname.StartsWith(searchName) || r.LastName.StartsWith(searchName)).ToList();
                if (items.Count == 0)
                {
                    MessageBox.Show($"Нету такого пользователя с ролью {ComboBoxPost.SelectedValue.ToString()}");
                    return;
                }
                else
                {
                    DataGridUser.ItemsSource = items;
                    return;
                }

            }

        }

        private void Find_Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (TextBoxUser.Text == "" && ComboBoxPost.SelectedValue.ToString() == "Все роли")
            {
                items = CinemaEntities.GetContext().Employee.ToList();
                DataGridUser.ItemsSource = items;
                return;
            }
            //Вывод всех пользователей с определённым именем
            else if (TextBoxUser.Text != "" && ComboBoxPost.SelectedValue.ToString() == "Все роли")
            {
                string searchName = TextBoxUser.Text;
                items = CinemaEntities.GetContext().Employee.Where(r => r.Name.StartsWith(searchName) || r.Surname.StartsWith(searchName) || r.LastName.StartsWith(searchName)).ToList();
                if (items.Count == 0)
                {
                    MessageBox.Show("Такого пользователя не существует, попробуйте снова");
                    return;
                }
                else
                {
                    DataGridUser.ItemsSource = items;
                    return;
                }
            }
            //Вывод всех пользователей определённой роли
            else if (TextBoxUser.Text == "" && ComboBoxPost.SelectedValue.ToString() != "Все роли")
            {
                items = CinemaEntities.GetContext().Employee.Where(r => r.Post == ComboBoxPost.SelectedValue.ToString()).ToList();
                if (items.Count == 0)
                {
                    MessageBox.Show($"Нету пользователей роли {ComboBoxPost.SelectedValue.ToString()}");
                    return;
                }
                else
                {
                    DataGridUser.ItemsSource = items;
                    return;
                }
            }
            //Вывод определённого пользователя определённого роли
            else if (TextBoxUser.Text != "" && ComboBoxPost.SelectedValue.ToString() != "Все роли")
            {
                string searchName = TextBoxUser.Text;
                string selectedBrand = ComboBoxPost.SelectedValue.ToString();
                items = CinemaEntities.GetContext().Employee.Where(r => r.Post.ToString() == selectedBrand && r.Name.StartsWith(searchName) || r.Surname.StartsWith(searchName) || r.LastName.StartsWith(searchName)).ToList();
                if (items.Count == 0)
                {
                    MessageBox.Show($"Нету такого пользователя с ролью {ComboBoxPost.SelectedValue.ToString()}");
                    return;
                }
                else
                {
                    DataGridUser.ItemsSource = items;
                    return;
                }

            }
        }
    }
}
