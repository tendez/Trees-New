using Microsoft.Maui.Controls;
using Trees.Models;
using Trees.Services;

namespace Trees.Views
{
    public partial class EdytujSprzedazPage : ContentPage
    {
        private readonly DatabaseService _databaseService;
        private readonly Sprzedaz _sprzedaz;

        public EdytujSprzedazPage(Sprzedaz sprzedaz)
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
            _sprzedaz = sprzedaz;
            BindingContext = _sprzedaz;
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(CenaEntry.Text))
                {
                    await DisplayAlert("Blad", "Cena  nie moze byc pusta.", "OK");
                    return;
                }


                if (!decimal.TryParse(CenaEntry.Text, out decimal nowaCena))
                {
                    await DisplayAlert("Blad", "Wprowadzone wartosci musza byc liczbami.", "OK");
                    return;
                }


                if (nowaCena <= 0)
                {
                    await DisplayAlert("Blad", "Cena musi byc wieksza od zera.", "OK");
                    return;
                }



                _sprzedaz.Cena = nowaCena;



                await _databaseService.UpdateSprzedazAsync(_sprzedaz);

                await DisplayAlert("Zapisano", "Zmiany zostaly zapisane.", "OK");
                await Navigation.PopAsync();
            }
            catch (InvalidOperationException ex)
            {

                await DisplayAlert("Blad", ex.Message, "OK");
            }
            catch (Exception ex)
            {

                await DisplayAlert("Blad", $"Wystapil blad: {ex.Message}", "OK");
            }
        }



    }
}
