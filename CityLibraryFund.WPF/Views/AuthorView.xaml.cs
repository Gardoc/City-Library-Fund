using CityLibraryFund.Data.DTOs;
using CityLibraryFund.WPF.Services;
using CityLibraryFund.WPF.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CityLibraryFund.WPF.Views
{
    /// <summary>
    /// Логика взаимодействия для AuthorsView.xaml
    /// </summary>
    public partial class AuthorsView : UserControl
    {
        private readonly AuthorService _authorService = new();
        private AuthorDetailsDto? _selectedAuthor;

        public AuthorsView()
        {
            InitializeComponent();

            ClearAuthorCard();
        }

        private async void AuthorsView_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadFiltersAsync();
            await LoadAuthorsAsync();
        }

        private async Task LoadFiltersAsync()
        {
            CountryComboBox.Items.Add("Все страны");

            var countries = await _authorService.GetCountriesAsync();

            foreach (string country in countries)
            {
                CountryComboBox.Items.Add(country);
            }

            CountryComboBox.SelectedIndex = 0;

            SortComboBox.Items.Add("ФИО ↑");
            SortComboBox.Items.Add("ФИО ↓");
            SortComboBox.Items.Add("Дата рождения ↑");
            SortComboBox.Items.Add("Дата рождения ↓");
            SortComboBox.Items.Add("Количество произведений ↓");

            SortComboBox.SelectedIndex = 0;
        }

        private async Task LoadAuthorsAsync()
        {
            string search = SearchTextBox.Text.Trim();

            string? country = CountryComboBox.SelectedIndex <= 0
                ? null
                : CountryComboBox.SelectedItem?.ToString();

            string sortBy = SortComboBox.SelectedIndex switch
            {
                1 => "fullNameDesc",
                2 => "birthDateAsc",
                3 => "birthDateDesc",
                4 => "worksCountDesc",
                _ => "fullNameAsc"
            };

            AuthorsDataGrid.ItemsSource =
                await _authorService.GetAuthorsAsync(
                    search,
                    country,
                    sortBy);
        }

        private async void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            await LoadAuthorsAsync();
        }

        private async void Filter_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }

            await LoadAuthorsAsync();
        }

        private async void AuthorsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AuthorsDataGrid.SelectedItem is not AuthorListDto author)
            {
                ClearAuthorCard();

                return;
            }

            _selectedAuthor = await _authorService.GetAuthorDetailsAsync(author.Id);

            if (_selectedAuthor == null)
            {
                return;
            }

            FillAuthorCard(_selectedAuthor);
        }

        private void FillAuthorCard(AuthorDetailsDto author)
        {
            InventoryButton.IsEnabled = true;
            EditButton.IsEnabled = true;
            DeleteButton.IsEnabled = author.WorksCount == 0;

            FullNameTextBlock.Text = author.FullName;

            BirthDateTextBlock.Text =
                author.BirthDate?.ToString("dd.MM.yyyy") ?? "-";

            CountryTextBlock.Text =
                string.IsNullOrWhiteSpace(author.Country)
                ? "-"
                : author.Country;

            WorksCountTextBlock.Text = author.WorksCount.ToString();
            CopiesCountTextBlock.Text = author.CopiesCount.ToString();

            LatestWorkTextBlock.Text = author.LatestWork;

            LatestWorkPanel.Visibility =
                string.IsNullOrWhiteSpace(author.LatestWork)
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AuthorAddEditWindow window = new();

            window.Owner = Window.GetWindow(this);

            window.ShowDialog();

            if (window.IsSaved)
            {
                await LoadAuthorsAsync();
            }
        }

        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedAuthor == null)
            {
                return;
            }

            int authorId = _selectedAuthor.Id;

            AuthorEditDto? dto =
                await _authorService.GetAuthorForEditAsync(authorId);

            if (dto == null)
            {
                return;
            }

            AuthorAddEditWindow window = new(dto);

            window.Owner = Window.GetWindow(this);

            window.ShowDialog();

            if (window.IsSaved)
            {
                await ReloadAndSelectAuthorAsync(authorId);
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedAuthor == null)
            {
                return;
            }

            MessageBoxResult confirm = MessageBox.Show(
                "Удалить автора?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirm != MessageBoxResult.Yes)
            {
                return;
            }

            int authorId = _selectedAuthor.Id;

            OperationResultDto result =
                await _authorService.DeleteAuthorAsync(authorId);

            if (!result.IsSuccess)
            {
                MessageBox.Show(
                    result.ErrorMessage,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            await LoadAuthorsAsync();

            ClearAuthorCard();
        }

        private void InventoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedAuthor == null)
            {
                return;
            }

            AuthorInventoryWindow window = new(
                _selectedAuthor.Id,
                _selectedAuthor.FullName);

            window.Owner = Window.GetWindow(this);

            window.ShowDialog();
        }

        private void ClearAuthorCard()
        {
            _selectedAuthor = null;

            FullNameTextBlock.Text = string.Empty;
            BirthDateTextBlock.Text = string.Empty;
            CountryTextBlock.Text = string.Empty;
            WorksCountTextBlock.Text = string.Empty;
            CopiesCountTextBlock.Text = string.Empty;
            LatestWorkTextBlock.Text = string.Empty;

            LatestWorkPanel.Visibility = Visibility.Collapsed;

            InventoryButton.IsEnabled = false;
            EditButton.IsEnabled = false;
            DeleteButton.IsEnabled = false;
        }

        private async Task ReloadAndSelectAuthorAsync(int authorId)
        {
            await LoadAuthorsAsync();

            if (AuthorsDataGrid.ItemsSource is not List<AuthorListDto> authors)
            {
                return;
            }

            AuthorListDto? selectedAuthor =
                authors.FirstOrDefault(a => a.Id == authorId);

            if (selectedAuthor == null)
            {
                ClearAuthorCard();

                return;
            }

            AuthorsDataGrid.SelectedItem = selectedAuthor;
            AuthorsDataGrid.ScrollIntoView(selectedAuthor);

            _selectedAuthor =
                await _authorService.GetAuthorDetailsAsync(authorId);

            if (_selectedAuthor != null)
            {
                FillAuthorCard(_selectedAuthor);
            }
        }
    }
}