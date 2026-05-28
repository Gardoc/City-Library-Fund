using CityLibraryFund.Data.DTOs;
using CityLibraryFund.WPF.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace CityLibraryFund.WPF.Windows
{
    /// <summary>
    /// Логика взаимодействия для WorkReadersWindow.xaml
    /// </summary>
    public partial class WorkReadersWindow : Window
    {
        private readonly WorkService _workService = new();

        private readonly int _workId;

        private readonly string _workTitle;

        public WorkReadersWindow(int workId, string workTitle)
        {
            InitializeComponent();

            _workId = workId;
            _workTitle = workTitle;

            Loaded += WorkReadersWindow_Loaded;
        }

        private async void WorkReadersWindow_Loaded(object sender, RoutedEventArgs e)
        {
            WorkTitleTextBlock.Text = _workTitle;

            await LoadReadersAsync();
        }

        private async Task LoadReadersAsync()
        {
            List<WorkReaderDto> readers =
                await _workService.GetWorkReadersAsync(_workId);

            ReadersDataGrid.ItemsSource = readers;
        }
    }
}