using CityLibraryFund.Data.DTOs;
using CityLibraryFund.WPF.Services;
using CityLibraryFund.WPF.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CityLibraryFund.WPF.Views
{
    /// <summary>
    /// Логика взаимодействия для LoansView.xaml
    /// </summary>
    public partial class LoansView : UserControl
    {
        private readonly LoanService _loanService = new();
        private LoanListDto? _selectedLoan;

        public LoansView()
        {
            InitializeComponent();

            ClearLoanCard();
        }

        private async void LoansView_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadFiltersAsync();
            await LoadLoansAsync();
        }

        private async Task LoadFiltersAsync()
        {
            StatusComboBox.Items.Add("Все");
            StatusComboBox.Items.Add("Активные");
            StatusComboBox.Items.Add("Возвращенные");
            StatusComboBox.Items.Add("Просроченные");

            StatusComboBox.SelectedIndex = 0;

            SortComboBox.Items.Add("Дата выдачи ↓");
            SortComboBox.Items.Add("Дата выдачи ↑");

            SortComboBox.SelectedIndex = 0;

            await Task.CompletedTask;
        }

        private async Task LoadLoansAsync()
        {
            string search = SearchTextBox.Text.Trim();

            string? status = StatusComboBox.SelectedIndex switch
            {
                1 => "active",
                2 => "returned",
                3 => "overdue",
                _ => null
            };

            string sortBy = SortComboBox.SelectedIndex switch
            {
                1 => "issueDateAsc",
                _ => "issueDateDesc"
            };

            LoansDataGrid.ItemsSource =
                await _loanService.GetLoansAsync(
                    search,
                    status,
                    sortBy);
        }

        private async void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            await LoadLoansAsync();
        }

        private async void Filter_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }

            await LoadLoansAsync();
        }

        private void LoansDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LoansDataGrid.SelectedItem is not LoanListDto loan)
            {
                ClearLoanCard();

                return;
            }

            _selectedLoan = loan;

            FillLoanCard(loan);
        }

        private void FillLoanCard(LoanListDto loan)
        {
            BookTitleTextBlock.Text = loan.BookTitle;
            ReaderTextBlock.Text = loan.ReaderFullName;
            InventoryTextBlock.Text = loan.InventoryNumber;

            IssueDateTextBlock.Text =
                loan.IssueDate.ToString("dd.MM.yyyy");

            DueDateTextBlock.Text =
                loan.DueDate.ToString("dd.MM.yyyy");

            ReturnDateTextBlock.Text =
                loan.ReturnDate?.ToString("dd.MM.yyyy");

            ReturnDatePanel.Visibility = loan.ReturnDate == null
                ? Visibility.Collapsed
                : Visibility.Visible;

            StatusTextBlock.Text = loan.Status;

            ReturnButton.IsEnabled = loan.ReturnDate == null;
        }

        private void ClearLoanCard()
        {
            _selectedLoan = null;

            BookTitleTextBlock.Text = string.Empty;
            ReaderTextBlock.Text = string.Empty;
            InventoryTextBlock.Text = string.Empty;
            IssueDateTextBlock.Text = string.Empty;
            DueDateTextBlock.Text = string.Empty;
            ReturnDateTextBlock.Text = string.Empty;
            StatusTextBlock.Text = string.Empty;

            ReturnDatePanel.Visibility = Visibility.Collapsed;

            ReturnButton.IsEnabled = false;
        }

        private async void IssueLoanButton_Click(object sender, RoutedEventArgs e)
        {
            IssueLoanWindow window = new();

            window.Owner = Window.GetWindow(this);

            window.ShowDialog();

            if (window.IsSaved)
            {
                await LoadLoansAsync();
            }
        }

        private async void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedLoan == null)
            {
                return;
            }

            MessageBoxResult result = MessageBox.Show(
                "Оформить возврат литературы?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            bool success =
                await _loanService.ReturnLoanAsync(_selectedLoan.Id);

            if (!success)
            {
                MessageBox.Show(
                    "Не удалось оформить возврат.",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            await ReloadAndSelectLoanAsync(_selectedLoan.Id);
        }

        private async Task ReloadAndSelectLoanAsync(int loanId)
        {
            await LoadLoansAsync();

            if (LoansDataGrid.ItemsSource is not List<LoanListDto> loans)
            {
                return;
            }

            LoanListDto? selectedLoan =
                loans.FirstOrDefault(l => l.Id == loanId);

            if (selectedLoan == null)
            {
                ClearLoanCard();

                return;
            }

            LoansDataGrid.SelectedItem = selectedLoan;
            LoansDataGrid.ScrollIntoView(selectedLoan);

            _selectedLoan = selectedLoan;

            FillLoanCard(selectedLoan);
        }
    }
}