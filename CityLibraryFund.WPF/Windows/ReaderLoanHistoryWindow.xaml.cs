using CityLibraryFund.Data.DTOs;
using CityLibraryFund.WPF.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace CityLibraryFund.WPF.Windows
{
    /// <summary>
    /// Логика взаимодействия для ReaderLoanHistoryWindow.xaml
    /// </summary>
    public partial class ReaderLoanHistoryWindow : Window
    {
        private readonly ReaderService _readerService = new();

        private readonly int _readerId;

        private readonly string _readerName;

        public ReaderLoanHistoryWindow(int readerId, string readerName)
        {
            InitializeComponent();

            _readerId = readerId;
            _readerName = readerName;

            Loaded += ReaderLoanHistoryWindow_Loaded;
        }

        private async void ReaderLoanHistoryWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ReaderNameTextBlock.Text = _readerName;

            await LoadHistoryAsync();
        }

        private async Task LoadHistoryAsync()
        {
            List<LoanHistoryDto> history =
                await _readerService.GetReaderLoanHistoryAsync(_readerId);

            HistoryDataGrid.ItemsSource = history;
        }
    }
}