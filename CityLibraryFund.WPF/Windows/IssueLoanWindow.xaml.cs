using CityLibraryFund.Data.DTOs;
using CityLibraryFund.WPF.Services;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace CityLibraryFund.WPF.Windows
{
    /// <summary>
    /// Логика взаимодействия для IssueLoanWindow.xaml
    /// </summary>
    public partial class IssueLoanWindow : Window
    {
        private readonly LoanService _loanService = new();

        public bool IsSaved { get; private set; }

        public IssueLoanWindow()
        {
            InitializeComponent();

            Loaded += IssueLoanWindow_Loaded;
        }

        private async void IssueLoanWindow_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            ReaderComboBox.ItemsSource =
                await _loanService.GetReadersForIssueAsync();

            LibraryItemComboBox.ItemsSource =
                await _loanService.GetAvailableLibraryItemsAsync();

            IssueDatePicker.SelectedDate = DateTime.Now;
            DueDatePicker.SelectedDate = DateTime.Now.AddDays(14);
        }

        private async void IssueButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (ReaderComboBox.SelectedValue == null)
            {
                MessageBox.Show(
                    "Выберите читателя.",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (LibraryItemComboBox.SelectedValue == null)
            {
                MessageBox.Show(
                    "Выберите экземпляр.",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (IssueDatePicker.SelectedDate == null ||
                DueDatePicker.SelectedDate == null)
            {
                MessageBox.Show(
                    "Выберите даты.",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            IssueLoanDto dto = new()
            {
                ReaderId = (int)ReaderComboBox.SelectedValue,
                LibraryItemId = (int)LibraryItemComboBox.SelectedValue,

                IssueDate = DateOnly.FromDateTime(
                    IssueDatePicker.SelectedDate.Value),

                DueDate = DateOnly.FromDateTime(
                    DueDatePicker.SelectedDate.Value)
            };

            var result = await _loanService.IssueLoanAsync(dto);

            if (!result.IsSuccess)
            {
                MessageBox.Show(
                    result.ErrorMessage,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            IsSaved = true;

            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}