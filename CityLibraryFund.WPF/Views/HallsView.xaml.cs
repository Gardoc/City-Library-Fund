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
    /// Логика взаимодействия для HallsView.xaml
    /// </summary>
    public partial class HallsView : UserControl
    {
        private readonly HallService _hallService = new();
        private HallDetailsDto? _selectedHall;

        public HallsView()
        {
            InitializeComponent();

            ClearHallCard();
        }

        private async void HallsView_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadFiltersAsync();
            await LoadHallsAsync();
        }

        private async Task LoadFiltersAsync()
        {
            LibraryComboBox.Items.Add("Все библиотеки");

            var libraries = await _hallService.GetLibrariesAsync();

            foreach (string library in libraries)
            {
                LibraryComboBox.Items.Add(library);
            }

            LibraryComboBox.SelectedIndex = 0;

            SortComboBox.Items.Add("Название ↑");
            SortComboBox.Items.Add("Название ↓");
            SortComboBox.Items.Add("Этаж ↑");
            SortComboBox.Items.Add("Этаж ↓");
            SortComboBox.Items.Add("Количество экземпляров ↓");

            SortComboBox.SelectedIndex = 0;
        }

        private async Task LoadHallsAsync()
        {
            string search = SearchTextBox.Text.Trim();

            string? library = LibraryComboBox.SelectedIndex <= 0
                ? null
                : LibraryComboBox.SelectedItem?.ToString();

            string sortBy = SortComboBox.SelectedIndex switch
            {
                1 => "nameDesc",
                2 => "floorAsc",
                3 => "floorDesc",
                4 => "itemsCountDesc",
                _ => "nameAsc"
            };

            HallsDataGrid.ItemsSource =
                await _hallService.GetHallsAsync(
                    search,
                    library,
                    sortBy);
        }

        private async void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            await LoadHallsAsync();
        }

        private async void Filter_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }

            await LoadHallsAsync();
        }

        private async void HallsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (HallsDataGrid.SelectedItem is not HallListDto hall)
            {
                ClearHallCard();

                return;
            }

            _selectedHall =
                await _hallService.GetHallDetailsAsync(hall.Id);

            if (_selectedHall == null)
            {
                return;
            }

            FillHallCard(_selectedHall);
        }

        private void FillHallCard(HallDetailsDto hall)
        {
            EmployeesButton.IsEnabled = true;

            NameTextBlock.Text = hall.Name;
            LibraryTextBlock.Text = hall.LibraryName;
            HallTypeTextBlock.Text = hall.HallType;

            FloorTextBlock.Text =
                hall.Floor?.ToString() ?? "-";

            EmployeesCountTextBlock.Text =
                hall.EmployeesCount.ToString();

            LibraryItemsCountTextBlock.Text =
                hall.LibraryItemsCount.ToString();

            ActiveLoansCountTextBlock.Text =
                hall.ActiveLoansCount.ToString();

            VisitsCountTextBlock.Text =
                hall.VisitsCount.ToString();
        }

        private void EmployeesButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedHall == null)
            {
                return;
            }

            HallEmployeesWindow window = new(
                _selectedHall.Id,
                _selectedHall.Name);

            window.Owner = Window.GetWindow(this);

            window.ShowDialog();
        }

        private void ClearHallCard()
        {
            _selectedHall = null;

            NameTextBlock.Text = string.Empty;
            LibraryTextBlock.Text = string.Empty;
            HallTypeTextBlock.Text = string.Empty;
            FloorTextBlock.Text = string.Empty;
            EmployeesCountTextBlock.Text = string.Empty;
            LibraryItemsCountTextBlock.Text = string.Empty;
            ActiveLoansCountTextBlock.Text = string.Empty;
            VisitsCountTextBlock.Text = string.Empty;

            EmployeesButton.IsEnabled = false;
        }

        private async Task ReloadAndSelectHallAsync(int hallId)
        {
            await LoadHallsAsync();

            if (HallsDataGrid.ItemsSource is not List<HallListDto> halls)
            {
                return;
            }

            HallListDto? selectedHall =
                halls.FirstOrDefault(h => h.Id == hallId);

            if (selectedHall == null)
            {
                ClearHallCard();

                return;
            }

            HallsDataGrid.SelectedItem = selectedHall;
            HallsDataGrid.ScrollIntoView(selectedHall);

            _selectedHall =
                await _hallService.GetHallDetailsAsync(hallId);

            if (_selectedHall != null)
            {
                FillHallCard(_selectedHall);
            }
        }
    }
}