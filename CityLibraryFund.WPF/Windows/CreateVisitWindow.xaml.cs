using CityLibraryFund.Data.DTOs;
using CityLibraryFund.WPF.Services;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace CityLibraryFund.WPF.Windows
{
    /// <summary>
    /// Логика взаимодействия для CreateVisitWindow.xaml
    /// </summary>
    public partial class CreateVisitWindow : Window
    {
        private readonly VisitService _visitService = new();

        public bool IsSaved { get; private set; }

        public CreateVisitWindow()
        {
            InitializeComponent();

            Loaded += CreateVisitWindow_Loaded;
        }

        private async void CreateVisitWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            ReaderComboBox.ItemsSource =
                await _visitService.GetReadersForVisitAsync();

            HallComboBox.ItemsSource =
                await _visitService.GetHallsForVisitAsync();

            VisitDatePicker.SelectedDate = DateTime.Now.Date;
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
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

            if (HallComboBox.SelectedValue == null)
            {
                MessageBox.Show(
                    "Выберите зал.",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (VisitDatePicker.SelectedDate == null)
            {
                MessageBox.Show(
                    "Выберите дату посещения.",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            CreateVisitDto dto = new()
            {
                ReaderId = (int)ReaderComboBox.SelectedValue,
                HallId = (int)HallComboBox.SelectedValue,

                VisitDate =
                    VisitDatePicker.SelectedDate.Value
                        .Add(DateTime.Now.TimeOfDay)
            };

            OperationResultDto result =
                await _visitService.CreateVisitAsync(dto);

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