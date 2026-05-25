using System.Windows;
using UP1.Models;
using UP1.Services;

namespace UP1.Views
{
    public partial class EditBookWindow : Window
    {
        private Book currentBook;

        public EditBookWindow(Book book)
        {
            InitializeComponent();
            currentBook = book;
            LoadBookData();
        }

        private void LoadBookData()
        {
            txtTitle.Text = currentBook.Title;
            txtDescription.Text = currentBook.Description;
            txtText.Text = currentBook.Content;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Название обязательно!");
                return;
            }

            currentBook.Title = txtTitle.Text;
            currentBook.Description = txtDescription.Text;
            currentBook.Content = txtText.Text;

            App.DataService.UpdateBook(currentBook);

            MessageBox.Show("Книга успешно обновлена!", "Успех");
            this.DialogResult = true;
            this.Close();
        }
    }
}