using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    /// Логика взаимодействия для FilmsWindow.xaml
    /// </summary>
    public partial class FilmsWindow : Window
    {
        public string action = "";
        List<Film> items = null;
        public FilmsWindow()
        {
            InitializeComponent();
            items = CinemaEntities.GetContext().Film.ToList();
            DataGridFilm.ItemsSource = items;

            var context = CinemaEntities.GetContext();

            var genre = context.Film.Select(film => film.Genre).ToList();
            genre.Insert(0, "Все жанры");

            ComboBoxGenre.ItemsSource = genre;
            ComboBoxGenre.SelectedIndex = 0;
        }

        public bool isDirty = true;

        private void Undo_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var context = CinemaEntities.GetContext();
            switch (action)
            {
                case "add":
                    var addFilm = context.Film.Local.Last();
                    if (addFilm != null)
                    {
                        context.Film.Remove(addFilm);
                        context.SaveChanges();
                        DataGridFilm.ItemsSource = null;
                        DataGridFilm.ItemsSource = context.Film.ToList();
                        MessageBox.Show("Последнее добавление фильма было отменено");
                    }
                    break;
                case "undo":
                    DataGridFilm.ItemsSource = null;
                    DataGridFilm.ItemsSource = CinemaEntities.GetContext().Film.ToList();
                    break;
                case "":
                    DataGridFilm.ItemsSource = CinemaEntities.GetContext().Film.ToList();
                    break;
            }

            MessageBox.Show("Отмена действий");
            isDirty = true;
            DataGridFilm.IsReadOnly = true;
        }

        private void Undo_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !isDirty;
        }

        private void Edit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DataGridFilm.ItemsSource = null;

            DataGridFilm.ItemsSource = items;

            DataGridFilm.IsReadOnly = false;
            DataGridFilm.BeginEdit();
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

            var items = CinemaEntities.GetContext().Film.ToList();
            DataGridFilm.ItemsSource = items;

            DataGridFilm.IsReadOnly = false;
            int maxId = CinemaEntities.GetContext().Film.ToList().Max(i => i.ID_Film);
            Film film = new Film()
            {
                ID_Film = maxId + 1,
                Name = "Введите название фильма",
                Genre = "Введите описание",
                Director = "Введите режисёра",
                Duration = 0,
                Year_of_release = System.DateTime.Now,
            };
            CinemaEntities.GetContext().Film.Add(film);
            try
            {
                CinemaEntities.GetContext().SaveChanges();
                DataGridFilm.ItemsSource = null;
                items = CinemaEntities.GetContext().Film.ToList();
                DataGridFilm.ItemsSource = items;
                DataGridFilm.Items.Refresh();
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
            DataGridFilm.ItemsSource = CinemaEntities.GetContext().Film.ToList();
            DataGridFilm.Items.Refresh();
            DataGridFilm.IsReadOnly = true;
            action = "";
            isDirty = true;
        }

        private void Save_CanExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !isDirty;
        }

        private void Find_Button_Click(object sender, RoutedEventArgs e)
        {
            if (TextBoxFilm.Text == "" && ComboBoxGenre.SelectedValue.ToString() == "Все жанры")
            {
                items = CinemaEntities.GetContext().Film.ToList();
                DataGridFilm.ItemsSource = items;
                return;
            }
            //Вывод всех фильмов с определённым названием
            else if (TextBoxFilm.Text != "" && ComboBoxGenre.SelectedValue.ToString() == "Все жанры")
            {
                string searchName = TextBoxFilm.Text;
                items = CinemaEntities.GetContext().Film.Where(r => r.Name.StartsWith(searchName)).ToList();
                if (items.Count == 0)
                {
                    MessageBox.Show("Такого фильма не существует, попробуйте снова");
                    return;
                }
                else
                {
                    DataGridFilm.ItemsSource = items;
                    return;
                }
            }
            //Вывод всех фильмов определенного жанра
            else if (TextBoxFilm.Text == "" && ComboBoxGenre.SelectedValue.ToString() != "Все жанры")
            {
                items = CinemaEntities.GetContext().Film.Where(r => r.Genre == ComboBoxGenre.SelectedValue.ToString()).ToList();
                if (items.Count == 0)
                {
                    MessageBox.Show($"Нету фильмов жанра {ComboBoxGenre.SelectedValue.ToString()}");
                    return;
                }
                else
                {
                    DataGridFilm.ItemsSource = items;
                    return;
                }
            }
            //Вывод определённого фильма определённого жанра
            else if (TextBoxFilm.Text != "" && ComboBoxGenre.SelectedValue.ToString() != "Все жанры")
            {
                string searchName = TextBoxFilm.Text;
                string selectedBrand = ComboBoxGenre.SelectedValue.ToString();
                items = CinemaEntities.GetContext().Film.Where(r => r.Genre.ToString() == selectedBrand && r.Name.StartsWith(searchName)).ToList();
                if (items.Count == 0)
                {
                    MessageBox.Show($"Нету такого фильма жанра {ComboBoxGenre.SelectedValue.ToString()}");
                    return;
                }
                else
                {
                    DataGridFilm.ItemsSource = items;
                    return;
                }

            }
        }

        private void Find_CanExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = isDirty;
        }

        private void Delete_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            
            var items = CinemaEntities.GetContext().Film.ToList();

            Film film = DataGridFilm.SelectedItem as Film;

                MessageBoxResult result = MessageBox.Show("Удалить данные ",
               "Предупреждение", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    DataGridFilm.SelectedIndex =
                    DataGridFilm.SelectedIndex == 0 ? 1 :
                    DataGridFilm.SelectedIndex - 1;
                    CinemaEntities.GetContext().Film.Remove(film);
                    CinemaEntities.GetContext().SaveChanges();
                    DataGridFilm.ItemsSource = CinemaEntities.GetContext().Film.ToList();
                    DataGridFilm.Items.Refresh();
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
            if (TextBoxFilm.Text == "" && ComboBoxGenre.SelectedValue.ToString() == "Все жанры")
            {
                items = CinemaEntities.GetContext().Film.ToList();
                DataGridFilm.ItemsSource = items;
                return;
            }
            //Вывод всех фильмов с определённым названием
            else if (TextBoxFilm.Text != "" && ComboBoxGenre.SelectedValue.ToString() == "Все жанры")
            {
                string searchName = TextBoxFilm.Text;
                items = CinemaEntities.GetContext().Film.Where(r => r.Name.StartsWith(searchName)).ToList();
                if (items.Count == 0)
                {
                    MessageBox.Show("Такого фильма не существует, попробуйте снова");
                    return;
                }
                else
                {
                    DataGridFilm.ItemsSource = items;
                    return;
                }
            }
            //Вывод всех фильмов определенного жанра
            else if (TextBoxFilm.Text == "" && ComboBoxGenre.SelectedValue.ToString() != "Все жанры")
            {
                items = CinemaEntities.GetContext().Film.Where(r => r.Genre == ComboBoxGenre.SelectedValue.ToString()).ToList();
                if (items.Count == 0)
                {
                    MessageBox.Show($"Нету фильмов жанра {ComboBoxGenre.SelectedValue.ToString()}");
                    return;
                }
                else
                {
                    DataGridFilm.ItemsSource = items;
                    return;
                }
            }
            //Вывод определённого фильма определённого жанра
            else if (TextBoxFilm.Text != "" && ComboBoxGenre.SelectedValue.ToString() != "Все жанры")
            {
                string searchName = TextBoxFilm.Text;
                string selectedBrand = ComboBoxGenre.SelectedValue.ToString();
                items = CinemaEntities.GetContext().Film.Where(r => r.Genre.ToString() == selectedBrand && r.Name.StartsWith(searchName)).ToList();
                if (items.Count == 0)
                {
                    MessageBox.Show($"Нету такого фильма жанра {ComboBoxGenre.SelectedValue.ToString()}");
                    return;
                }
                else
                {
                    DataGridFilm.ItemsSource = items;
                    return;
                }

            }
        }
    }
}
