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
    /// Логика взаимодействия для VisitsView.xaml
    /// </summary>
    public partial class VisitsView : UserControl
    {
        private readonly VisitService _visitService = new();

        private VisitListDto? _selectedVisit;

        public VisitsView()
        {
            InitializeComponent();

            ClearVisitCard();
        }

        private async void VisitsView_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadFiltersAsync();
            await LoadVisitsAsync();
        }

        private async Task LoadFiltersAsync()
        {
            HallComboBox.Items.Add(new HallVisitDto
            {
                Id = 0,
                DisplayName = "Все залы"
            });

            List<HallVisitDto> halls =
                await _visitService.GetHallsForVisitAsync();

            foreach (HallVisitDto hall in halls)
            {
                HallComboBox.Items.Add(hall);
            }

            HallComboBox.SelectedIndex = 0;

            SortComboBox.Items.Add("Дата посещения ↓");
            SortComboBox.Items.Add("Дата посещения ↑");

            SortComboBox.SelectedIndex = 0;
        }

        private async Task LoadVisitsAsync()
        {
            string search = SearchTextBox.Text.Trim();

            int? hallId = HallComboBox.SelectedIndex <= 0
                ? null
                : (HallComboBox.SelectedItem as HallVisitDto)?.Id;

            string sortBy = SortComboBox.SelectedIndex switch
            {
                1 => "visitDateAsc",
                _ => "visitDateDesc"
            };

            VisitsDataGrid.ItemsSource =
                await _visitService.GetVisitsAsync(
                    search,
                    hallId,
                    sortBy);
        }

        private async void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            await LoadVisitsAsync();
        }

        private async void Filter_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }

            await LoadVisitsAsync();
        }

        private void VisitsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VisitsDataGrid.SelectedItem is not VisitListDto visit)
            {
                ClearVisitCard();

                return;
            }

            _selectedVisit = visit;

            FillVisitCard(visit);
        }

        private void FillVisitCard(VisitListDto visit)
        {
            ReaderTextBlock.Text = visit.ReaderFullName;
            LibraryTextBlock.Text = visit.LibraryName;
            HallTextBlock.Text = visit.HallName;
            EmployeeTextBlock.Text = visit.EmployeeFullName;

            VisitDateTextBlock.Text =
                visit.VisitDate.ToString("dd.MM.yyyy HH:mm");
        }

        private void ClearVisitCard()
        {
            _selectedVisit = null;

            ReaderTextBlock.Text = string.Empty;
            LibraryTextBlock.Text = string.Empty;
            HallTextBlock.Text = string.Empty;
            EmployeeTextBlock.Text = string.Empty;
            VisitDateTextBlock.Text = string.Empty;
        }

        private async void AddVisitButton_Click(object sender, RoutedEventArgs e)
        {
            CreateVisitWindow window = new();

            window.Owner = Window.GetWindow(this);

            window.ShowDialog();

            if (window.IsSaved)
            {
                await LoadVisitsAsync();
            }
        }
    }
}