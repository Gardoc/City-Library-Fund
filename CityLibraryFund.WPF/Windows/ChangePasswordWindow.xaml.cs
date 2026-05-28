using CityLibraryFund.Data.DTOs;
using CityLibraryFund.WPF.Services;
using System.Windows;

namespace CityLibraryFund.WPF.Windows
{
    /// <summary>
    /// Логика взаимодействия для ChangePasswordWindow.xaml
    /// </summary>
    public partial class ChangePasswordWindow : Window
    {
        private readonly EmployeeService _employeeService = new();

        private readonly int _employeeId;

        public bool IsSaved { get; private set; }

        public ChangePasswordWindow(int employeeId, string employeeFullName)
        {
            InitializeComponent();

            _employeeId = employeeId;

            EmployeeTextBlock.Text =
                $"Сотрудник: {employeeFullName}";
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePasswordDto dto = new()
            {
                EmployeeId = _employeeId,
                OldPassword = OldPasswordBox.Password,
                NewPassword = NewPasswordBox.Password
            };

            OperationResultDto result =
                await _employeeService.ChangePasswordAsync(dto);

            if (!result.IsSuccess)
            {
                MessageBox.Show(
                    result.ErrorMessage,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            MessageBox.Show(
                "Пароль успешно изменен.",
                "Успех",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            IsSaved = true;

            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}