using CityLibraryFund.Data.DTOs;
using CityLibraryFund.WPF.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace CityLibraryFund.WPF.Windows
{
    /// <summary>
    /// Логика взаимодействия для AuthorInventoryWindow.xaml
    /// </summary>
    public partial class AuthorInventoryWindow : Window
    {
        private readonly AuthorService _authorService = new();

        private readonly int _authorId;

        private readonly string _authorName;

        public AuthorInventoryWindow(int authorId, string authorName)
        {
            InitializeComponent();

            _authorId = authorId;
            _authorName = authorName;

            Loaded += AuthorInventoryWindow_Loaded;
        }

        private async void AuthorInventoryWindow_Loaded(object sender, RoutedEventArgs e)
        {
            AuthorNameTextBlock.Text = _authorName;

            await LoadInventoryAsync();
        }

        private async Task LoadInventoryAsync()
        {
            List<AuthorInventoryDto> inventory =
                await _authorService.GetAuthorInventoryAsync(_authorId);

            InventoryDataGrid.ItemsSource = inventory;
        }
    }
}