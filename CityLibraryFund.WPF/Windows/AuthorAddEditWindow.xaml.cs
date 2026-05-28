using CityLibraryFund.Data.DTOs;
using CityLibraryFund.WPF.Services;
using System;
using System.Windows;

namespace CityLibraryFund.WPF.Windows
{
    /// <summary>
    /// Логика взаимодействия для AuthorAddEditWindow.xaml
    /// </summary>
    public partial class AuthorAddEditWindow : Window
    {
        private readonly AuthorService _authorService = new();

        private readonly AuthorEditDto? _author;

        public bool IsSaved { get; private set; }

        public AuthorAddEditWindow(AuthorEditDto? author = null)
        {
            InitializeComponent();

            _author = author;

            if (_author != null)
            {
                FillForm(_author);
            }
        }

        private void FillForm(AuthorEditDto author)
        {
            FirstNameTextBox.Text = author.FirstName;
            LastNameTextBox.Text = author.LastName;
            CountryTextBox.Text = author.Country;

            BirthDatePicker.SelectedDate =
                author.BirthDate?.ToDateTime(TimeOnly.MinValue);
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            AuthorEditDto dto = new()
            {
                Id = _author?.Id ?? 0,
                FirstName = FirstNameTextBox.Text,
                LastName = LastNameTextBox.Text,
                Country = CountryTextBox.Text,

                BirthDate = BirthDatePicker.SelectedDate != null
                    ? DateOnly.FromDateTime(BirthDatePicker.SelectedDate.Value)
                    : null
            };

            OperationResultDto result;

            if (_author == null)
            {
                result = await _authorService.CreateAuthorAsync(dto);
            }
            else
            {
                result = await _authorService.UpdateAuthorAsync(dto);
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

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}