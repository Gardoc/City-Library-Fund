using CityLibraryFund.Data.DTOs;
using CityLibraryFund.WPF.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace CityLibraryFund.WPF.Windows
{
    /// <summary>
    /// Логика взаимодействия для WorkInventoryWindow.xaml
    /// </summary>
    public partial class WorkInventoryWindow : Window
    {
        private readonly WorkService _workService = new();

        private readonly int _workId;

        private readonly string _workTitle;

        public WorkInventoryWindow(int workId, string workTitle)
        {
            InitializeComponent();

            _workId = workId;
            _workTitle = workTitle;

            Loaded += WorkInventoryWindow_Loaded;
        }

        private async void WorkInventoryWindow_Loaded(object sender, RoutedEventArgs e)
        {
            WorkTitleTextBlock.Text = _workTitle;

            await LoadInventoryAsync();
        }

        private async Task LoadInventoryAsync()
        {
            List<WorkInventoryDto> inventory =
                await _workService.GetWorkInventoryAsync(_workId);

            InventoryDataGrid.ItemsSource = inventory;
        }
    }
}