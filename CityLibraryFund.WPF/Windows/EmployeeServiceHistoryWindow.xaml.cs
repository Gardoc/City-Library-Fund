using CityLibraryFund.Data.DTOs;
using CityLibraryFund.WPF.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace CityLibraryFund.WPF.Windows
{
    /// <summary>
    /// Логика взаимодействия для EmployeeServiceHistoryWindow.xaml
    /// </summary>
    public partial class EmployeeServiceHistoryWindow : Window
    {
        private readonly EmployeeService _employeeService = new();

        private readonly int _employeeId;

        private readonly string _employeeName;

        public EmployeeServiceHistoryWindow(
            int employeeId,
            string employeeName)
        {
            InitializeComponent();

            _employeeId = employeeId;
            _employeeName = employeeName;

            Loaded += EmployeeServiceHistoryWindow_Loaded;
        }

        private async void EmployeeServiceHistoryWindow_Loaded(object sender, RoutedEventArgs e)
        {
            EmployeeNameTextBlock.Text = _employeeName;

            DateFromPicker.SelectedDate = DateTime.Now.AddMonths(-1);
            DateToPicker.SelectedDate = DateTime.Now;

            await LoadDataAsync();
        }

        private async void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            if (DateFromPicker.SelectedDate == null ||
                DateToPicker.SelectedDate == null)
            {
                MessageBox.Show(
                    "Выберите период.",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            DateOnly dateFrom =
                DateOnly.FromDateTime(
                    DateFromPicker.SelectedDate.Value);

            DateOnly dateTo =
                DateOnly.FromDateTime(
                    DateToPicker.SelectedDate.Value);

            if (dateFrom > dateTo)
            {
                MessageBox.Show(
                    "Дата начала не может быть больше даты окончания.",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            EmployeeServiceStatisticsDto statistics =
                await _employeeService.GetEmployeeServiceStatisticsAsync(
                    _employeeId,
                    dateFrom,
                    dateTo);

            ReadersCountTextBlock.Text = statistics.ReadersCount.ToString();

            LoansCountTextBlock.Text = statistics.LoansCount.ToString();

            ReturnedCountTextBlock.Text =  statistics.ReturnedCount.ToString();

            ActiveLoansCountTextBlock.Text = statistics.ActiveLoansCount.ToString();

            List<EmployeeServiceHistoryDto> history =
                await _employeeService.GetEmployeeServiceHistoryAsync(
                    _employeeId,
                    dateFrom,
                    dateTo);

            HistoryDataGrid.ItemsSource = history;
        }
    }
}