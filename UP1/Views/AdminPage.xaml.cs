using System.Linq;
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
            LoadUsers();
            LoadAuthorRequests();
        }

        // ПОЛЬЗОВАТЕЛИ
        private void LoadUsers()
        {
            if (lbUsers == null) return;
            lbUsers.Items.Clear();

            var users = App.DataService.GetAllUsers();

            foreach (var user in users)
            {
                string role = user.Role?.Name ?? "User";
                lbUsers.Items.Add(new ListBoxItem
                {
                    Content = $"{user.DisplayName} ({user.Login}) — {role}",
                    Tag = user.Id
                });
            }
        }

        // ЗАЯВКИ НА АВТОРА
        private void LoadAuthorRequests()
        {
            if (lbAuthorRequests == null) return;
            lbAuthorRequests.Items.Clear();

            var requests = App.DataService.GetAuthorRequests();

            foreach (var user in requests)
            {
                lbAuthorRequests.Items.Add($"{user.DisplayName} ({user.Login}) — {user.AuthorRequestReason}");
            }
        }

        private void BtnAcceptAuthor_Click(object sender, RoutedEventArgs e)
        {
            if (lbAuthorRequests.SelectedItem == null)
            {
                MessageBox.Show("Выберите заявку из списка!", "Предупреждение");
                return;
            }

            string selectedText = lbAuthorRequests.SelectedItem.ToString();
            string login = selectedText.Split('(')[1].Split(')')[0];

            var user = App.DataService.GetAllUsers().FirstOrDefault(u => u.Login == login);

            if (user != null)
            {
                bool success = App.DataService.ProcessAuthorRequest(user.Id, true);
                if (success)
                {
                    MessageBox.Show($"Заявка пользователя {user.DisplayName} одобрена!\nТеперь он Автор.", "Успешно");
                    LoadAllData();
                }
            }
        }

        private void BtnRejectAuthor_Click(object sender, RoutedEventArgs e)
        {
            if (lbAuthorRequests.SelectedItem == null)
            {
                MessageBox.Show("Выберите заявку из списка!", "Предупреждение");
                return;
            }

            string selectedText = lbAuthorRequests.SelectedItem.ToString();
            string login = selectedText.Split('(')[1].Split(')')[0];

            var user = App.DataService.GetAllUsers().FirstOrDefault(u => u.Login == login);

            if (user != null)
            {
                bool success = App.DataService.ProcessAuthorRequest(user.Id, false);
                if (success)
                {
                    MessageBox.Show($"Заявка пользователя {user.DisplayName} отклонена.", "Отклонено");
                    LoadAllData();
                }
            }
        }

        // СМЕНА РОЛИ
        private void BtnChangeRole_Click(object sender, RoutedEventArgs e)
        {
            if (lbUsers.SelectedItem == null)
            {
                MessageBox.Show("Сначала выберите пользователя из списка!", "Предупреждение");
                return;
            }

            if (cmbNewRole.SelectedItem == null)
            {
                MessageBox.Show("Выберите новую роль!", "Предупреждение");
                return;
            }

            string newRole = (cmbNewRole.SelectedItem as ComboBoxItem)?.Content.ToString();

            var selectedItem = lbUsers.SelectedItem as ListBoxItem;
            int userId = (int)selectedItem.Tag;

            bool success = App.DataService.ChangeUserRole(userId, newRole);

            if (success)
            {
                MessageBox.Show($"Роль успешно изменена на {newRole}!", "Успешно");
                LoadUsers();
            }
            else
            {
                MessageBox.Show("Не удалось изменить роль пользователя.", "Ошибка");
            }
        }

        private void BtnResetPassword_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Пароль успешно сброшен (прототип).", "Успешно");
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Пользователь заморожен (прототип).", "Успешно");
        }
        private void BtnUnfreeze_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Элемент разморожен (прототип).", "Успешно");
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