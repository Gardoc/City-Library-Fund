using CityLibraryFund.Data.Context;
using CityLibraryFund.Data.DTOs;
using CityLibraryFund.WPF.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Windows;
using System.Windows.Controls;

namespace CityLibraryFund.WPF.Views
{
    /// <summary>
    /// Логика взаимодействия для AnalyticsQueriesView.xaml
    /// </summary>
    public partial class AnalyticsQueriesView : UserControl
    {
        private readonly AnalyticsService _analyticsService = new();

        public AnalyticsQueriesView()
        {
            InitializeComponent();

            Loaded += AnalyticsQueriesView_Loaded;
        }

        private async void AnalyticsQueriesView_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadFiltersAsync();

            LoanDateFromPicker.SelectedDate = DateTime.Now.AddMonths(-1);
            LoanDateToPicker.SelectedDate = DateTime.Now;

            FundDateFromPicker.SelectedDate = DateTime.Now.AddMonths(-1);
            FundDateToPicker.SelectedDate = DateTime.Now;
        }

        private async Task LoadFiltersAsync()
        {
            ReaderTypeComboBox.Items.Add("Все типы");

            using AppDbContext context = DbContextFactory.Create();

            List<string> readerTypes = await context.ReaderTypes
                .OrderBy(rt => rt.Name)
                .Select(rt => rt.Name)
                .ToListAsync();

            foreach (string readerType in readerTypes)
            {
                ReaderTypeComboBox.Items.Add(readerType);
            }

            ReaderTypeComboBox.SelectedIndex = 0;
        }

        private async void LoadReadersButton_Click(object sender, RoutedEventArgs e)
        {
            ReadersDataGrid.ItemsSource =
                await _analyticsService.GetReadersByCharacteristicsAsync(
                    ReaderTypeComboBox.SelectedIndex <= 0
                        ? null
                        : ReaderTypeComboBox.SelectedItem?.ToString(),

                    UniversityTextBox.Text,
                    FacultyTextBox.Text,
                    ScientificTopicTextBox.Text);
        }

        private async void LoadOverdueReadersButton_Click(object sender, RoutedEventArgs e)
        {
            ReadersDataGrid.ItemsSource =
                await _analyticsService.GetOverdueReadersAsync();
        }

        private async void LoadInactiveReadersButton_Click(object sender, RoutedEventArgs e)
        {
            ReadersDataGrid.ItemsSource =
                await _analyticsService.GetInactiveReadersAsync(
                    DateTime.Now.AddMonths(-6));
        }

        private async void LoadReadersByWorkButton_Click(object sender, RoutedEventArgs e)
        {
            if (LoanDateFromPicker.SelectedDate == null ||
                LoanDateToPicker.SelectedDate == null)
            {
                MessageBox.Show(
                    "Выберите период.",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            LoansDataGrid.ItemsSource =
                await _analyticsService.GetReadersByWorkAsync(
                    WorkTitleTextBox.Text,

                    DateOnly.FromDateTime(
                        LoanDateFromPicker.SelectedDate.Value),

                    DateOnly.FromDateTime(
                        LoanDateToPicker.SelectedDate.Value));
        }

        private async void LoadReceiptsButton_Click(object sender, RoutedEventArgs e)
        {
            await LoadFundAsync(true);
        }

        private async void LoadWriteOffsButton_Click(object sender, RoutedEventArgs e)
        {
            await LoadFundAsync(false);
        }

        private async Task LoadFundAsync(bool isReceipt)
        {
            if (FundDateFromPicker.SelectedDate == null ||
                FundDateToPicker.SelectedDate == null)
            {
                MessageBox.Show(
                    "Выберите период.",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            FundDataGrid.ItemsSource =
                await _analyticsService.GetReceiptWriteOffAsync(
                    DateOnly.FromDateTime(
                        FundDateFromPicker.SelectedDate.Value),

                    DateOnly.FromDateTime(
                        FundDateToPicker.SelectedDate.Value),

                    isReceipt);
        }
    }
}