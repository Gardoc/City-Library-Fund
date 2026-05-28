using CityLibraryFund.Data.DTOs;
using CityLibraryFund.WPF.Services;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CityLibraryFund.WPF.Views
{
    /// <summary>
    /// Логика взаимодействия для LibrariesView.xaml
    /// </summary>
    public partial class LibrariesView : UserControl
    {
        private readonly LibraryService _libraryService = new();
        private LibraryDetailsDto? _selectedLibrary;

        public LibrariesView()
        {
            InitializeComponent();

            ClearLibraryCard();
        }

        private async void LibrariesView_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadFiltersAsync();
            await LoadLibrariesAsync();
        }

        private async Task LoadFiltersAsync()
        {
            StatusComboBox.Items.Add("Все");
            StatusComboBox.Items.Add("Активные");
            StatusComboBox.Items.Add("Неактивные");

            StatusComboBox.SelectedIndex = 0;

            SortComboBox.Items.Add("Название ↑");
            SortComboBox.Items.Add("Название ↓");
            SortComboBox.Items.Add("Количество экземпляров ↓");

            SortComboBox.SelectedIndex = 0;

            await Task.CompletedTask;
        }

        private async Task LoadLibrariesAsync()
        {
            string search = SearchTextBox.Text.Trim();

            bool? isActive = StatusComboBox.SelectedIndex switch
            {
                1 => true,
                2 => false,
                _ => null
            };

            string sortBy = SortComboBox.SelectedIndex switch
            {
                1 => "nameDesc",
                2 => "itemsCountDesc",
                _ => "nameAsc"
            };

            LibrariesDataGrid.ItemsSource =
                await _libraryService.GetLibrariesAsync(
                    search,
                    isActive,
                    sortBy);
        }

        private async void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            await LoadLibrariesAsync();
        }

        private async void Filter_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }

            await LoadLibrariesAsync();
        }

        private async void LibrariesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LibrariesDataGrid.SelectedItem is not LibraryListDto library)
            {
                ClearLibraryCard();

                return;
            }

            _selectedLibrary =
                await _libraryService.GetLibraryDetailsAsync(library.Id);

            if (_selectedLibrary == null)
            {
                return;
            }

            FillLibraryCard(_selectedLibrary);
        }

        private void FillLibraryCard(LibraryDetailsDto library)
        {
            NameTextBlock.Text = library.Name;
            AddressTextBlock.Text = library.Address;
            PhoneTextBlock.Text = library.Phone;

            StatusTextBlock.Text = library.IsActive
                ? "Активна"
                : "Неактивна";

            HallsCountTextBlock.Text =
                library.HallsCount.ToString();

            EmployeesCountTextBlock.Text =
                library.EmployeesCount.ToString();

            ReadersCountTextBlock.Text =
                library.ReadersCount.ToString();

            LibraryItemsCountTextBlock.Text =
                library.LibraryItemsCount.ToString();

            ActiveLoansCountTextBlock.Text =
                library.ActiveLoansCount.ToString();
        }

        private void ClearLibraryCard()
        {
            _selectedLibrary = null;

            NameTextBlock.Text = string.Empty;
            AddressTextBlock.Text = string.Empty;
            PhoneTextBlock.Text = string.Empty;
            StatusTextBlock.Text = string.Empty;
            HallsCountTextBlock.Text = string.Empty;
            EmployeesCountTextBlock.Text = string.Empty;
            ReadersCountTextBlock.Text = string.Empty;
            LibraryItemsCountTextBlock.Text = string.Empty;
            ActiveLoansCountTextBlock.Text = string.Empty;
        }
    }
}