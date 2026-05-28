using CityLibraryFund.Data.DTOs;
using CityLibraryFund.WPF.Helpers;
using CityLibraryFund.WPF.Services;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CityLibraryFund.WPF.Windows
{
    /// <summary>
    /// Логика взаимодействия для ReaderAddEditWindow.xaml
    /// </summary>
    public partial class ReaderAddEditWindow : Window
    {
        private readonly ReaderService _readerService = new();

        private readonly ReaderEditDto? _reader;

        public bool IsSaved { get; private set; }

        public ReaderAddEditWindow(ReaderEditDto? reader = null)
        {
            InitializeComponent();

            _reader = reader;

            Loaded += ReaderAddEditWindow_Loaded;
        }

        private async void ReaderAddEditWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ReaderTypeComboBox.ItemsSource =
                await _readerService.GetReaderTypesForEditAsync();

            LibraryComboBox.ItemsSource =
                await _readerService.GetLibrariesAsync();

            if (_reader != null)
            {
                FillForm(_reader);
            }
            else
            {
                ReaderTypeComboBox.IsEnabled = true;
            }
        }

        private void FillForm(ReaderEditDto reader)
        {
            ReaderTypeComboBox.IsEnabled = false;

            FirstNameTextBox.Text = reader.FirstName;
            LastNameTextBox.Text = reader.LastName;
            PatronymicTextBox.Text = reader.Patronymic;

            BirthDatePicker.SelectedDate =
                reader.BirthDate.ToDateTime(TimeOnly.MinValue);

            ReaderTypeComboBox.SelectedValue =
                reader.ReaderTypeId;

            LibraryComboBox.SelectedValue =
                reader.LibraryId;

            PhoneTextBox.Text = reader.Phone;
            EmailTextBox.Text = reader.Email;
            AddressTextBox.Text = reader.Address;

            UniversityTextBox.Text = reader.University;
            FacultyTextBox.Text = reader.Faculty;

            CourseTextBox.Text =
                reader.Course?.ToString();

            GroupTextBox.Text = reader.GroupName;

            TeacherUniversityTextBox.Text =
                reader.University;

            DepartmentTextBox.Text = reader.Department;

            OrganizationTextBox.Text =
                reader.Organization;

            ScientificTopicTextBox.Text =
                reader.ScientificTopic;

            SchoolTextBox.Text = reader.School;

            ClassNumberTextBox.Text =
                reader.ClassNumber?.ToString();

            UpdateReaderTypePanels();
        }

        private void ReaderTypeComboBox_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }

            UpdateReaderTypePanels();
        }

        private void UpdateReaderTypePanels()
        {
            StudentPanel.Visibility = Visibility.Collapsed;
            TeacherPanel.Visibility = Visibility.Collapsed;
            ScientistPanel.Visibility = Visibility.Collapsed;
            SchoolerPanel.Visibility = Visibility.Collapsed;

            if (ReaderTypeComboBox.SelectedItem == null)
            {
                return;
            }

            string readerType =
                ((dynamic)ReaderTypeComboBox.SelectedItem).Name;

            if (readerType == "Студент")
            {
                StudentPanel.Visibility = Visibility.Visible;
            }
            else if (readerType == "Преподаватель")
            {
                TeacherPanel.Visibility = Visibility.Visible;
            }
            else if (readerType == "Научный работник")
            {
                ScientistPanel.Visibility = Visibility.Visible;
            }
            else if (readerType == "Школьник")
            {
                SchoolerPanel.Visibility = Visibility.Visible;
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (BirthDatePicker.SelectedDate == null)
            {
                MessageBox.Show(
                    "Выберите дату рождения.",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (ReaderTypeComboBox.SelectedValue == null)
            {
                MessageBox.Show(
                    "Выберите тип читателя.",
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

            ReaderEditDto dto = new()
            {
                Id = _reader?.Id ?? 0,

                FirstName = FirstNameTextBox.Text,
                LastName = LastNameTextBox.Text,
                Patronymic = PatronymicTextBox.Text,

                BirthDate =
                    DateOnly.FromDateTime(
                        BirthDatePicker.SelectedDate.Value),

                ReaderTypeId =
                    (int)ReaderTypeComboBox.SelectedValue,

                LibraryId =
                    (int)LibraryComboBox.SelectedValue,

                Phone = PhoneTextBox.Text,
                Email = EmailTextBox.Text,
                Address = AddressTextBox.Text,

                University =
                    StudentPanel.Visibility == Visibility.Visible
                    ? UniversityTextBox.Text
                    : TeacherUniversityTextBox.Text,

                Faculty = FacultyTextBox.Text,
                GroupName = GroupTextBox.Text,
                Department = DepartmentTextBox.Text,
                Organization = OrganizationTextBox.Text,
                ScientificTopic = ScientificTopicTextBox.Text,
                School = SchoolTextBox.Text
            };

            if (int.TryParse(CourseTextBox.Text, out int course))
            {
                dto.Course = course;
            }

            if (int.TryParse(
                ClassNumberTextBox.Text,
                out int classNumber))
            {
                dto.ClassNumber = classNumber;
            }

            OperationResultDto result;

            if (_reader == null)
            {
                result =
                    await _readerService.CreateReaderAsync(dto);
            }
            else
            {
                result =
                    await _readerService.UpdateReaderAsync(dto);
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