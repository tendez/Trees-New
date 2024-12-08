using Trees.Models;
using Trees.Services;
using Microsoft.Maui.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Trees.Views
{
    public partial class DodajSprzedazPage : ContentPage
    {
        private readonly DatabaseService _databaseService;
        private Gatunek _selectedGatunek;
        private readonly Stoisko _stoisko;

        public DodajSprzedazPage(Stoisko stoisko)
        {
            InitializeComponent();


            _databaseService = new DatabaseService();

            _stoisko = stoisko;
            LoadGatunki();
        }

        private async void LoadGatunki()
        {

            var gatunki = await _databaseService.GetGatunkiAsync();
            GatunekCollectionView.ItemsSource = gatunki;
        }

        private void OnGatunekSelected(object sender, SelectionChangedEventArgs e)
        {
            _selectedGatunek = e.CurrentSelection.FirstOrDefault() as Gatunek;
            if (_selectedGatunek != null)
            {

                DalejButton.IsEnabled = true;
            }
        }

        private async void OnDalejClicked(object sender, EventArgs e)
        {

            if (_selectedGatunek != null)
            {
                await Navigation.PushAsync(new WyborWielkosciPage(_selectedGatunek, _stoisko));
            }
        }
    }
}
