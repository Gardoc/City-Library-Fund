using CityLibraryFund.Data.DTOs;
using CityLibraryFund.WPF.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace CityLibraryFund.WPF.Windows
{
    /// <summary>
    /// Логика взаимодействия для EditionReadersWindow.xaml
    /// </summary>
    public partial class EditionReadersWindow : Window
    {
        private readonly EditionService _editionService = new();

        private readonly int _editionId;

        private readonly string _editionInfo;

        public EditionReadersWindow(int editionId, string editionInfo)
        {
            InitializeComponent();

            _editionId = editionId;
            _editionInfo = editionInfo;

            Loaded += EditionReadersWindow_Loaded;
        }

        private async void EditionReadersWindow_Loaded(object sender, RoutedEventArgs e)
        {
            EditionInfoTextBlock.Text = _editionInfo;

            await LoadReadersAsync();
        }

        private async Task LoadReadersAsync()
        {
            List<EditionReaderDto> readers =
                await _editionService.GetEditionReadersAsync(_editionId);

            ReadersDataGrid.ItemsSource = readers;
        }
    }
}