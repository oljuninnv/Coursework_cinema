using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Xml.Linq;

namespace Cinema
{
    /// <summary>
    /// Логика взаимодействия для UserWindow.xaml
    /// </summary>
    public partial class UserWindow : Window
    {
        List<Film> items = null;

        public UserWindow()
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
    }
}
