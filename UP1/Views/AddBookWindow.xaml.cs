using System.Windows;
using UP1.Models;
using UP1.Services;

namespace UP1.Views
{
    public partial class AddBookWindow : Window
    {
        public AddBookWindow()
        {
            InitializeComponent();
        }

        private void BtnPublish_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Название книги обязательно!");
                return;
            }

            var newBook = new Book
            {
                Title = txtTitle.Text,
                Description = txtDescription.Text,
                Content = txtText.Text,
                CoverPath = "📖",
                Rating = 0
            };

            App.DataService.AddBook(newBook);

            MessageBox.Show("Книга успешно опубликована!", "Успех");
            this.DialogResult = true;
            this.Close();
        }
    }
}