using CityLibraryFund.WPF.Services;
using CityLibraryFund.WPF.Views;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CityLibraryFund.WPF.Windows
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Sidebar.NavigationRequested += Sidebar_NavigationRequested;

            OpenStartPage();
        }

        private void Sidebar_NavigationRequested(string destination)
        {
            MainContent.Content = destination switch
            {
                "Authors" => new AuthorsView(),
                "Dashboard" => new DashboardView(),
                "Editions" => new EditionsView(),
                "Employees" => new EmployeesView(),
                "Readers" => new ReadersView(),
                "Libraries" => new LibrariesView(),
                "LibraryItems" => new LibraryItemsView(),
                "Loans" => new LoansView(),
                "Halls" => new HallsView(),
                "Visits" => new VisitsView(),
                "Works" => new WorksView(),
                "GeneralAnalytics" => new DashboardView(),
                "AnalyticQueries" => new AnalyticsQueriesView(),
                _ => MainContent.Content
            };
        }

        private void OpenStartPage()
        {
            string role = CurrentUserService.Role;

            switch (role)
            {
                case "Администратор":

                    Sidebar_NavigationRequested(
                        "Libraries");

                    break;

                case "Библиотекарь":

                    Sidebar_NavigationRequested(
                        "Readers");

                    break;

                case "Аналитик":

                    Sidebar_NavigationRequested(
                        "GeneralAnalytics");

                    break;
            }
        }
    }
}