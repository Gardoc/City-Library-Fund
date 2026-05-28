using CityLibraryFund.Data.DTOs;
using CityLibraryFund.WPF.Services;
using System;
using System.Windows;
using System.Windows.Controls;

namespace CityLibraryFund.WPF.Windows
{
    /// <summary>
    /// Логика взаимодействия для EmployeeAddEditWindow.xaml
    /// </summary>
    public partial class EmployeeAddEditWindow : Window
    {
        private readonly EmployeeService _employeeService = new();

        private readonly EmployeeEditDto? _employee;

        public bool IsSaved { get; private set; }

        public EmployeeAddEditWindow(EmployeeEditDto? employee = null)
        {
            InitializeComponent();

            _employee = employee;

            Loaded += EmployeeAddEditWindow_Loaded;
        }

        private async void EmployeeAddEditWindow_Loaded(object sender, RoutedEventArgs e)
        {
            RoleComboBox.ItemsSource =
                await _employeeService.GetEmployeeRolesForEditAsync();

            LibraryComboBox.ItemsSource =
                await _employeeService.GetLibrariesAsync();

            if (_employee != null)
            {
                FillForm(_employee);
            }
        }

        private async void FillForm(EmployeeEditDto employee)
        {
            var hall = await _employeeService.GetHallAsync(employee.HallId);

            if (hall != null)
            {
                LibraryComboBox.SelectedValue = hall.LibraryId;

                HallComboBox.ItemsSource =
                    await _employeeService.GetHallsByLibraryAsync(
                        hall.LibraryId);

                HallComboBox.SelectedValue = employee.HallId;
            }

            FirstNameTextBox.Text = employee.FirstName;
            LastNameTextBox.Text = employee.LastName;
            PatronymicTextBox.Text = employee.Patronymic;
            PositionTextBox.Text = employee.Position;

            HireDatePicker.SelectedDate =
                employee.HireDate.ToDateTime(TimeOnly.MinValue);

            RoleComboBox.SelectedValue = employee.EmployeeRoleId;
            HallComboBox.SelectedValue = employee.HallId;

            LoginTextBox.Text = employee.Login;

            LoginTextBox.IsEnabled = false;

            PasswordPanel.Visibility = Visibility.Collapsed;
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (HireDatePicker.SelectedDate == null)
            {
                MessageBox.Show(
                    "Выберите дату приема.",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (RoleComboBox.SelectedValue == null)
            {
                MessageBox.Show(
                    "Выберите роль.",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (LibraryComboBox.SelectedValue == null)
            {
                MessageBox.Show(
                    "Выберите библиотеку.",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (HallComboBox.SelectedValue == null)
            {
                MessageBox.Show(
                    "Выберите зал.",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            EmployeeEditDto dto = new()
            {
                Id = _employee?.Id ?? 0,

                FirstName = FirstNameTextBox.Text,
                LastName = LastNameTextBox.Text,
                Patronymic = PatronymicTextBox.Text,
                Position = PositionTextBox.Text,

                HireDate =
                    DateOnly.FromDateTime(
                        HireDatePicker.SelectedDate.Value),

                EmployeeRoleId = (int)RoleComboBox.SelectedValue,
                HallId = (int)HallComboBox.SelectedValue,

                Login = LoginTextBox.Text,

                Password = _employee == null
                    ? PasswordBox.Password
                    : string.Empty
            };

            OperationResultDto result;

            if (_employee == null)
            {
                result =
                    await _employeeService.CreateEmployeeAsync(dto);
            }
            else
            {
                result =
                    await _employeeService.UpdateEmployeeAsync(dto);
            }

            if (!result.IsSuccess)
            {
                MessageBox.Show(
                    result.ErrorMessage,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            IsSaved = true;

            Close();
        }

        private async void LibraryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LibraryComboBox.SelectedValue is not int libraryId)
            {
                return;
            }

            HallComboBox.ItemsSource =
                await _employeeService.GetHallsByLibraryAsync(libraryId);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}