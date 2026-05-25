using System.Windows;
using System.Windows.Controls;
using UP1.Models;
using UP1.Services;

namespace UP1.Views
{
    public partial class AdminPage : Page
    {
        public AdminPage()
        {
            InitializeComponent();
            LoadAllData();
        }

        private void LoadAllData()
        {
            lbUsers.Items.Clear();

            var users = App.DataService.GetAllUsers();

            foreach (User user in users)
            {
                string roleName = user.Role?.Name ?? user.Role?.ToString() ?? "User";
                lbUsers.Items.Add($"{user.DisplayName} ({user.Login}) — {roleName}");
            }
        }

        private void BtnChangeRole_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Функция смены роли будет реализована позже.", "Информация");
        }

        private void BtnResetPassword_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Пароль успешно сброшен (прототип).", "Успешно");
        }

        private void BtnUnfreeze_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Элемент разморожен (прототип).", "Успешно");
        }

        private void BtnAcceptAuthor_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Заявка на роль автора принята.", "Успешно");
        }

        private void BtnRejectAuthor_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Заявка отклонена.", "Отклонено");
        }

        private void BtnReviewComplaint_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Жалоба рассмотрена.", "Готово");
        }

        private void BtnRejectComplaint_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Жалоба отклонена.", "Отклонено");
        }
    }
}