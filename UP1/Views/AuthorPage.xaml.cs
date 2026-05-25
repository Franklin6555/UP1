using System.Windows;
using System.Windows.Controls;
using UP1.Models;
using UP1.Services;

namespace UP1.Views
{
    public partial class AuthorPage : Page
    {
        public AuthorPage()
        {
            InitializeComponent();
            LoadAuthorBooks();
        }

        private void LoadAuthorBooks()
        {
            authorBooksPanel.Children.Clear();
            var books = App.DataService.GetAllBooks();

            foreach (var book in books)
            {
                var card = CreateBookCard(book);
                authorBooksPanel.Children.Add(card);
            }
        }

        private Border CreateBookCard(Book book)
        {
            var border = new Border
            {
                Width = 170,
                Height = 290,
                Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(45, 45, 48)),
                CornerRadius = new CornerRadius(10),
                Margin = new Thickness(12),
                Cursor = System.Windows.Input.Cursors.Hand
            };

            var stack = new StackPanel { Margin = new Thickness(10) };

            var cover = new TextBlock { Text = book.CoverPath ?? "📖", FontSize = 60, HorizontalAlignment = HorizontalAlignment.Center };
            var title = new TextBlock { Text = book.Title, FontWeight = FontWeights.Bold, TextWrapping = TextWrapping.Wrap, TextAlignment = TextAlignment.Center, Foreground = System.Windows.Media.Brushes.White };

            var btnEdit = new Button
            {
                Content = "✏️ Редактировать",
                Height = 30,
                Margin = new Thickness(0, 8, 0, 0),
                Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 152, 0)),
                Foreground = System.Windows.Media.Brushes.White
            };

            stack.Children.Add(cover);
            stack.Children.Add(title);
            stack.Children.Add(btnEdit);

            border.Child = stack;

            border.MouseLeftButtonUp += (s, e) =>
            {
                if (s != btnEdit)
                    NavigationService.Navigate(new BookDetailsPage(book));
            };

            btnEdit.Click += (s, e) =>
            {
                var editWindow = new EditBookWindow(book);
                if (editWindow.ShowDialog() == true)
                    LoadAuthorBooks();
            };

            return border;
        }

        private void BtnAddNewBook_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddBookWindow();
            if (addWindow.ShowDialog() == true)
                LoadAuthorBooks();
        }
    }
}