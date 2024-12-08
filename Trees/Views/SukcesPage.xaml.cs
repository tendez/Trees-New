using Microsoft.Maui.Controls;
using Trees.Models;

namespace Trees.Views
{
    public partial class SukcesPage : ContentPage
    {
        private readonly Stoisko _stoisko;
        public SukcesPage(Stoisko stoisko)
        {
            _stoisko = stoisko;
            InitializeComponent();
        }

        private async void OnDodajKolejnaSprzedazClicked(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }

        private async void OnZobaczSprzedazClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ZobaczSprzedazPage(_stoisko));
        }
    }
}
