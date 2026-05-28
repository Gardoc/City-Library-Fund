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
    /// Логика взаимодействия для WorksView.xaml
    /// </summary>
    public partial class WorksView : UserControl
    {
        private readonly WorkService _workService = new();

        private WorkDetailsDto? _selectedWork;

        public WorksView()
        {
            InitializeComponent();

            ClearWorkCard();
        }

        private async void WorksView_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadFiltersAsync();
            await LoadWorksAsync();
        }

        private async Task LoadFiltersAsync()
        {
            WorkTypeComboBox.Items.Add(new WorkType
            {
                Id = 0,
                Name = "Все типы"
            });

            var workTypes = await _workService.GetWorkTypesAsync();

            foreach (WorkType workType in workTypes)
            {
                WorkTypeComboBox.Items.Add(workType);
            }

            WorkTypeComboBox.SelectedIndex = 0;

            LanguageComboBox.Items.Add("Все языки");

            var languages = await _workService.GetLanguagesAsync();

            foreach (string language in languages)
            {
                LanguageComboBox.Items.Add(language);
            }

            LanguageComboBox.SelectedIndex = 0;

            SortComboBox.Items.Add("Название ↑");
            SortComboBox.Items.Add("Название ↓");
            SortComboBox.Items.Add("Год написания ↑");
            SortComboBox.Items.Add("Год написания ↓");
            SortComboBox.Items.Add("Количество изданий ↓");

            SortComboBox.SelectedIndex = 0;
        }

        private async Task LoadWorksAsync()
        {
            string search = SearchTextBox.Text.Trim();

            int? workTypeId = WorkTypeComboBox.SelectedIndex <= 0
                ? null
                : ((WorkType)WorkTypeComboBox.SelectedItem).Id;

            string? language = LanguageComboBox.SelectedIndex <= 0
                ? null
                : LanguageComboBox.SelectedItem?.ToString();

            string sortBy = SortComboBox.SelectedIndex switch
            {
                1 => "titleDesc",
                2 => "yearAsc",
                3 => "yearDesc",
                4 => "editionsCountDesc",
                _ => "titleAsc"
            };

            WorksDataGrid.ItemsSource =
                await _workService.GetWorksAsync(
                    search,
                    workTypeId,
                    language,
                    sortBy);
        }

        private async void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            await LoadWorksAsync();
        }

        private async void Filter_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }

            await LoadWorksAsync();
        }

        private async void WorksDataGrid_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (WorksDataGrid.SelectedItem is not WorkListDto work)
            {
                ClearWorkCard();

                return;
            }

            _selectedWork = await _workService.GetWorkDetailsAsync(work.Id);

            if (_selectedWork == null)
            {
                return;
            }

            FillWorkCard(_selectedWork);
        }

        private void FillWorkCard(WorkDetailsDto work)
        {
            ReadersWithWorkButton.IsEnabled = true;
            InventoryButton.IsEnabled = true;
            EditButton.IsEnabled = true;

            DeleteButton.IsEnabled = work.EditionsCount == 0;

            TitleTextBlock.Text = work.Title;
            WorkTypeTextBlock.Text = work.WorkType;
            AuthorsTextBlock.Text = work.Authors;

            YearWrittenTextBlock.Text =
                work.YearWritten?.ToString() ?? "-";

            LanguageTextBlock.Text =
                string.IsNullOrWhiteSpace(work.Language)
                ? "-"
                : work.Language;

            DescriptionTextBlock.Text = work.Description;

            EditionsCountTextBlock.Text =
                work.EditionsCount.ToString();

            CopiesCountTextBlock.Text =
                work.CopiesCount.ToString();

            LatestEditionTextBlock.Text = work.LatestEdition;

            DescriptionPanel.Visibility =
                string.IsNullOrWhiteSpace(work.Description)
                ? Visibility.Collapsed
                : Visibility.Visible;

            LatestEditionPanel.Visibility =
                string.IsNullOrWhiteSpace(work.LatestEdition)
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        private void ClearWorkCard()
        {
            _selectedWork = null;

            TitleTextBlock.Text = string.Empty;
            WorkTypeTextBlock.Text = string.Empty;
            AuthorsTextBlock.Text = string.Empty;
            YearWrittenTextBlock.Text = string.Empty;
            LanguageTextBlock.Text = string.Empty;
            DescriptionTextBlock.Text = string.Empty;
            EditionsCountTextBlock.Text = string.Empty;
            CopiesCountTextBlock.Text = string.Empty;
            LatestEditionTextBlock.Text = string.Empty;

            DescriptionPanel.Visibility = Visibility.Collapsed;
            LatestEditionPanel.Visibility = Visibility.Collapsed;

            ReadersWithWorkButton.IsEnabled = false;
            InventoryButton.IsEnabled = false;
            EditButton.IsEnabled = false;
            DeleteButton.IsEnabled = false;
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            WorkAddEditWindow window = new();

            window.Owner = Window.GetWindow(this);

            window.ShowDialog();

            if (window.IsSaved)
            {
                await LoadWorksAsync();
            }
        }

        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedWork == null)
            {
                return;
            }

            int workId = _selectedWork.Id;

            WorkEditDto? dto =
                await _workService.GetWorkForEditAsync(workId);

            if (dto == null)
            {
                return;
            }

            WorkAddEditWindow window = new(dto);

            window.Owner = Window.GetWindow(this);

            window.ShowDialog();

            if (window.IsSaved)
            {
                await ReloadAndSelectWorkAsync(workId);
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedWork == null)
            {
                return;
            }

            MessageBoxResult confirm = MessageBox.Show(
                "Удалить произведение?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirm != MessageBoxResult.Yes)
            {
                return;
            }

            int workId = _selectedWork.Id;

            OperationResultDto result =
                await _workService.DeleteWorkAsync(workId);

            if (!result.IsSuccess)
            {
                MessageBox.Show(
                    result.ErrorMessage,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            await LoadWorksAsync();

            ClearWorkCard();
        }

        private void InventoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedWork == null)
            {
                return;
            }

            WorkInventoryWindow window = new(
                _selectedWork.Id,
                _selectedWork.Title);

            window.Owner = Window.GetWindow(this);

            window.ShowDialog();
        }

        private void ReadersWithWorkButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedWork == null)
            {
                return;
            }

            WorkReadersWindow window = new(
                _selectedWork.Id,
                _selectedWork.Title);

            window.Owner = Window.GetWindow(this);

            window.ShowDialog();
        }

        private async Task ReloadAndSelectWorkAsync(int workId)
        {
            await LoadWorksAsync();

            if (WorksDataGrid.ItemsSource is not List<WorkListDto> works)
            {
                return;
            }

            WorkListDto? selectedWork =
                works.FirstOrDefault(w => w.Id == workId);

            if (selectedWork == null)
            {
                ClearWorkCard();

                return;
            }

            WorksDataGrid.SelectedItem = selectedWork;

            WorksDataGrid.ScrollIntoView(selectedWork);

            _selectedWork =
                await _workService.GetWorkDetailsAsync(workId);

            if (_selectedWork != null)
            {
                FillWorkCard(_selectedWork);
            }
        }
    }
}