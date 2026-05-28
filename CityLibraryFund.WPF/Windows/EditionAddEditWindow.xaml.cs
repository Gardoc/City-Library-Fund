using CityLibraryFund.Data.DTOs;
using CityLibraryFund.Data.Entities;
using CityLibraryFund.WPF.Helpers;
using CityLibraryFund.WPF.Services;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CityLibraryFund.WPF.Windows
{
    /// <summary>
    /// Логика взаимодействия для EditionAddEditWindow.xaml
    /// </summary>
    public partial class EditionAddEditWindow : Window
    {
        private readonly EditionService _editionService = new();

        private readonly EditionEditDto? _edition;

        public bool IsSaved { get; private set; }

        public EditionAddEditWindow(EditionEditDto? edition = null)
        {
            InitializeComponent();

            _edition = edition;

            Loaded += EditionAddEditWindow_Loaded;
        }

        private async void EditionAddEditWindow_Loaded(object sender, RoutedEventArgs e)
        {
            WorkComboBox.ItemsSource =
                await _editionService.GetWorksAsync();

            if (_edition != null)
            {
                FillForm(_edition);
            }
        }

        private void FillForm(EditionEditDto edition)
        {
            WorkComboBox.SelectedValue = edition.WorkId;

            PublisherTextBox.Text = edition.Publisher;

            PublishYearTextBox.Text =
                edition.PublishYear.ToString();

            IsbnTextBox.Text = edition.Isbn;

            PageCountTextBox.Text =
                edition.PageCount?.ToString();

            EditionNumberTextBox.Text =
                edition.EditionNumber?.ToString();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(
                PublishYearTextBox.Text,
                out int publishYear))
            {
                MessageBox.Show(
                    "Введите корректный год издания.",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            int? pageCount = null;

            if (!string.IsNullOrWhiteSpace(PageCountTextBox.Text))
            {
                bool parsed =
                    int.TryParse(PageCountTextBox.Text, out int pages);

                if (!parsed)
                {
                    MessageBox.Show(
                        "Введите корректное количество страниц.",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }

                pageCount = pages;
            }

            int? editionNumber = null;

            if (!string.IsNullOrWhiteSpace(EditionNumberTextBox.Text))
            {
                bool parsed =
                    int.TryParse(
                        EditionNumberTextBox.Text,
                        out int number);

                if (!parsed)
                {
                    MessageBox.Show(
                        "Введите корректный номер издания.",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }

                editionNumber = number;
            }

            if (WorkComboBox.SelectedValue == null)
            {
                MessageBox.Show(
                    "Выберите произведение.",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            EditionEditDto dto = new()
            {
                Id = _edition?.Id ?? 0,

                WorkId = (int)WorkComboBox.SelectedValue,

                Publisher = PublisherTextBox.Text,

                PublishYear = publishYear,

                Isbn = string.IsNullOrWhiteSpace(IsbnTextBox.Text)
                    ? null
                    : IsbnTextBox.Text,

                PageCount = pageCount,

                EditionNumber = editionNumber
            };

            OperationResultDto result;

            if (_edition == null)
            {
                result =
                    await _editionService.CreateEditionAsync(dto);
            }
            else
            {
                result =
                    await _editionService.UpdateEditionAsync(dto);
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