using System.Linq;
using System.Windows;
using UP1.Services;
using UP1.Windows;

namespace UP1.Windows
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Введите логин и пароль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var user = App.DataService.GetUser(login, password);

            if (user != null)
            {
                if (user.IsFrozen)
                {
                    MessageBox.Show($"Ваш аккаунт заморожен!\nПричина: {user.FreezeReason}",
                                  "Доступ запрещён", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MainWindow main = new MainWindow(user);
                main.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Регистрация будет добавлена позже.\n\nТестовые аккаунты:\nadmin / admin123\nauthor / author123\nuser / user123");
        }
    }
}