using System.Windows;
using UP1.Models;

namespace UP1.Views
{
    public partial class ReadBookWindow : Window
    {
        public ReadBookWindow(Book book)
        {
            InitializeComponent();
            tbBookTitle.Text = book.Title;
            tbBookText.Text = book.Text ?? "Текст книги пока не загружен...";
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}