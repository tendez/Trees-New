using Microsoft.Maui.Controls;
using Trees.Models;
using Trees.Services;
using System;
using System.Linq;

namespace Trees.Views
{
    public partial class MojaSprzedazPage : ContentPage
    {
        private readonly DatabaseService _databaseService;
        private readonly Stoisko _stoisko;
        private float totalSprzedaz;
        private int _loggedInUserId;

        public MojaSprzedazPage(Stoisko stoisko)
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
            _stoisko = stoisko;

            // Pobierz ID zalogowanego u�ytkownika
            _loggedInUserId = Preferences.Get("UserID", -1);

            if (_loggedInUserId == -1)
            {
                DisplayAlert("Blad", "Nie mozna zaladowac danych uzytkownika. Prosze sprobowac ponownie.", "OK");
                return;
            }

            LoadTotalSprzedaz(_stoisko, _loggedInUserId, DateTime.Now);
            LoadSprzedaz(_stoisko, _loggedInUserId, DateTime.Now);
        }

        async void LoadTotalSprzedaz(Stoisko stoisko, int userId, DateTime date)
        {
            try
            {
                float totalSprzedaz = await _databaseService.GetTotalSprzedazByStoiskoAndUserAndDateAsync(stoisko.StoiskoID, userId, date);
                TotalSprzedazLabel.Text = $"Suma sprzedazy: {totalSprzedaz} zl";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystapil blad podczas ladowania danych: {ex.Message}");
                await DisplayAlert("Blad", "Wystapil blad podczas ladowania danych. Sprobuj ponownie poniej.", "OK");
            }
        }

        async void LoadSprzedaz(Stoisko stoisko, int userId, DateTime date)
        {
            try
            {
                var sprzedazList = await _databaseService.GetSprzedazWithDetailsAndUserAndDateAsync(stoisko, userId, date);
                SprzedazCollectionView.ItemsSource = sprzedazList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystapil blad podczas ladowania danych: {ex.Message}");
                await DisplayAlert("Blad", "Wystapil blad podczas ladowania danych. Sprobuj ponownie poniej.", "OK");
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadSprzedaz(_stoisko, _loggedInUserId, DateTime.Now);
        }

        private async void OnEditClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            Sprzedaz? sprzedaz = button?.CommandParameter as Sprzedaz;

            if (sprzedaz != null)
            {
                await Navigation.PushAsync(new EdytujSprzedazPage(sprzedaz));
            }
        }

        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            Sprzedaz? sprzedaz = button?.CommandParameter as Sprzedaz;

            if (sprzedaz != null)
            {
                var confirm = await DisplayAlert("Usun", "Czy na pewno chcesz usunac ten wpis?", "Tak", "Nie");
                if (confirm)
                {
                    await _databaseService.DeleteSprzedazAsync(sprzedaz.SprzedazID);
                    await DisplayAlert("Usunieto", "Wpis zostal usuniety.", "OK");
                    LoadSprzedaz(_stoisko, _loggedInUserId, DateTime.Now);
                }
            }
        }
        private void OnDateSelected(object sender, DateChangedEventArgs e)
        {
            LoadSprzedaz(_stoisko, _loggedInUserId, e.NewDate);
            LoadTotalSprzedaz(_stoisko, _loggedInUserId, e.NewDate);
        }
    }
}
