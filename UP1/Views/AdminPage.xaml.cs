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
            LoadActiveUsers();
            LoadFrozenItems();
            LoadAuthorRequests();
            LoadComplaints();
        }
        private void LoadComplaints()
        {
            if (lbComplaints == null) return;
            lbComplaints.Items.Clear();

            var complaints = App.DataService.GetAllComplaints();

            foreach (var complaint in complaints)
            {
                string target = complaint.Book != null
                    ? $"Книга: «{complaint.Book.Title}»"
                    : "Другое";

                var item = new ListBoxItem
                {
                    Content = $"От {complaint.User?.DisplayName ?? "Пользователь"}: {target}\n{complaint.Reason}",
                    Tag = complaint.Id
                };
                lbComplaints.Items.Add(item);
            }

            if (complaints.Count == 0)
            {
                lbComplaints.Items.Add("Жалоб пока нет.");
            }
        }

        private void BtnResolveComplaint_Click(object sender, RoutedEventArgs e)
        {
            if (lbComplaints.SelectedItem == null || lbComplaints.SelectedItem is string)
            {
                MessageBox.Show("Выберите жалобу!", "Предупреждение");
                return;
            }

            var selected = lbComplaints.SelectedItem as ListBoxItem;
            int complaintId = (int)selected.Tag;

            bool success = App.DataService.ResolveComplaint(complaintId);

            if (success)
            {
                MessageBox.Show("Жалоба помечена как рассмотренная.", "Успешно");
                LoadComplaints();
            }
        }

        private void BtnRejectComplaint_Click(object sender, RoutedEventArgs e)
        {
            if (lbComplaints.SelectedItem == null || lbComplaints.SelectedItem is string)
            {
                MessageBox.Show("Выберите жалобу!", "Предупреждение");
                return;
            }

            var selected = lbComplaints.SelectedItem as ListBoxItem;
            int complaintId = (int)selected.Tag;

            bool success = App.DataService.ResolveComplaint(complaintId); // можно сделать отдельный метод Reject

            if (success)
            {
                MessageBox.Show("Жалоба отклонена.", "Отклонено");
                LoadComplaints();
            }
        }
        // ПОЛЬЗОВАТЕЛИ
        private void LoadActiveUsers()
        {
            if (lbUsers == null) return;
            lbUsers.Items.Clear();

            var users = App.DataService.GetAllUsers()
                .Where(u => !u.IsFrozen)        // ← фильтруем замороженных
                .ToList();

            foreach (var user in users)
            {
                string role = user.Role?.Name ?? "User";

                var item = new ListBoxItem
                {
                    Content = $"{user.DisplayName} ({user.Login}) — {role}",
                    Tag = user.Id
                };
                lbUsers.Items.Add(item);
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
                LoadAllData();
            }
            else
            {
                MessageBox.Show("Не удалось изменить роль пользователя.", "Ошибка");
            }
        }

        // ================== ЗАМОРОЗКА ПОЛЬЗОВАТЕЛЯ ==================
        private void BtnFreezeUser_Click(object sender, RoutedEventArgs e)
        {
            if (lbUsers.SelectedItem == null)
            {
                MessageBox.Show("Выберите пользователя из списка!", "Предупреждение");
                return;
            }

            var selectedItem = lbUsers.SelectedItem as ListBoxItem;
            int userId = (int)selectedItem.Tag;

            bool success = App.DataService.FreezeUser(userId, "Заморожен администратором");

            if (success)
            {
                MessageBox.Show("Пользователь успешно заморожен!", "Успешно");
                LoadAllData();
            }
            else
            {
                MessageBox.Show("Не удалось заморозить пользователя.", "Ошибка");
            }
        }

        private void LoadFrozenItems()
        {
            if (lbFrozenItems == null) return;
            lbFrozenItems.Items.Clear();

            // Замороженные пользователи
            var frozenUsers = App.DataService.GetFrozenUsers();
            foreach (var user in frozenUsers)
            {
                var item = new ListBoxItem
                {
                    Content = $"👤 ПОЛЬЗОВАТЕЛЬ: {user.DisplayName} ({user.Login}) — {user.FreezeReason}",
                    Tag = new FrozenItem { Type = "User", Id = user.Id }
                };
                lbFrozenItems.Items.Add(item);
            }

            // Замороженные книги
            var frozenBooks = App.DataService.GetFrozenBooks();
            foreach (var book in frozenBooks)
            {
                var item = new ListBoxItem
                {
                    Content = $"📖 КНИГА: «{book.Title}» — {book.Author?.DisplayName ?? "Неизвестен"}",
                    Tag = new FrozenItem { Type = "Book", Id = book.Id }
                };
                lbFrozenItems.Items.Add(item);
            }

            if (lbFrozenItems.Items.Count == 0)
            {
                lbFrozenItems.Items.Add("Нет замороженных элементов.");
            }
        }

        private void BtnUnfreeze_Click(object sender, RoutedEventArgs e)
        {
            if (lbFrozenItems.SelectedItem == null || lbFrozenItems.SelectedItem is string)
            {
                MessageBox.Show("Выберите элемент для разморозки!", "Предупреждение");
                return;
            }

            var selected = lbFrozenItems.SelectedItem as ListBoxItem;
            var frozenItem = selected.Tag as FrozenItem;

            if (frozenItem == null) return;

            bool success = false;
            string message = "";

            if (frozenItem.Type == "User")
            {
                success = App.DataService.UnfreezeUser(frozenItem.Id);
                message = "Пользователь успешно разморожен!";
            }
            else if (frozenItem.Type == "Book")
            {
                success = App.DataService.UnfreezeBook(frozenItem.Id);
                message = "Книга успешно разморожена!";
            }

            if (success)
            {
                MessageBox.Show(message, "Успешно");
                LoadAllData(); // обновляем оба списка
            }
            else
            {
                MessageBox.Show("Не удалось разморозить элемент.", "Ошибка");
            }
        }
        public class FrozenItem
        {
            public string Type { get; set; }  // "User" или "Book"
            public int Id { get; set; }
        }
    }
}