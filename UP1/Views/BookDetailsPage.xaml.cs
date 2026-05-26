using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
            if (currentBook.IsFrozen)
            {
                btnFreezeBook.Content = "❄️ Разморозить книгу";
            }
            else
            {
                btnFreezeBook.Content = "❄️ Заморозить книгу";
            }
        }
        private void BtnRead_Click(object sender, RoutedEventArgs e)
        {
            ReadBookWindow readWindow = new ReadBookWindow(currentBook);
            readWindow.ShowDialog();
        }

        private void LoadReviews()
        {
            reviewsPanel.Children.Clear();

            var reviews = App.DataService.GetReviewsForBook(currentBook.Id);

            if (!reviews.Any())
            {
                var empty = new TextBlock
                {
                    Text = "Пока нет отзывов. Будьте первым!",
                    Foreground = Brushes.Gray,
                    FontStyle = FontStyles.Italic,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(20)
                };
                reviewsPanel.Children.Add(empty);
                return;
            }

            foreach (var review in reviews)
            {
                Border reviewBorder = new Border
                {
                    Background = new SolidColorBrush(Color.FromRgb(40, 40, 40)),
                    CornerRadius = new CornerRadius(8),
                    Margin = new Thickness(0, 0, 0, 12),
                    Padding = new Thickness(12)
                };

                StackPanel reviewPanel = new StackPanel();

                // Заголовок отзыва
                StackPanel header = new StackPanel { Orientation = Orientation.Horizontal };
                header.Children.Add(new TextBlock
                {
                    Text = review.UserLogin,
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.White
                });
                header.Children.Add(new TextBlock
                {
                    Text = $"  •  {new string('★', review.Rating)}",
                    Foreground = Brushes.Gold,
                    Margin = new Thickness(8, 0, 0, 0)
                });

                reviewPanel.Children.Add(header);

                // Текст отзыва
                TextBlock textBlock = new TextBlock
                {
                    Text = review.Text,
                    TextWrapping = TextWrapping.Wrap,
                    Foreground = Brushes.LightGray,
                    Margin = new Thickness(0, 8, 0, 0)
                };

                reviewPanel.Children.Add(textBlock);

                reviewBorder.Child = reviewPanel;
                reviewsPanel.Children.Add(reviewBorder);
            }
        }

        private void BtnPublishReview_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtReviewText.Text))
            {
                MessageBox.Show("Напишите текст отзыва!", "Ошибка");
                return;
            }

            int rating = int.Parse(((ComboBoxItem)cmbRating.SelectedItem).Content.ToString().Substring(0, 1));

            var review = new Review
            {
                BookId = currentBook.Id,
                UserId = MainWindow.CurrentUser.Id,
                UserLogin = MainWindow.CurrentUser.DisplayName,
                Text = txtReviewText.Text.Trim(),
                Rating = rating
            };

            if (App.DataService.AddReview(review))
            {
                MessageBox.Show("Отзыв успешно опубликован!", "Спасибо");
                txtReviewText.Clear();
                LoadReviews();           // обновляем список
                LoadBookInfo();          // обновляем средний рейтинг
            }
            else
            {
                MessageBox.Show("Не удалось опубликовать отзыв.", "Ошибка");
            }
        }

        private void BtnFreezeBook_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.CurrentUser?.Role?.Name != "Administrator")
            {
                MessageBox.Show("Только администратор может замораживать/размораживать книги.",
                               "Доступ запрещён");
                return;
            }

            if (currentBook.IsFrozen)
            {
                // Разморозка
                bool success = App.DataService.UnfreezeBook(currentBook.Id);
                if (success)
                {
                    currentBook.IsFrozen = false;
                    btnFreezeBook.Content = "❄️ Заморозить книгу";
                    MessageBox.Show("Книга успешно разморожена.", "Успешно");
                }
            }
            else
            {
                // Заморозка
                bool success = App.DataService.FreezeBook(currentBook.Id, "Заморожено администратором");
                if (success)
                {
                    currentBook.IsFrozen = true;
                    btnFreezeBook.Content = "❄️ Разморозить книгу";
                    MessageBox.Show("Книга успешно заморожена.", "Успешно");
                }
            }
        }
        private void BtnReportBook_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.CurrentUser == null)
            {
                MessageBox.Show("Войдите в аккаунт, чтобы отправить жалобу.", "Ошибка");
                return;
            }

            var result = MessageBox.Show(
                $"Отправить жалобу на книгу «{currentBook.Title}»?",
                "Подтверждение жалобы",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                bool success = App.DataService.AddComplaint(
                    MainWindow.CurrentUser.Id,
                    currentBook.Id);

                if (success)
                {
                    MessageBox.Show("Жалоба успешно отправлена администратору!", "Спасибо");
                }
                else
                {
                    MessageBox.Show("Не удалось отправить жалобу. Попробуйте позже.", "Ошибка");
                }
            }
        }
    }
}