using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UP1.Models;
using UP1.Services;

namespace UP1.Views
{
    public partial class BookCatalogPage : Page
    {
        private List<Book> allBooks = new List<Book>();

        public BookCatalogPage()
        {
            InitializeComponent();
            LoadBooks();
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
            var border = new Border
            {
                Width = 170,
                Height = 280,
                Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(45, 45, 48)),
                CornerRadius = new CornerRadius(10),
                Margin = new Thickness(12),
                Cursor = System.Windows.Input.Cursors.Hand
            };

            var stack = new StackPanel { Margin = new Thickness(10) };

            var cover = new TextBlock
            {
                Text = book.CoverPath ?? "📖",
                FontSize = 65,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            var title = new TextBlock
            {
                Text = book.Title,
                FontWeight = FontWeights.Bold,
                TextWrapping = TextWrapping.Wrap,
                TextAlignment = TextAlignment.Center,
                Foreground = System.Windows.Media.Brushes.White
            };

            var author = new TextBlock
            {
                Text = book.Author?.DisplayName ?? book.Author?.ToString() ?? "Неизвестен",
                TextAlignment = TextAlignment.Center,
                Foreground = System.Windows.Media.Brushes.LightGray
            };

            var rating = new TextBlock
            {
                Text = $"⭐ {book.Rating:F1}",
                Foreground = System.Windows.Media.Brushes.Gold,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            stack.Children.Add(cover);
            stack.Children.Add(title);
            stack.Children.Add(author);
            stack.Children.Add(rating);

            border.Child = stack;

            border.MouseLeftButtonUp += (s, e) => NavigationService.Navigate(new BookDetailsPage(book));

            return border;
        }
    }
}