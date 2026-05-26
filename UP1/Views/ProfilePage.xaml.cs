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
            if (MainWindow.CurrentUser == null) return;

            if (MainWindow.CurrentUser.HasAuthorRequest)
            {
                MessageBox.Show("Вы уже подавали заявку.\nОжидайте решения администратора.",
                               "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show(
                "Вы действительно хотите подать заявку на роль Автора?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                bool success = App.DataService.SubmitAuthorRequest(MainWindow.CurrentUser.Id,
                    "Хочу публиковать свои книги"); // стандартная причина

                if (success)
                {
                    MessageBox.Show("Заявка на роль Автора успешно отправлена!\nОжидайте решения администратора.",
                                   "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadUserInfo(); // обновляем информацию на странице
                }
                else
                {
                    MessageBox.Show("Не удалось отправить заявку. Возможно, вы уже отправляли её ранее.",
                                   "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
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

            if (btnApplyAuthor != null)
            {
                string roleName = MainWindow.CurrentUser.Role?.Name ?? "User";

                btnApplyAuthor.Visibility = (roleName == "User")
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

    }
}