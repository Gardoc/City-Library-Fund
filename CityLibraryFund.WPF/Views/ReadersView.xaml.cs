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
    /// Логика взаимодействия для ReadersView.xaml
    /// </summary>
    public partial class ReadersView : UserControl
    {
        private readonly ReaderService _readerService = new();
        private ReaderDetailsDto? _selectedReader;

        public ReadersView()
        {
            InitializeComponent();

            ClearReaderCard();
        }

        private async void ReadersView_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadFiltersAsync();
            await LoadReadersAsync();
        }

        private async Task LoadFiltersAsync()
        {
            ReaderTypeComboBox.Items.Add("Все типы");

            var readerTypes = await _readerService.GetReaderTypesAsync();

            foreach (string type in readerTypes)
            {
                ReaderTypeComboBox.Items.Add(type);
            }

            ReaderTypeComboBox.SelectedIndex = 0;

            StatusComboBox.Items.Add("Все");
            StatusComboBox.Items.Add("Активные");
            StatusComboBox.Items.Add("Неактивные");

            StatusComboBox.SelectedIndex = 0;

            SortComboBox.Items.Add("ФИО ↑");
            SortComboBox.Items.Add("ФИО ↓");
            SortComboBox.Items.Add("Дата регистрации ↑");
            SortComboBox.Items.Add("Дата регистрации ↓");

            SortComboBox.SelectedIndex = 0;
        }

        private async Task LoadReadersAsync()
        {
            string search = SearchTextBox.Text.Trim();

            string? readerType = ReaderTypeComboBox.SelectedIndex <= 0
                ? null
                : ReaderTypeComboBox.SelectedItem?.ToString();

            bool? isActive = StatusComboBox.SelectedIndex switch
            {
                1 => true,
                2 => false,
                _ => null
            };

            string sortBy = SortComboBox.SelectedIndex switch
            {
                1 => "fullNameDesc",
                2 => "registrationDateDesc",
                3 => "registrationDateAsc",
                _ => "fullNameAsc"
            };

            ReadersDataGrid.ItemsSource =
                await _readerService.GetReadersAsync(
                    search,
                    readerType,
                    isActive,
                    sortBy);
        }

        private async void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            await LoadReadersAsync();
        }

        private async void Filter_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }

            await LoadReadersAsync();
        }

        private async void ReadersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ReadersDataGrid.SelectedItem is not ReaderListDto reader)
            {
                ClearReaderCard();

                return;
            }

            _selectedReader =
                await _readerService.GetReaderDetailsAsync(reader.Id);

            if (_selectedReader == null)
            {
                return;
            }

            FillReaderCard(_selectedReader);
        }

        private void FillReaderCard(ReaderDetailsDto reader)
        {
            HistoryButton.IsEnabled = reader.IsActive;
            EditButton.IsEnabled = reader.IsActive;

            DeactivateButton.IsEnabled = true;

            FullNameTextBlock.Text = reader.FullName;
            ReaderTypeTextBlock.Text = reader.ReaderType;

            BirthDateTextBlock.Text =
                reader.BirthDate.ToString("dd.MM.yyyy");

            RegistrationDateTextBlock.Text =
                reader.RegistrationDate.ToString("dd.MM.yyyy");

            PhoneTextBlock.Text = reader.Phone;
            EmailTextBlock.Text = reader.Email;
            AddressTextBlock.Text = reader.Address;

            LibraryTextBlock.Text = reader.LibraryName;

            UniversityTextBlock.Text = reader.University;
            FacultyTextBlock.Text = reader.Faculty;

            CourseTextBlock.Text =
                reader.Course?.ToString();

            GroupTextBlock.Text = reader.GroupName;

            DepartmentTextBlock.Text = reader.Department;

            OrganizationTextBlock.Text = reader.Organization;

            ScientificTopicTextBlock.Text =
                reader.ScientificTopic;

            SchoolTextBlock.Text = reader.School;

            SetPanelVisibility(PhonePanel, reader.Phone);
            SetPanelVisibility(EmailPanel, reader.Email);
            SetPanelVisibility(AddressPanel, reader.Address);

            SetPanelVisibility(UniversityPanel, reader.University);
            SetPanelVisibility(FacultyPanel, reader.Faculty);

            SetPanelVisibility(
                CoursePanel,
                reader.Course?.ToString());

            SetPanelVisibility(GroupPanel, reader.GroupName);

            SetPanelVisibility(DepartmentPanel, reader.Department);

            SetPanelVisibility(
                OrganizationPanel,
                reader.Organization);

            SetPanelVisibility(
                ScientificTopicPanel,
                reader.ScientificTopic);

            SetPanelVisibility(SchoolPanel, reader.School);

            DeactivateButton.Content = reader.IsActive
                ? "Деактивировать"
                : "Активировать";
        }

        private void SetPanelVisibility(StackPanel panel, string? value)
        {
            panel.Visibility = string.IsNullOrWhiteSpace(value)
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            ReaderAddEditWindow window = new();

            window.Owner = Window.GetWindow(this);

            window.ShowDialog();

            if (window.IsSaved)
            {
                await LoadReadersAsync();
            }
        }

        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedReader == null)
            {
                return;
            }

            int readerId = _selectedReader.Id;

            ReaderEditDto? dto =
                await _readerService.GetReaderForEditAsync(readerId);

            if (dto == null)
            {
                return;
            }

            ReaderAddEditWindow window = new(dto);

            window.Owner = Window.GetWindow(this);

            window.ShowDialog();

            if (window.IsSaved)
            {
                await ReloadAndSelectReaderAsync(readerId);
            }
        }

        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedReader == null)
            {
                MessageBox.Show(
                    "Выберите читателя.",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            ReaderLoanHistoryWindow window = new(
                _selectedReader.Id,
                _selectedReader.FullName);

            window.Owner = Window.GetWindow(this);

            window.ShowDialog();
        }

        private async void DeactivateButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedReader == null)
            {
                return;
            }

            int readerId = _selectedReader.Id;

            bool result =
                await _readerService.ToggleReaderActivityAsync(readerId);

            if (!result)
            {
                MessageBox.Show(
                    "Не удалось изменить статус.",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            await ReloadAndSelectReaderAsync(readerId);
        }

        private void ClearReaderCard()
        {
            _selectedReader = null;

            FullNameTextBlock.Text = string.Empty;

            ReaderTypeTextBlock.Text = string.Empty;
            BirthDateTextBlock.Text = string.Empty;
            RegistrationDateTextBlock.Text = string.Empty;

            PhoneTextBlock.Text = string.Empty;
            EmailTextBlock.Text = string.Empty;
            AddressTextBlock.Text = string.Empty;

            LibraryTextBlock.Text = string.Empty;

            UniversityTextBlock.Text = string.Empty;
            FacultyTextBlock.Text = string.Empty;
            CourseTextBlock.Text = string.Empty;
            GroupTextBlock.Text = string.Empty;

            DepartmentTextBlock.Text = string.Empty;
            OrganizationTextBlock.Text = string.Empty;
            ScientificTopicTextBlock.Text = string.Empty;

            SchoolTextBlock.Text = string.Empty;

            PhonePanel.Visibility = Visibility.Collapsed;
            EmailPanel.Visibility = Visibility.Collapsed;
            AddressPanel.Visibility = Visibility.Collapsed;

            UniversityPanel.Visibility = Visibility.Collapsed;
            FacultyPanel.Visibility = Visibility.Collapsed;
            CoursePanel.Visibility = Visibility.Collapsed;
            GroupPanel.Visibility = Visibility.Collapsed;

            DepartmentPanel.Visibility = Visibility.Collapsed;
            OrganizationPanel.Visibility = Visibility.Collapsed;
            ScientificTopicPanel.Visibility = Visibility.Collapsed;

            SchoolPanel.Visibility = Visibility.Collapsed;

            HistoryButton.IsEnabled = false;
            EditButton.IsEnabled = false;
            DeactivateButton.IsEnabled = false;

            DeactivateButton.Content = "Деактивировать";
        }

        private async Task ReloadAndSelectReaderAsync(int readerId)
        {
            await LoadReadersAsync();

            if (ReadersDataGrid.ItemsSource is not List<ReaderListDto> readers)
            {
                return;
            }

            ReaderListDto? selectedReader =
                readers.FirstOrDefault(r => r.Id == readerId);

            if (selectedReader == null)
            {
                ClearReaderCard();

                return;
            }

            ReadersDataGrid.SelectedItem = selectedReader;
            ReadersDataGrid.ScrollIntoView(selectedReader);

            _selectedReader =
                await _readerService.GetReaderDetailsAsync(readerId);

            if (_selectedReader != null)
            {
                FillReaderCard(_selectedReader);
            }
        }
    }
}