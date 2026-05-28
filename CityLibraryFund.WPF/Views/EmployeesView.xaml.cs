using CityLibraryFund.Data.DTOs;
using CityLibraryFund.WPF.Services;
using CityLibraryFund.WPF.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CityLibraryFund.WPF.Views
{
    /// <summary>
    /// Логика взаимодействия для EmployeesView.xaml
    /// </summary>
    public partial class EmployeesView : UserControl
    {
        private readonly EmployeeService _employeeService = new();
        private EmployeeDetailsDto? _selectedEmployee;

        public EmployeesView()
        {
            InitializeComponent();

            ClearEmployeeCard();
        }

        private async void EmployeesView_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadFiltersAsync();
            await LoadEmployeesAsync();
        }

        private async Task LoadFiltersAsync()
        {
            RoleComboBox.Items.Add("Все роли");

            var roles = await _employeeService.GetEmployeeRolesAsync();

            foreach (string role in roles)
            {
                RoleComboBox.Items.Add(role);
            }

            RoleComboBox.SelectedIndex = 0;

            StatusComboBox.Items.Add("Все");
            StatusComboBox.Items.Add("Активные");
            StatusComboBox.Items.Add("Неактивные");

            StatusComboBox.SelectedIndex = 0;

            SortComboBox.Items.Add("ФИО ↑");
            SortComboBox.Items.Add("ФИО ↓");
            SortComboBox.Items.Add("Дата приема ↑");
            SortComboBox.Items.Add("Дата приема ↓");

            SortComboBox.SelectedIndex = 0;
        }

        private async Task LoadEmployeesAsync()
        {
            string search = SearchTextBox.Text.Trim();

            string? role = RoleComboBox.SelectedIndex <= 0
                ? null
                : RoleComboBox.SelectedItem?.ToString();

            bool? isActive = StatusComboBox.SelectedIndex switch
            {
                1 => true,
                2 => false,
                _ => null
            };

            string sortBy = SortComboBox.SelectedIndex switch
            {
                1 => "fullNameDesc",
                2 => "hireDateAsc",
                3 => "hireDateDesc",
                _ => "fullNameAsc"
            };

            EmployeesDataGrid.ItemsSource =
                await _employeeService.GetEmployeesAsync(
                    search,
                    role,
                    isActive,
                    sortBy);
        }

        private async void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            await LoadEmployeesAsync();
        }

        private async void Filter_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }

            await LoadEmployeesAsync();
        }

        private async void EmployeesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EmployeesDataGrid.SelectedItem is not EmployeeListDto employee)
            {
                ClearEmployeeCard();

                return;
            }

            _selectedEmployee =
                await _employeeService.GetEmployeeDetailsAsync(employee.Id);

            if (_selectedEmployee == null)
            {
                return;
            }

            FillEmployeeCard(_selectedEmployee);
        }

        private void FillEmployeeCard(EmployeeDetailsDto employee)
        {
            ChangePasswordButton.IsEnabled = employee.IsActive;
            EditButton.IsEnabled = employee.IsActive;

            ServiceHistoryButton.IsEnabled =
                employee.IsActive &&
                employee.Role == "Библиотекарь";

            DeactivateButton.IsEnabled = true;

            FullNameTextBlock.Text = employee.FullName;
            RoleTextBlock.Text = employee.Role;
            PositionTextBlock.Text = employee.Position;

            HireDateTextBlock.Text =
                employee.HireDate.ToString("dd.MM.yyyy");

            LoginTextBlock.Text = employee.Login;
            LibraryTextBlock.Text = employee.LibraryName;
            HallTextBlock.Text = employee.HallName;
            HallTypeTextBlock.Text = employee.HallType;

            FloorTextBlock.Text =
                employee.Floor?.ToString() ?? "-";

            DeactivateButton.Content =
                employee.IsActive
                ? "Деактивировать"
                : "Активировать";
        }

        private void ServiceHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedEmployee == null)
            {
                return;
            }

            EmployeeServiceHistoryWindow window = new(
                _selectedEmployee.Id,
                _selectedEmployee.FullName);

            window.Owner = Window.GetWindow(this);

            window.ShowDialog();
        }

        private async void ChangePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedEmployee == null)
            {
                return;
            }

            ChangePasswordWindow window = new(
                _selectedEmployee.Id,
                _selectedEmployee.FullName);

            window.Owner = Window.GetWindow(this);

            window.ShowDialog();

            if (window.IsSaved)
            {
                await ReloadAndSelectEmployeeAsync(_selectedEmployee.Id);
            }
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            EmployeeAddEditWindow window = new();

            window.Owner = Window.GetWindow(this);

            window.ShowDialog();

            if (window.IsSaved)
            {
                await LoadEmployeesAsync();
            }
        }

        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedEmployee == null)
            {
                return;
            }

            int employeeId = _selectedEmployee.Id;

            EmployeeEditDto? dto =
                await _employeeService.GetEmployeeForEditAsync(employeeId);

            if (dto == null)
            {
                return;
            }

            EmployeeAddEditWindow window = new(dto);

            window.Owner = Window.GetWindow(this);

            window.ShowDialog();

            if (window.IsSaved)
            {
                await ReloadAndSelectEmployeeAsync(employeeId);
            }
        }

        private async void DeactivateButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedEmployee == null)
            {
                return;
            }

            int employeeId = _selectedEmployee.Id;

            bool result =
                await _employeeService.ToggleEmployeeActivityAsync(employeeId);

            if (!result)
            {
                MessageBox.Show(
                    "Не удалось изменить статус.",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            await ReloadAndSelectEmployeeAsync(employeeId);
        }

        private void ClearEmployeeCard()
        {
            _selectedEmployee = null;

            FullNameTextBlock.Text = string.Empty;
            RoleTextBlock.Text = string.Empty;
            PositionTextBlock.Text = string.Empty;
            HireDateTextBlock.Text = string.Empty;
            LoginTextBlock.Text = string.Empty;
            LibraryTextBlock.Text = string.Empty;
            HallTextBlock.Text = string.Empty;
            HallTypeTextBlock.Text = string.Empty;
            FloorTextBlock.Text = string.Empty;

            ServiceHistoryButton.IsEnabled = false;
            ChangePasswordButton.IsEnabled = false;
            EditButton.IsEnabled = false;
            DeactivateButton.IsEnabled = false;

            DeactivateButton.Content = "Деактивировать";
        }

        private async Task ReloadAndSelectEmployeeAsync(int employeeId)
        {
            await LoadEmployeesAsync();

            if (EmployeesDataGrid.ItemsSource is not List<EmployeeListDto> employees)
            {
                return;
            }

            EmployeeListDto? selectedEmployee =
                employees.FirstOrDefault(e => e.Id == employeeId);

            if (selectedEmployee == null)
            {
                ClearEmployeeCard();

                return;
            }

            EmployeesDataGrid.SelectedItem = selectedEmployee;
            EmployeesDataGrid.ScrollIntoView(selectedEmployee);

            _selectedEmployee =
                await _employeeService.GetEmployeeDetailsAsync(employeeId);

            if (_selectedEmployee != null)
            {
                FillEmployeeCard(_selectedEmployee);
            }
        }
    }
}