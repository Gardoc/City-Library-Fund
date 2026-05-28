using CityLibraryFund.Data.DTOs;
using CityLibraryFund.WPF.Helpers;
using CityLibraryFund.WPF.Services;
using System;
using System.Windows;
using System.Windows.Input;

namespace CityLibraryFund.WPF.Windows
{
    /// <summary>
    /// Логика взаимодействия для LibraryItemAddEditWindow.xaml
    /// </summary>
    public partial class LibraryItemAddEditWindow : Window
    {
        private readonly LibraryItemService _libraryItemService = new();

        private readonly LibraryItemEditDto? _libraryItem;

        public bool IsSaved { get; private set; }

        public LibraryItemAddEditWindow(LibraryItemEditDto? libraryItem = null)
        {
            InitializeComponent();

            _libraryItem = libraryItem;

            Loaded += LibraryItemAddEditWindow_Loaded;
        }

        private async void LibraryItemAddEditWindow_Loaded(object sender, RoutedEventArgs e)
        {
            EditionComboBox.ItemsSource =
                await _libraryItemService.GetEditionsAsync();

            LibraryComboBox.ItemsSource =
                await _libraryItemService.GetLibrariesAsync();

            ArrivalDatePicker.SelectedDate = DateTime.Now;

            if (_libraryItem != null)
            {
                FillForm(_libraryItem);
            }

            UpdateLoanDaysState();
        }

        private async void FillForm(LibraryItemEditDto item)
        {
            EditionComboBox.SelectedValue = item.EditionId;

            var hall =
                await _libraryItemService.GetHallAsync(item.HallId);

            if (hall != null)
            {
                LibraryComboBox.SelectedValue = hall.LibraryId;

                HallComboBox.ItemsSource =
                    await _libraryItemService
                        .GetHallsByLibraryAsync(hall.LibraryId);

                HallComboBox.SelectedValue = item.HallId;
            }

            InventoryNumberTextBox.Text = item.InventoryNumber;
            RackNumberTextBox.Text = item.RackNumber.ToString();
            ShelfNumberTextBox.Text = item.ShelfNumber.ToString();

            OnlyReadingRoomCheckBox.IsChecked =
                item.OnlyReadingRoom;

            LoanDaysTextBox.Text =
                item.LoanDays?.ToString();

            ArrivalDatePicker.SelectedDate =
                item.ArrivalDate.ToDateTime(TimeOnly.MinValue);

            PriceTextBox.Text = item.Price?.ToString();
        }

        private void OnlyReadingRoomCheckBox_Changed(
            object sender,
            RoutedEventArgs e)
        {
            UpdateLoanDaysState();
        }

        private void UpdateLoanDaysState()
        {
            bool onlyReadingRoom =
                OnlyReadingRoomCheckBox.IsChecked == true;

            LoanDaysTextBox.IsEnabled = !onlyReadingRoom;
            LoanDaysTextBlock.IsEnabled = !onlyReadingRoom;

            if (onlyReadingRoom)
            {
                LoanDaysTextBox.Text = string.Empty;
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(
                RackNumberTextBox.Text,
                out int rackNumber))
            {
                MessageBox.Show(
                    "Введите корректный номер стеллажа.",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (!int.TryParse(
                ShelfNumberTextBox.Text,
                out int shelfNumber))
            {
                MessageBox.Show(
                    "Введите корректный номер полки.",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            int? loanDays = null;

            if (OnlyReadingRoomCheckBox.IsChecked != true)
            {
                bool parsed =
                    int.TryParse(
                        LoanDaysTextBox.Text,
                        out int parsedLoanDays);

                if (!parsed)
                {
                    MessageBox.Show(
                        "Введите корректный срок выдачи.",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }

                loanDays = parsedLoanDays;
            }

            decimal? price = null;

            if (!string.IsNullOrWhiteSpace(PriceTextBox.Text))
            {
                bool parsed =
                    decimal.TryParse(
                        PriceTextBox.Text,
                        out decimal parsedPrice);

                if (!parsed)
                {
                    MessageBox.Show(
                        "Введите корректную цену.",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }

                price = parsedPrice;
            }

            if (ArrivalDatePicker.SelectedDate == null)
            {
                MessageBox.Show(
                    "Выберите дату поступления.",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (EditionComboBox.SelectedValue == null)
            {
                MessageBox.Show(
                    "Выберите издание из списка.",
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

            LibraryItemEditDto dto = new()
            {
                Id = _libraryItem?.Id ?? 0,
                EditionId = (int)EditionComboBox.SelectedValue,
                HallId = (int)HallComboBox.SelectedValue,

                InventoryNumber =
                    InventoryNumberTextBox.Text,

                RackNumber = rackNumber,
                ShelfNumber = shelfNumber,

                OnlyReadingRoom =
                    OnlyReadingRoomCheckBox.IsChecked == true,

                LoanDays = loanDays,

                ArrivalDate =
                    DateOnly.FromDateTime(
                        ArrivalDatePicker.SelectedDate!.Value),

                Price = price
            };

            OperationResultDto result;

            if (_libraryItem == null)
            {
                result =
                    await _libraryItemService
                        .CreateLibraryItemAsync(dto);
            }
            else
            {
                result =
                    await _libraryItemService
                        .UpdateLibraryItemAsync(dto);
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

        private async void LibraryComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (LibraryComboBox.SelectedValue is not int libraryId)
            {
                return;
            }

            HallComboBox.ItemsSource =
                await _libraryItemService
                    .GetHallsByLibraryAsync(libraryId);
        }

        private void NumberOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ValidationHelper.AllowOnlyNumbers(sender, e);
        }

        private void NumberOnly_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            ValidationHelper.PreventPasteNonNumeric(sender, e);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}