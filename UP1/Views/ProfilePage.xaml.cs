using System.Windows;
using System.Windows.Controls;
using UP1.Models;
using UP1.Windows;

namespace UP1.Views
{
    public partial class ProfilePage : Page
    {
        public ProfilePage()
        {
            InitializeComponent();
            LoadUserInfo();
        }
        private void BtnApplyAuthor_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Заявка на роль Автора отправлена администратору!", "Успешно");
        }
        private void LoadUserInfo()
        {
            var user = MainWindow.CurrentUser;
            if (user == null) return;

            tbFullName.Text = $"Имя: {user.DisplayName}";
            tbLogin.Text = $"Логин: {user.Login}";
            tbEmail.Text = $"Email: {user.Email}";
            tbRole.Text = $"Роль: {user.Role?.Name ?? user.Role?.ToString() ?? "User"}";

            if (user.IsFrozen)
            {
                tbFreezeWarning.Visibility = Visibility.Visible;
                tbFreezeWarning.Text = $"⚠️ Аккаунт заморожен!\nПричина: {user.FreezeReason}";
            }
        }

    }
}