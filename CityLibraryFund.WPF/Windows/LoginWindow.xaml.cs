using CityLibraryFund.WPF.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CityLibraryFund.WPF.Windows
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly AuthService _authService;

        public LoginWindow()
        {
            InitializeComponent();

            var context = DbContextFactory.Create();

            _authService = new AuthService(context);
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text;
            string password = PasswordBox.Password;

            var result = await _authService.LoginAsync(login, password);

            if (!result.IsSuccess)
            {
                MessageBox.Show(
                    result.ErrorMessage,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            CurrentUserService.EmployeeId = result.EmployeeId;
            CurrentUserService.FullName = result.FullName;
            CurrentUserService.Role = result.Role;
            CurrentUserService.HallId = result.HallId;
            CurrentUserService.HallName = result.HallName;
            CurrentUserService.LibraryId = result.LibraryId;
            CurrentUserService.LibraryName = result.LibraryName;

            MainWindow mainWindow = new MainWindow();

            mainWindow.Show();

            Close();
        }
    }
}
