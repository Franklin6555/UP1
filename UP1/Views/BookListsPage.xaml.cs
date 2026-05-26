using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using UP1.Models;
using UP1.Services;
using UP1.Windows;

namespace UP1.Views
{
    public partial class BookListsPage : Page
    {
        private string currentStatus = "В планах"; // статус по умолчанию

        public BookListsPage()
        {
            InitializeComponent();
            LoadCurrentShelf();
        }

        private void LoadCurrentShelf()
        {
            if (MainWindow.CurrentUser == null) return;

            var books = App.DataService.GetBooksOnShelf(MainWindow.CurrentUser.Id, currentStatus);
            DisplayBooks(books);
        }

        private void DisplayBooks(List<Book> books)
        {
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

            TextBlock cover = new TextBlock { Text = book.CoverPath ?? "📖", FontSize = 65, HorizontalAlignment = HorizontalAlignment.Center };
            TextBlock title = new TextBlock { Text = book.Title, FontWeight = FontWeights.Bold, TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 10, 0, 5) };
            TextBlock author = new TextBlock { Text = book.Author?.DisplayName ?? "Неизвестен", Foreground = Brushes.LightGray, FontSize = 12 };
            TextBlock genre = new TextBlock { Text = book.Genre ?? "", Foreground = Brushes.Gold, FontSize = 11 };

            panel.Children.Add(cover);
            panel.Children.Add(title);
            panel.Children.Add(author);
            panel.Children.Add(genre);

            border.Child = panel;

            ContextMenu menu = new ContextMenu();

            menu.Items.Add(CreateMenuItem("📚 В планах", () => ChengeBookShelf(book.Id, "В планах")));
            menu.Items.Add(CreateMenuItem("📖 Читаю", () => ChengeBookShelf(book.Id, "Читаю")));
            menu.Items.Add(CreateMenuItem("✅ Прочитано", () => ChengeBookShelf(book.Id, "Прочитано")));
            menu.Items.Add(CreateMenuItem("🗑 Заброшено", () => ChengeBookShelf(book.Id, "Заброшено")));

            border.ContextMenu = menu;

            // Открытие страницы книги по клику
            border.MouseLeftButtonUp += (sender, e) =>
            {
                NavigationService.Navigate(new BookDetailsPage(book));
            };

            return border;
        }
        public void ChengeBookShelf(int bookId, string statusName)
        {
            bool success = App.DataService.AddBookToShelf(MainWindow.CurrentUser.Id, bookId, statusName);

            if (success)
            {
                MessageBox.Show($"Книга пернесена в список: {statusName}", "Успешно");
            }
            else
            {
                MessageBox.Show("Не удалось пернесети книгу в список.", "Ошибка");
            }
            LoadCurrentShelf();
        }
        private MenuItem CreateMenuItem(string header, Action action)
        {
            MenuItem item = new MenuItem { Header = header };
            item.Click += (s, e) => action();
            return item;
        }

        // ================== КНОПКИ ФИЛЬТРАЦИИ ==================
        private void BtnPlan_Click(object sender, RoutedEventArgs e) => ChangeShelf("В планах");
        private void BtnReading_Click(object sender, RoutedEventArgs e) => ChangeShelf("Читаю");
        private void BtnFinished_Click(object sender, RoutedEventArgs e) => ChangeShelf("Прочитано");
        private void BtnDropped_Click(object sender, RoutedEventArgs e) => ChangeShelf("Заброшено");

        private void ChangeShelf(string status)
        {
            currentStatus = status;
            LoadCurrentShelf();
        }
    }
}