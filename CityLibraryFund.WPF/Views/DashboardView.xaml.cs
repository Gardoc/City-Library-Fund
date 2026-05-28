using CityLibraryFund.Data.Context;
using CityLibraryFund.WPF.Services;
using System.Windows.Controls;

namespace CityLibraryFund.WPF.Views
{
    /// <summary>
    /// Логика взаимодействия для DashboardView.xaml
    /// </summary>
    public partial class DashboardView : UserControl
    {
        private readonly DashboardService _dashboardService;

        public DashboardView()
        {
            InitializeComponent();

            var context = DbContextFactory.Create();

            _dashboardService = new DashboardService(context);

            Loaded += DashboardView_Loaded;
        }

        private async void DashboardView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var statistics = await _dashboardService.GetStatisticsAsync();

            if (statistics != null)
            {
                ReadersCountTextBlock.Text =
                    statistics.ReadersCount.ToString();

                BooksCountTextBlock.Text =
                    statistics.LibraryItemsCount.ToString();

                LoansCountTextBlock.Text =
                    statistics.ActiveLoansCount.ToString();

                OverdueCountTextBlock.Text =
                    statistics.OverdueReadersCount.ToString();
            }

            LoansDataGrid.ItemsSource =
                await _dashboardService.GetCurrentLoansAsync();

            OverdueDataGrid.ItemsSource =
                await _dashboardService.GetOverdueReadersAsync();

            PopularWorksDataGrid.ItemsSource =
                await _dashboardService.GetPopularWorksAsync();

            EmployeePerformanceDataGrid.ItemsSource =
                await _dashboardService.GetEmployeePerformanceAsync();
        }
    }
}