using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using UP1.Models;
using UP1.Services;
using UP1.Windows;

namespace UP1.Views
{
    public partial class BookCatalogPage : Page
    {
        private List<Book> allBooks = new List<Book>();
        private List<string> selectedGenres = new List<string>();

        public BookCatalogPage()
        {
            InitializeComponent();
            LoadBooks();
            LoadGenreFilters(); 
        }
        private void LoadGenreFilters()
        {
            genresPanel.Children.Clear();
            var genres = allBooks.Select(b => b.Genre)
                                 .Where(g => !string.IsNullOrEmpty(g))
                                 .Distinct()
                                 .OrderBy(g => g)
                                 .ToList();

            foreach (var genre in genres)
            {
                CheckBox cb = new CheckBox
                {
                    Content = genre,
                    Margin = new Thickness(0, 0, 15, 0),
                    Foreground = System.Windows.Media.Brushes.White,
                    FontSize = 14
                };
                cb.Checked += Genre_Checked;
                cb.Unchecked += Genre_Unchecked;
                genresPanel.Children.Add(cb);
            }
        }

        private void Genre_Checked(object sender, RoutedEventArgs e)
        {
            var cb = sender as CheckBox;
            if (cb != null && !selectedGenres.Contains(cb.Content.ToString()))
            {
                selectedGenres.Add(cb.Content.ToString());
            }
            ApplyFilters();
        }

        private void Genre_Unchecked(object sender, RoutedEventArgs e)
        {
            var cb = sender as CheckBox;
            selectedGenres.Remove(cb.Content.ToString());
            ApplyFilters();
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void CmbSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            var filtered = allBooks.AsQueryable();

            // Поиск по названию и автору
            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                string search = txtSearch.Text.ToLower();
                filtered = filtered.Where(b =>
                    b.Title.ToLower().Contains(search) ||
                    (b.Author != null && b.Author.DisplayName != null &&
                     b.Author.DisplayName.ToLower().Contains(search)));
            }

            // Фильтр по жанрам
            if (selectedGenres.Any())
            {
                filtered = filtered.Where(b => selectedGenres.Contains(b.Genre));
            }

            // Сортировка
            var sortItem = cmbSort.SelectedItem as ComboBoxItem;
            string sortMode = sortItem?.Content.ToString();

            switch (sortMode)
            {
                case "По названию (А-Я)":
                    filtered = filtered.OrderBy(b => b.Title);
                    break;
                case "По названию (Я-А)":
                    filtered = filtered.OrderByDescending(b => b.Title);
                    break;
                case "По оценке (высокая)":
                    filtered = filtered.OrderByDescending(b => b.Rating);
                    break;
                case "По оценке (низкая)":
                    filtered = filtered.OrderBy(b => b.Rating);
                    break;
            }

            DisplayBooks(filtered.ToList());
        }
        public void AddBookToShelf(int bookId, string statusName)
        {
            if (MainWindow.CurrentUser == null)
            {
                MessageBox.Show("Войдите в аккаунт для добавления книг в списки.", "Ошибка");
                return;
            }

            bool success = App.DataService.AddBookToShelf(MainWindow.CurrentUser.Id, bookId, statusName);

            if (success)
            {
                MessageBox.Show($"Книга добавлена в список: **{statusName}**", "Успешно");
            }
            else
            {
                MessageBox.Show("Не удалось добавить книгу в список.", "Ошибка");
            }
        }
        private void LoadBooks()
        {
            allBooks = App.DataService.GetAllBooks();
            DisplayBooks(allBooks);
        }

        private void DisplayBooks(List<Book> books)
        {
            if (booksPanel == null) return;
            booksPanel.Children.Clear();

            foreach (var book in books)
            {
                var card = CreateBookCard(book);
                booksPanel.Children.Add(card);
            }
        }

        private Border CreateBookCard(Book book)
        {
            Border border = new Border
            {
                Width = 170,
                Height = 255,
                Background = new SolidColorBrush(Color.FromRgb(45, 45, 45)),
                CornerRadius = new CornerRadius(10),
                Margin = new Thickness(8),
                Cursor = Cursors.Hand
            };

            StackPanel panel = new StackPanel { Margin = new Thickness(10) };

            // Обложка
            TextBlock cover = new TextBlock
            {
                Text = book.CoverPath ?? "📖",
                FontSize = 65,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 5, 0, 10)
            };

            // Название книги
            TextBlock title = new TextBlock
            {
                Text = book.Title,
                FontWeight = FontWeights.Bold,
                FontSize = 14,
                TextWrapping = TextWrapping.Wrap,
                TextAlignment = TextAlignment.Center,
                Height = 40,
                Margin = new Thickness(0, 0, 0, 5)
            };

            // Автор
            TextBlock author = new TextBlock
            {
                Text = book.Author?.DisplayName ?? "Неизвестный автор",
                Foreground = new SolidColorBrush(Color.FromRgb(170, 170, 170)),
                FontSize = 12,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 8)
            };

            // Жанр
            TextBlock genre = new TextBlock
            {
                Text = book.Genre ?? "Без жанра",
                Foreground = new SolidColorBrush(Color.FromRgb(255, 215, 0)), // Gold
                FontSize = 11,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 10)
            };

            // Рейтинг
            TextBlock rating = new TextBlock
            {
                Text = $"⭐ {book.Rating:F1}",
                Foreground = new SolidColorBrush(Colors.Orange),
                FontSize = 13,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            panel.Children.Add(cover);
            panel.Children.Add(title);
            panel.Children.Add(author);
            panel.Children.Add(genre);
            panel.Children.Add(rating);

            border.Child = panel;

            ContextMenu menu = new ContextMenu();

            menu.Items.Add(CreateMenuItem("📚 В планах", () => AddBookToShelf(book.Id, "В планах")));
            menu.Items.Add(CreateMenuItem("📖 Читаю", () => AddBookToShelf(book.Id, "Читаю")));
            menu.Items.Add(CreateMenuItem("✅ Прочитано", () => AddBookToShelf(book.Id, "Прочитано")));
            menu.Items.Add(CreateMenuItem("🗑 Заброшено", () => AddBookToShelf(book.Id, "Заброшено")));

            border.ContextMenu = menu;

            // Открытие страницы книги по клику
            border.MouseLeftButtonUp += (sender, e) =>
            {
                NavigationService.Navigate(new BookDetailsPage(book));
            };

            // Hover-эффект
            border.MouseEnter += (s, e) =>
            {
                border.Background = new SolidColorBrush(Color.FromRgb(60, 60, 60));
            };

            border.MouseLeave += (s, e) =>
            {
                border.Background = new SolidColorBrush(Color.FromRgb(45, 45, 45));
            };

            return border;
        }
        private MenuItem CreateMenuItem(string header, Action action)
        {
            MenuItem item = new MenuItem { Header = header };
            item.Click += (s, e) => action();
            return item;
        }
    }
}