using CityLibraryFund.WPF.Services;
using CityLibraryFund.WPF.Windows;
using System;
using System.Windows;
using System.Windows.Controls;

namespace CityLibraryFund.WPF.Views
{
    /// <summary>
    /// Логика взаимодействия для SidebarView.xaml
    /// </summary>
    public partial class SidebarView : UserControl
    {
        public event Action<string>? NavigationRequested;

        private Button? _activeButton;

        public SidebarView()
        {
            InitializeComponent();

            UserTextBlock.Text = CurrentUserService.FullName;
            RoleTextBlock.Text = CurrentUserService.Role;

            LibraryTextBlock.Text =
                CurrentUserService.LibraryName;

            HallTextBlock.Text =
                CurrentUserService.HallName;

            ConfigureSidebarByRole();
        }

        private void ConfigureSidebarByRole()
        {
            string role = CurrentUserService.Role;

            switch (role)
            {
                case "Администратор":

                    LibrariesButton.Visibility = Visibility.Visible;
                    HallsButton.Visibility = Visibility.Visible;
                    AuthorsButton.Visibility = Visibility.Visible;
                    WorksButton.Visibility = Visibility.Visible;
                    EditionsButton.Visibility = Visibility.Visible;
                    LibraryItemsButton.Visibility = Visibility.Visible;
                    EmployeesButton.Visibility = Visibility.Visible;

                    ActivateButton(LibrariesButton);

                    break;

                case "Библиотекарь":

                    ReadersButton.Visibility = Visibility.Visible;
                    VisitsButton.Visibility = Visibility.Visible;
                    LoansButton.Visibility = Visibility.Visible;
                    LibrarianLibraryItemsButton.Visibility =
                        Visibility.Visible;

                    ActivateButton(ReadersButton);

                    break;

                case "Аналитик":

                    GeneralAnalyticsButton.Visibility =
                        Visibility.Visible;

                    AnalyticQueriesButton.Visibility =
                        Visibility.Visible;

                    ActivateButton(GeneralAnalyticsButton);

                    break;
            }
        }

        private void ActivateButton(Button button)
        {
            _activeButton = button;

            button.Style = (Style)Application.Current.Resources[
                "ActiveSidebarButton"];
        }

        private void NavigationButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (sender is not Button button ||
                button.Tag is not string destination)
            {
                return;
            }

            if (_activeButton != null)
            {
                _activeButton.Style =
                    (Style)Application.Current.Resources[
                        "SidebarButton"];
            }

            button.Style =
                (Style)Application.Current.Resources[
                    "ActiveSidebarButton"];

            _activeButton = button;

            NavigationRequested?.Invoke(destination);
        }

        private void LogoutButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            CurrentUserService.EmployeeId = 0;
            CurrentUserService.FullName = string.Empty;
            CurrentUserService.Role = string.Empty;
            CurrentUserService.HallId = 0;
            CurrentUserService.HallName = string.Empty;
            CurrentUserService.LibraryId = 0;
            CurrentUserService.LibraryName = string.Empty;

            LoginWindow loginWindow = new();

            loginWindow.Show();

            Window.GetWindow(this)?.Close();
        }
    }
}