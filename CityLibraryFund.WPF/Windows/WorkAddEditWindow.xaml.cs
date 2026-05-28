using CityLibraryFund.Data.DTOs;
using CityLibraryFund.Data.Entities;
using CityLibraryFund.WPF.Helpers;
using CityLibraryFund.WPF.Services;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CityLibraryFund.WPF.Windows
{
    /// <summary>
    /// Логика взаимодействия для WorkAddEditWindow.xaml
    /// </summary>
    public partial class WorkAddEditWindow : Window
    {
        private readonly WorkService _workService = new();

        private readonly WorkEditDto? _work;

        private List<Author> _allAuthors = new();

        private List<Author> _selectedAuthors = new();

        public bool IsSaved { get; private set; }

        public WorkAddEditWindow(WorkEditDto? work = null)
        {
            InitializeComponent();

            _work = work;

            Loaded += WorkAddEditWindow_Loaded;
        }

        private async void WorkAddEditWindow_Loaded(object sender, RoutedEventArgs e)
        {
            WorkTypeComboBox.ItemsSource =
                await _workService.GetWorkTypesAsync();

            _allAuthors =
                await _workService.GetAuthorsAsync();

            if (_work != null)
            {
                FillForm(_work);
            }

            RefreshAuthorLists();
        }

        private void FillForm(WorkEditDto work)
        {
            TitleTextBox.Text = work.Title;

            YearWrittenTextBox.Text =
                work.YearWritten?.ToString();

            LanguageTextBox.Text = work.Language;
            DescriptionTextBox.Text = work.Description;

            WorkTypeComboBox.SelectedValue =
                work.WorkTypeId;

            _selectedAuthors = _allAuthors
                .Where(a => work.AuthorIds.Contains(a.Id))
                .ToList();
        }

        private void RefreshAuthorLists()
        {
            string search =
                AuthorSearchTextBox.Text
                    .Trim()
                    .ToLower();

            var availableAuthors = _allAuthors
                .Where(a =>
                    !_selectedAuthors.Any(sa => sa.Id == a.Id))
                .Where(a =>
                    string.IsNullOrWhiteSpace(search) ||
                    a.FullName.ToLower().Contains(search))
                .OrderBy(a => a.LastName)
                .ThenBy(a => a.FirstName)
                .ToList();

            AllAuthorsListBox.ItemsSource = availableAuthors;

            SelectedAuthorsListBox.ItemsSource = null;

            SelectedAuthorsListBox.ItemsSource =
                _selectedAuthors
                    .OrderBy(a => a.LastName)
                    .ThenBy(a => a.FirstName)
                    .ToList();
        }

        private void AuthorSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshAuthorLists();
        }

        private void AddAuthorButton_Click(object sender, RoutedEventArgs e)
        {
            if (AllAuthorsListBox.SelectedItem is not Author author)
            {
                return;
            }

            _selectedAuthors.Add(author);

            RefreshAuthorLists();
        }

        private void RemoveAuthorButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedAuthorsListBox.SelectedItem is not Author author)
            {
                return;
            }

            _selectedAuthors.Remove(author);

            RefreshAuthorLists();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (WorkTypeComboBox.SelectedValue == null)
            {
                MessageBox.Show(
                    "Выберите тип произведения.",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            int? yearWritten = null;

            if (!string.IsNullOrWhiteSpace(
                YearWrittenTextBox.Text))
            {
                bool parsed =
                    int.TryParse(
                        YearWrittenTextBox.Text,
                        out int year);

                if (!parsed)
                {
                    MessageBox.Show(
                        "Введите корректный год.",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }

                yearWritten = year;
            }

            WorkEditDto dto = new()
            {
                Id = _work?.Id ?? 0,

                Title = TitleTextBox.Text,

                WorkTypeId =
                    (int)WorkTypeComboBox.SelectedValue,

                YearWritten = yearWritten,

                Language = LanguageTextBox.Text,
                Description = DescriptionTextBox.Text,

                AuthorIds =
                    _selectedAuthors
                        .Select(a => a.Id)
                        .ToList()
            };

            OperationResultDto result;

            if (_work == null)
            {
                result =
                    await _workService.CreateWorkAsync(dto);
            }
            else
            {
                result =
                    await _workService.UpdateWorkAsync(dto);
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