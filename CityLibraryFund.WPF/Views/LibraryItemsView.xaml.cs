using CityLibraryFund.Data.DTOs;
using CityLibraryFund.Data.Entities;
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
    /// Логика взаимодействия для LibraryItemsView.xaml
    /// </summary>
    public partial class LibraryItemsView : UserControl
    {
        private readonly LibraryItemService _libraryItemService = new();
        private LibraryItemDetailsDto? _selectedLibraryItem;

        public LibraryItemsView()
        {
            InitializeComponent();

            ClearLibraryItemCard();
        }

        private async void LibraryItemsView_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentUserService.Role == "Библиотекарь")
            {
                AddButton.Visibility = Visibility.Collapsed;
                EditButton.Visibility = Visibility.Collapsed;
                WriteOffButton.Visibility = Visibility.Collapsed;
            }

            await LoadFiltersAsync();
            await LoadLibraryItemsAsync();
        }

        private async Task LoadFiltersAsync()
        {
            StatusComboBox.Items.Add(new LibraryItemStatus
            {
                Id = 0,
                Name = "Все статусы"
            });

            var statuses = await _libraryItemService.GetStatusesAsync();

            foreach (LibraryItemStatus status in statuses)
            {
                StatusComboBox.Items.Add(status);
            }

            StatusComboBox.SelectedIndex = 0;

            SortComboBox.Items.Add("Инвентарный номер ↑");
            SortComboBox.Items.Add("Инвентарный номер ↓");
            SortComboBox.Items.Add("Дата поступления ↑");
            SortComboBox.Items.Add("Дата поступления ↓");

            SortComboBox.SelectedIndex = 0;
        }

        private async Task LoadLibraryItemsAsync()
        {
            string search = SearchTextBox.Text.Trim();

            int? statusId = StatusComboBox.SelectedIndex <= 0
                ? null
                : ((LibraryItemStatus)StatusComboBox.SelectedItem).Id;

            string sortBy = SortComboBox.SelectedIndex switch
            {
                1 => "inventoryDesc",
                2 => "arrivalDateAsc",
                3 => "arrivalDateDesc",
                _ => "inventoryAsc"
            };

            LibraryItemsDataGrid.ItemsSource =
                await _libraryItemService.GetLibraryItemsAsync(
                    search,
                    statusId,
                    null,
                    sortBy);
        }

        private async void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            await LoadLibraryItemsAsync();
        }

        private async void Filter_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }

            await LoadLibraryItemsAsync();
        }

        private async void LibraryItemsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LibraryItemsDataGrid.SelectedItem is not LibraryItemListDto item)
            {
                ClearLibraryItemCard();

                return;
            }

            _selectedLibraryItem =
                await _libraryItemService.GetLibraryItemDetailsAsync(item.Id);

            if (_selectedLibraryItem == null)
            {
                return;
            }

            FillLibraryItemCard(_selectedLibraryItem);
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            LibraryItemAddEditWindow window = new();

            window.Owner = Window.GetWindow(this);

            window.ShowDialog();

            if (window.IsSaved)
            {
                await LoadLibraryItemsAsync();
            }
        }

        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedLibraryItem == null)
            {
                return;
            }

            int libraryItemId = _selectedLibraryItem.Id;

            LibraryItemEditDto? dto =
                await _libraryItemService.GetLibraryItemForEditAsync(
                    libraryItemId);

            if (dto == null)
            {
                return;
            }

            LibraryItemAddEditWindow window = new(dto);

            window.Owner = Window.GetWindow(this);

            window.ShowDialog();

            if (window.IsSaved)
            {
                await ReloadAndSelectLibraryItemAsync(libraryItemId);
            }
        }

        private async void WriteOffButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedLibraryItem == null)
            {
                return;
            }

            MessageBoxResult confirm = MessageBox.Show(
                "Списать экземпляр?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirm != MessageBoxResult.Yes)
            {
                return;
            }

            int libraryItemId = _selectedLibraryItem.Id;

            OperationResultDto result =
                await _libraryItemService.WriteOffLibraryItemAsync(
                    libraryItemId);

            if (!result.IsSuccess)
            {
                MessageBox.Show(
                    result.ErrorMessage,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            await ReloadAndSelectLibraryItemAsync(libraryItemId);
        }

        private void FillLibraryItemCard(LibraryItemDetailsDto item)
        {
            bool canEdit = !item.IsWrittenOff && !item.IsIssued;
            bool canWriteOff = !item.IsWrittenOff && !item.IsIssued;

            EditButton.IsEnabled = canEdit;
            WriteOffButton.IsEnabled = canWriteOff;

            InventoryNumberTextBlock.Text = item.InventoryNumber;
            WorkTitleTextBlock.Text = item.WorkTitle;
            AuthorsTextBlock.Text = item.Authors;

            PublisherTextBlock.Text =
                item.Publisher + ", " + item.PublishYear;

            StatusTextBlock.Text = item.Status;
            LibraryTextBlock.Text = item.LibraryName;
            HallTextBlock.Text = item.HallName;

            ShelfTextBlock.Text =
                "Стеллаж " + item.RackNumber +
                ", полка " + item.ShelfNumber;

            ArrivalDateTextBlock.Text =
                item.ArrivalDate.ToString("dd.MM.yyyy");

            OnlyReadingRoomTextBlock.Text =
                item.OnlyReadingRoom ? "Да" : "Нет";
        }

        private void ClearLibraryItemCard()
        {
            _selectedLibraryItem = null;

            InventoryNumberTextBlock.Text = string.Empty;
            WorkTitleTextBlock.Text = string.Empty;
            AuthorsTextBlock.Text = string.Empty;
            PublisherTextBlock.Text = string.Empty;
            StatusTextBlock.Text = string.Empty;
            LibraryTextBlock.Text = string.Empty;
            HallTextBlock.Text = string.Empty;
            ShelfTextBlock.Text = string.Empty;
            ArrivalDateTextBlock.Text = string.Empty;
            OnlyReadingRoomTextBlock.Text = string.Empty;

            EditButton.IsEnabled = false;
            WriteOffButton.IsEnabled = false;
        }

        private async Task ReloadAndSelectLibraryItemAsync(int libraryItemId)
        {
            await LoadLibraryItemsAsync();

            if (LibraryItemsDataGrid.ItemsSource is not List<LibraryItemListDto> items)
            {
                return;
            }

            LibraryItemListDto? selectedItem =
                items.FirstOrDefault(li => li.Id == libraryItemId);

            if (selectedItem == null)
            {
                ClearLibraryItemCard();

                return;
            }

            LibraryItemsDataGrid.SelectedItem = selectedItem;
            LibraryItemsDataGrid.ScrollIntoView(selectedItem);

            _selectedLibraryItem =
                await _libraryItemService.GetLibraryItemDetailsAsync(
                    libraryItemId);

            if (_selectedLibraryItem != null)
            {
                FillLibraryItemCard(_selectedLibraryItem);
            }
        }
    }
}