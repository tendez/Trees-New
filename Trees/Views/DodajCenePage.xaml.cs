using Trees.Models;
using Trees.Services;
using Microsoft.Maui.Controls;
using System;

namespace Trees.Views
{
    public partial class DodajCenePage : ContentPage
    {
        private readonly DatabaseService _databaseService;
        private readonly Gatunek _selectedGatunek;
        private readonly Wielkosc _selectedWielkosc;
        private readonly Stoisko _stoisko;

        public DodajCenePage(Gatunek selectedGatunek, Wielkosc selectedWielkosc, Stoisko stoisko)
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
            _selectedGatunek = selectedGatunek;
            _selectedWielkosc = selectedWielkosc;
            _stoisko = stoisko;
            Loaded += Page_Loaded;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

        }
        private void Page_Loaded(object sender, EventArgs e)
        {
            CenaEntry.Focus(); // Ustawienie automatycznego fokusu na pole CenaEntry
        }

        private async void OnDodajSprzedazClicked(object sender, EventArgs e)
        {
            warning.IsVisible = false;

            if (!decimal.TryParse(CenaEntry.Text, out decimal cena))
            {
                warning.Text = "Wprowadz poprawna liczbe w polu Cena.";
                warning.IsVisible = true;
                return;
            }

            if (cena <= 0)
            {
                warning.Text = "Cena musi byc wieksza niz 0.";
                warning.IsVisible = true;
                return;
            }

            var magazyn = await _databaseService.GetMagazynAsync(_selectedGatunek.GatunekID, _selectedWielkosc.WielkoscID, _stoisko.StoiskoID);
            if (magazyn == null || magazyn.Ilosc < 1)
            {
                warning.Text = "Nie ma wystarczajacej ilosci towaru w magazynie.";
                warning.IsVisible = true;
                return;
            }

            magazyn.Ilosc -= 1;
            await _databaseService.UpdateMagazynAsync(magazyn);

            var sprzedaz = new Sprzedaz
            {
                UserID = Preferences.Get("UserID", 0),
                GatunekID = _selectedGatunek.GatunekID,
                WielkoscID = _selectedWielkosc.WielkoscID,
                Cena = cena,
                DataSprzedazy = DateTime.Now,
                StoiskoID = _stoisko.StoiskoID
            };

            await _databaseService.AddSprzedazAsync(sprzedaz);

            await Navigation.PushAsync(new SukcesPage(_stoisko));
        }
    }
}
