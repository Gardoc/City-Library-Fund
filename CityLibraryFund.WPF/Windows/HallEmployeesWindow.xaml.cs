using CityLibraryFund.Data.DTOs;
using CityLibraryFund.WPF.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace CityLibraryFund.WPF.Windows
{
    /// <summary>
    /// Логика взаимодействия для HallEmployeesWindow.xaml
    /// </summary>
    public partial class HallEmployeesWindow : Window
    {
        private readonly HallService _hallService = new();

        private readonly int _hallId;

        private readonly string _hallName;

        public HallEmployeesWindow(int hallId, string hallName)
        {
            InitializeComponent();

            _hallId = hallId;
            _hallName = hallName;

            Loaded += HallEmployeesWindow_Loaded;
        }

        private async void HallEmployeesWindow_Loaded(object sender, RoutedEventArgs e)
        {
            HallNameTextBlock.Text = _hallName;

            await LoadEmployeesAsync();
        }

        private async Task LoadEmployeesAsync()
        {
            List<HallEmployeeDto> employees =
                await _hallService.GetHallEmployeesAsync(_hallId);

            EmployeesDataGrid.ItemsSource = employees;
        }
    }
}