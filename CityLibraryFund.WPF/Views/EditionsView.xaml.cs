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
    /// Логика взаимодействия для EditionsView.xaml
    /// </summary>
    public partial class EditionsView : UserControl
    {
        private readonly EditionService _editionService = new();
        private EditionDetailsDto? _selectedEdition;

        public EditionsView()
        {
            InitializeComponent();

            ClearEditionCard();
        }

        private async void EditionsView_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadFiltersAsync();
            await LoadEditionsAsync();
        }

        private async Task LoadFiltersAsync()
        {
            WorkTypeComboBox.Items.Add(new WorkType
            {
                Id = 0,
                Name = "Все типы"
            });

            var workTypes = await _editionService.GetWorkTypesAsync();

            foreach (WorkType workType in workTypes)
            {
                WorkTypeComboBox.Items.Add(workType);
            }

            WorkTypeComboBox.SelectedIndex = 0;

            SortComboBox.Items.Add("Название ↑");
            SortComboBox.Items.Add("Название ↓");
            SortComboBox.Items.Add("Год издания ↑");
            SortComboBox.Items.Add("Год издания ↓");
            SortComboBox.Items.Add("Количество экземпляров ↓");

            SortComboBox.SelectedIndex = 0;
        }

        private async Task LoadEditionsAsync()
        {
            string search = SearchTextBox.Text.Trim();

            int? workTypeId = WorkTypeComboBox.SelectedIndex <= 0
                ? null
                : ((WorkType)WorkTypeComboBox.SelectedItem).Id;

            string sortBy = SortComboBox.SelectedIndex switch
            {
                1 => "titleDesc",
                2 => "publishYearAsc",
                3 => "publishYearDesc",
                4 => "copiesCountDesc",
                _ => "titleAsc"
            };

            EditionsDataGrid.ItemsSource =
                await _editionService.GetEditionsAsync(
                    search,
                    workTypeId,
                    sortBy);
        }

        private async void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            await LoadEditionsAsync();
        }

        private async void Filter_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }

            await LoadEditionsAsync();
        }

        private async void EditionsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EditionsDataGrid.SelectedItem is not EditionListDto edition)
            {
                ClearEditionCard();

                return;
            }

            _selectedEdition =
                await _editionService.GetEditionDetailsAsync(edition.Id);

            if (_selectedEdition == null)
            {
                return;
            }

            FillEditionCard(_selectedEdition);
        }

        private void FillEditionCard(EditionDetailsDto edition)
        {
            ReadersButton.IsEnabled = edition.CopiesCount > 0;
            EditButton.IsEnabled = true;
            DeleteButton.IsEnabled = edition.CopiesCount == 0;

            WorkTitleTextBlock.Text = edition.WorkTitle;
            AuthorsTextBlock.Text = edition.Authors;
            WorkTypeTextBlock.Text = edition.WorkType;
            PublisherTextBlock.Text = edition.Publisher;

            PublishYearTextBlock.Text =
                edition.PublishYear.ToString();

            EditionNumberTextBlock.Text =
                edition.EditionNumber?.ToString();

            IsbnTextBlock.Text = edition.Isbn;

            PageCountTextBlock.Text =
                edition.PageCount?.ToString();

            LanguageTextBlock.Text =
                string.IsNullOrWhiteSpace(edition.Language)
                ? "-"
                : edition.Language;

            CopiesCountTextBlock.Text =
                edition.CopiesCount.ToString();

            DescriptionTextBlock.Text =
                edition.Description;

            EditionNumberPanel.Visibility =
                edition.EditionNumber == null
                ? Visibility.Collapsed
                : Visibility.Visible;

            IsbnPanel.Visibility =
                string.IsNullOrWhiteSpace(edition.Isbn)
                ? Visibility.Collapsed
                : Visibility.Visible;

            PageCountPanel.Visibility =
                edition.PageCount == null
                ? Visibility.Collapsed
                : Visibility.Visible;

            DescriptionPanel.Visibility =
                string.IsNullOrWhiteSpace(edition.Description)
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        private void ReadersButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedEdition == null)
            {
                return;
            }

            string editionInfo =
                _selectedEdition.WorkTitle +
                " (" +
                _selectedEdition.Publisher +
                ", " +
                _selectedEdition.PublishYear +
                ")";

            EditionReadersWindow window = new(
                _selectedEdition.Id,
                editionInfo);

            window.Owner = Window.GetWindow(this);

            window.ShowDialog();
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            EditionAddEditWindow window = new();

            window.Owner = Window.GetWindow(this);

            window.ShowDialog();

            if (window.IsSaved)
            {
                await LoadEditionsAsync();
            }
        }

        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedEdition == null)
            {
                return;
            }

            int editionId = _selectedEdition.Id;

            EditionEditDto? dto =
                await _editionService.GetEditionForEditAsync(editionId);

            if (dto == null)
            {
                return;
            }

            EditionAddEditWindow window = new(dto);

            window.Owner = Window.GetWindow(this);

            window.ShowDialog();

            if (window.IsSaved)
            {
                await ReloadAndSelectEditionAsync(editionId);
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedEdition == null)
            {
                return;
            }

            MessageBoxResult confirm = MessageBox.Show(
                "Удалить издание?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirm != MessageBoxResult.Yes)
            {
                return;
            }

            int editionId = _selectedEdition.Id;

            OperationResultDto result =
                await _editionService.DeleteEditionAsync(editionId);

            if (!result.IsSuccess)
            {
                MessageBox.Show(
                    result.ErrorMessage,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            await LoadEditionsAsync();

            ClearEditionCard();
        }

        private void ClearEditionCard()
        {
            _selectedEdition = null;

            WorkTitleTextBlock.Text = string.Empty;
            AuthorsTextBlock.Text = string.Empty;
            WorkTypeTextBlock.Text = string.Empty;
            PublisherTextBlock.Text = string.Empty;
            PublishYearTextBlock.Text = string.Empty;
            EditionNumberTextBlock.Text = string.Empty;
            IsbnTextBlock.Text = string.Empty;
            PageCountTextBlock.Text = string.Empty;
            LanguageTextBlock.Text = string.Empty;
            CopiesCountTextBlock.Text = string.Empty;
            DescriptionTextBlock.Text = string.Empty;

            EditionNumberPanel.Visibility = Visibility.Collapsed;
            IsbnPanel.Visibility = Visibility.Collapsed;
            PageCountPanel.Visibility = Visibility.Collapsed;
            DescriptionPanel.Visibility = Visibility.Collapsed;

            ReadersButton.IsEnabled = false;
            EditButton.IsEnabled = false;
            DeleteButton.IsEnabled = false;
        }

        private async Task ReloadAndSelectEditionAsync(int editionId)
        {
            await LoadEditionsAsync();

            if (EditionsDataGrid.ItemsSource is not List<EditionListDto> editions)
            {
                return;
            }

            EditionListDto? selectedEdition =
                editions.FirstOrDefault(e => e.Id == editionId);

            if (selectedEdition == null)
            {
                ClearEditionCard();

                return;
            }

            EditionsDataGrid.SelectedItem = selectedEdition;
            EditionsDataGrid.ScrollIntoView(selectedEdition);

            _selectedEdition =
                await _editionService.GetEditionDetailsAsync(editionId);

            if (_selectedEdition != null)
            {
                FillEditionCard(_selectedEdition);
            }
        }
    }
}