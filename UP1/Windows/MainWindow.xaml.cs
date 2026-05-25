using System.Windows;
using System.Windows.Controls;
using UP1.Models;
using UP1.Views;

namespace UP1.Windows
{
    public partial class MainWindow : Window
    {
        public static User CurrentUser { get; private set; }

        public MainWindow(User user)
        {
            InitializeComponent();
            CurrentUser = user;
            ApplyRoleVisibility();
            LoadDefaultPage();
        }

        private void ApplyRoleVisibility()
        {
            if (CurrentUser == null) return;

            string roleName = CurrentUser.Role?.Name ?? "User";

            btnAuthor.Visibility = (roleName == "Author" || roleName == "Administrator")
                                 ? Visibility.Visible : Visibility.Collapsed;

            btnAdmin.Visibility = roleName == "Administrator"
                                ? Visibility.Visible : Visibility.Collapsed;

            btnFreezeWarning.Visibility = CurrentUser.IsFrozen ? Visibility.Visible : Visibility.Collapsed;
        }

        private void LoadDefaultPage()
        {
            ContentFrame.Navigate(new BookCatalogPage());
            tbPageTitle.Text = "Каталог книг";
        }

        private void BtnCatalog_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new BookCatalogPage());
            tbPageTitle.Text = "Каталог книг";
        }

        private void BtnLists_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new BookListsPage());
            tbPageTitle.Text = "Мои списки книг";
        }

        private void BtnAuthor_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new AuthorPage());
            tbPageTitle.Text = "Страница автора";
        }

        private void BtnAdmin_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new AdminPage());
            tbPageTitle.Text = "Администрирование";
        }

        private void BtnProfile_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new ProfilePage());
            tbPageTitle.Text = "Профиль";
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Выйти из аккаунта?", "Выход", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                LoginWindow login = new LoginWindow();
                login.Show();
                this.Close();
            }
        }
        private void BtnFreezeWarning_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentUser?.IsFrozen == true)
            {
                MessageBox.Show($"Ваш аккаунт заморожен!\n\nПричина: {CurrentUser.FreezeReason ?? "Не указана"}",
                                "Предупреждение",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}