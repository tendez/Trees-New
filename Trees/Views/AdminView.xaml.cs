using Microsoft.Maui.Controls;
using Trees.Models;
namespace Trees.Views
{
    public partial class AdminView : ContentPage
    {
        public AdminView()
        {
            InitializeComponent();
        }

        private async void OnZwrotyClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Views.ReturnsPage()); // Strona dla "Zwroty"
        }

        private async void OnStatystykiClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Views.StatisticsPage()); // Strona dla "Statystyki"
        }

        private async void OnMagazynClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Views.WarehousePage()); // Strona dla "Magazyn"
        }
    }
}
