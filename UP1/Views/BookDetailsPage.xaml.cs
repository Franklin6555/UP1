using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UP1.Models;
using UP1.Services;
using UP1.Windows;

namespace UP1.Views
{
    public partial class BookDetailsPage : Page
    {
        private Book currentBook;

        public BookDetailsPage(Book book)
        {
            InitializeComponent();
            currentBook = book;
            LoadBookInfo();
            LoadReviews();
        }

        private void LoadBookInfo()
        {
            tbCover.Text = currentBook.CoverPath ?? "📖";
            tbTitle.Text = currentBook.Title;
            tbAuthor.Text = "Автор: " + (currentBook.Author?.DisplayName ?? "Неизвестен");
            tbRating.Text = $"⭐ {currentBook.Rating}";
            tbDescription.Text = currentBook.Description ?? "Описание отсутствует.";

            btnFreezeBook.Visibility = (MainWindow.CurrentUser?.Role?.Name == "Administrator")
                                     ? Visibility.Visible : Visibility.Collapsed;
        }

        private void LoadReviews()
        {
            reviewsPanel.Children.Clear();
            // Заглушка
            var tb = new TextBlock
            {
                Text = "Пока нет отзывов.",
                Foreground = System.Windows.Media.Brushes.Gray,
                Margin = new Thickness(10)
            };
            reviewsPanel.Children.Add(tb);
        }

        private void BtnRead_Click(object sender, RoutedEventArgs e)
        {
            ReadBookWindow readWindow = new ReadBookWindow(currentBook);
            readWindow.ShowDialog();
        }

        private void BtnPublishReview_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtReviewText.Text)) return;

            MessageBox.Show("Отзыв сохранён (EF версия в разработке)", "Успешно");
            txtReviewText.Clear();
        }

        private void BtnFreezeBook_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Функция заморозки книги (EF)", "Заморозка");
        }
        private void BtnReportBook_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Жалоба на книгу отправлена администратору.", "Жалоба отправлена");
        }
    }
}