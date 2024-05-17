using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.Remoting.Contexts;
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
    /// Логика взаимодействия для SessionWindow.xaml
    /// </summary>
    public partial class SessionWindow : Window
    {
        public string action = "";
        List<Session> items = null;
        public SessionWindow()
        {
            InitializeComponent();
            items = CinemaEntities.GetContext().Session.ToList();
            DataGridSession.ItemsSource = items;

            var context = CinemaEntities.GetContext();

            var halls = context.Hall.Select(hall => hall.Name).Distinct().ToList();

            ComboBoxHall.ItemsSource = halls;

            var date = context.Session.Select(dates => dates.Date).Distinct().ToList();

            ComboBoxDate.ItemsSource = date;
        }

        public bool isDirty = true;

        private void Undo_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !isDirty;
        }

        private void Edit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DataGridSession.ItemsSource = null;

            DataGridSession.ItemsSource = items;

            DataGridSession.IsReadOnly = false;
            DataGridSession.BeginEdit();
            action = "undo";
            isDirty = false;
            MessageBox.Show("Редактировать");
        }

        private void Edit_CanExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = isDirty;
        }

        private void New_CanExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = isDirty;
        }

        private void Save_CanExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !isDirty;
        }

        private void Find_CanExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = isDirty;
        }

        private void New_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var items = CinemaEntities.GetContext().Session.ToList();
            DataGridSession.ItemsSource = items;

            DataGridSession.IsReadOnly = false;
            int maxId = CinemaEntities.GetContext().Session.ToList().Max(i => i.ID_Session);
            Session session = new Session()
            {
                ID_Session = maxId + 1,
                Time = DateTime.Now.TimeOfDay,
                Price = 150,
                Film = CinemaEntities.GetContext().Film.FirstOrDefault(),
                Hall = CinemaEntities.GetContext().Hall.FirstOrDefault(),
            };
            CinemaEntities.GetContext().Session.Add(session);
            try
            {
                CinemaEntities.GetContext().SaveChanges();
                DataGridSession.ItemsSource = null;
                items = CinemaEntities.GetContext().Session.ToList();
                DataGridSession.ItemsSource = items;
                DataGridSession.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            action = "add";
            isDirty = false;
        }

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CinemaEntities.GetContext().SaveChanges();
            DataGridSession.ItemsSource = CinemaEntities.GetContext().Session.ToList();
            DataGridSession.Items.Refresh();
            DataGridSession.IsReadOnly = true;
            action = "";
            isDirty = true;
        }

        private void Find_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void Delete_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var items = CinemaEntities.GetContext().Session.ToList();

            Session session = DataGridSession.SelectedItem as Session;

            MessageBoxResult result = MessageBox.Show("Удалить данные ",
           "Предупреждение", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                DataGridSession.SelectedIndex =
                DataGridSession.SelectedIndex == 0 ? 1 :
                DataGridSession.SelectedIndex - 1;
                CinemaEntities.GetContext().Session.Remove(session);
                CinemaEntities.GetContext().SaveChanges();
                DataGridSession.ItemsSource = CinemaEntities.GetContext().Session.ToList();
                DataGridSession.Items.Refresh();
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

        private void Undo_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var context = CinemaEntities.GetContext();
            switch (action)
            {
                case "add":
                    var addSesion = context.Session.Local.Last();
                    if (addSesion != null)
                    {
                        context.Session.Remove(addSesion);
                        context.SaveChanges();
                        DataGridSession.ItemsSource = null;
                        DataGridSession.ItemsSource = context.Session.ToList();
                        MessageBox.Show("Последнее добавление фильма было отменено");
                    }
                    break;
                case "undo":
                    DataGridSession.ItemsSource = null;
                    DataGridSession.ItemsSource = CinemaEntities.GetContext().Session.ToList();
                    break;
                case "":
                    DataGridSession.ItemsSource = CinemaEntities.GetContext().Session.ToList();
                    break;
            }

            MessageBox.Show("Отмена действий");
            isDirty = true;
            DataGridSession.IsReadOnly = true;
        }

        private void Find_Button_Click(object sender, RoutedEventArgs e)
        {
            string filmName = TextBoxFilm.Text;
            DateTime? date = ComboBoxDate.SelectedValue is DateTime ? (DateTime?)ComboBoxDate.SelectedValue : null;
            string hall = ComboBoxHall.SelectedValue != null ? ComboBoxHall.SelectedValue.ToString() : null;

            List<Session> sessions;

            if (filmName != "" && date != null && hall != null)
            {
                sessions = CinemaEntities.GetContext().Session.Where(s => s.Film.Name.StartsWith(filmName) && s.Date == date && s.Hall.Name == hall).ToList();
            }
            else if (filmName != "" && date != null)
            {
                sessions = CinemaEntities.GetContext().Session.Where(s => s.Film.Name.StartsWith(filmName) && s.Date == date).ToList();
            }
            else if (filmName != "" && hall != null)
            {
                sessions = CinemaEntities.GetContext().Session.Where(s => s.Film.Name.StartsWith(filmName) && s.Hall.Name == hall).ToList(); ;
            }
            else if (date != null && hall != null)
            {
                sessions = CinemaEntities.GetContext().Session.Where(s => s.Date == date && s.Hall.Name == hall).ToList();
            }
            else if (filmName != "")
            {
                sessions = CinemaEntities.GetContext().Session.Where(s => s.Film.Name.StartsWith(filmName)).ToList();
            }
            else if (date != null)
            {
                sessions = CinemaEntities.GetContext().Session.Where(s => s.Date == date).ToList();
            }
            else if (hall != null)
            {
                sessions = CinemaEntities.GetContext().Session.Where(s => s.Hall.Name == hall).ToList();
            }
            else
            {
                sessions = CinemaEntities.GetContext().Session.ToList();
            }

            DataGridSession.ItemsSource = sessions;
            ComboBoxDate.SelectedIndex = -1;
            ComboBoxHall.SelectedIndex = -1;
        }
    }
}
