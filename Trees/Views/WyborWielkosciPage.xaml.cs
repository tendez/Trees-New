using Trees.Models;
using Trees.Services;
using Microsoft.Maui.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Trees.Views
{
    public partial class WyborWielkosciPage : ContentPage
    {
        private readonly DatabaseService _databaseService;
        private readonly Gatunek _selectedGatunek;
        private readonly Stoisko _stoisko;

        public WyborWielkosciPage(Gatunek selectedGatunek, Stoisko stoisko)
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
            _selectedGatunek = selectedGatunek;
            _stoisko = stoisko;
            LoadWielkosci();
        }

        private async void LoadWielkosci()
        {
            // Pobierz wszystkie wielkosci z bazy danych
            var wielkosci = await _databaseService.GetWielkosciAsync();
            // Przefiltruj wielkosci na podstawie wybranego gatunku
            WielkoscCollectionView.ItemsSource = FilterWielkosci(wielkosci);
        }

       
        private IEnumerable<Wielkosc> FilterWielkosci(IEnumerable<Wielkosc> wielkosci)
        {
            // Filtracja w zaleznosci od wybranego gatunku, tylko dla okreslonych kolorow
            if (_selectedGatunek.NazwaGatunku == "Swierk Pospolity Donica")
            {
                return wielkosci.Where(w => w.OpisWielkosci == "70-90cm" || w.OpisWielkosci == "90-120cm" || w.OpisWielkosci == "130-160cm" || w.OpisWielkosci == "170-180cm" || w.OpisWielkosci == "190-220cm");
            }
            else if (_selectedGatunek.NazwaGatunku == "Swierk Srebrny Donica")
            {
                return wielkosci.Where(w => w.OpisWielkosci == "70-90cm" || w.OpisWielkosci == "90-120cm" || w.OpisWielkosci == "130-160cm");
            }
            else if (_selectedGatunek.NazwaGatunku == "Swierk Pospolity")
            {
                return wielkosci.Where(w => w.OpisWielkosci == "130-160cm" || w.OpisWielkosci == "170-210cm" || w.OpisWielkosci == "220-270cm");
            }
            else if (_selectedGatunek.NazwaGatunku == "Jodla")
            {
                return wielkosci.Where(w => w.OpisWielkosci == "120-150cm" || w.OpisWielkosci == "150-200cm" || w.OpisWielkosci == "200-250cm" || w.OpisWielkosci == "280-330cm" || w.OpisWielkosci == "340-440cm" || w.OpisWielkosci == "450-550cm");
            }
            else if (_selectedGatunek.NazwaGatunku == "Jodla Donica")
            {
                return wielkosci.Where(w => w.OpisWielkosci == "80-100cm" || w.OpisWielkosci == "100-130cm" || w.OpisWielkosci == "130-160cm");
            }
            else if (_selectedGatunek.NazwaGatunku == "Swierk Srebrny")
            {
                return wielkosci.Where(w => w.OpisWielkosci == "130-160cm" || w.OpisWielkosci == "170-210cm" || w.OpisWielkosci == "220-270cm");
            }
            else if (_selectedGatunek.NazwaGatunku == "Stojak Metalowy")
            {
                return wielkosci.Where(w => w.OpisWielkosci == "standard");
            }
            else if (_selectedGatunek.NazwaGatunku == "Stojak Plastikowy")
            {
                return wielkosci.Where(w => w.OpisWielkosci == "standard");
            }

            return wielkosci;
        }

        private void OnWielkoscSelected(object sender, SelectionChangedEventArgs e)
        {
            Wielkosc? selectedWielkosc = e.CurrentSelection.FirstOrDefault() as Wielkosc;
            if (selectedWielkosc != null)
            {
                DalejButton.IsEnabled = true;
            }
        }

        private async void OnDalejClicked(object sender, EventArgs e)
        {
            Wielkosc? selectedWielkosc = WielkoscCollectionView.SelectedItem as Wielkosc;
            if (selectedWielkosc != null)
            {
                await Navigation.PushAsync(new DodajCenePage(_selectedGatunek, selectedWielkosc, _stoisko));
            }
        }
    }
}
