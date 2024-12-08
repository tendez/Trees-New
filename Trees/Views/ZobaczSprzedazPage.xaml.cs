using Microsoft.Maui.Controls;
using Trees.Models;
using Trees.Services;
using System;
using System.Linq;

namespace Trees.Views
{
    public partial class ZobaczSprzedazPage : ContentPage
    {
        private readonly DatabaseService _databaseService;
        private readonly Stoisko _stoisko;
        private float totalSprzedaz;
        public ZobaczSprzedazPage(Stoisko stoisko)
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
            _stoisko = stoisko;
            LoadTotalSprzedaz(stoisko, DateTime.Now);
            LoadSprzedaz(stoisko, DateTime.Now);
        }


        async void LoadTotalSprzedaz(Stoisko stoisko, DateTime date)
        {
            try
            {
                totalSprzedaz = await _databaseService.GetTotalSprzedazByStoiskoAndDateAsync(stoisko.StoiskoID, date);
                TotalSprzedazLabel.Text = $"Suma sprzedazy: {totalSprzedaz} zl";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystapil blad podczas ladowania danych: {ex.Message}");
                await DisplayAlert("Blad", "Wystapil blad podczas ladowania danych. Sprobuj ponownie pozniej.", "OK");
            }
        }

        async void LoadSprzedaz(Stoisko stoisko, DateTime date)
        {
            try
            {
                var sprzedazList = await _databaseService.GetSprzedazWithDetailsAndDateAsync(stoisko, date);
                SprzedazCollectionView.ItemsSource = sprzedazList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystapil blad podczas ladowania danych: {ex.Message}");
                await DisplayAlert("Blad", "Wystapil blad podczas ladowania danych. Sprobuj ponownie pozniej.", "OK");
            }
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadSprzedaz(_stoisko, DateTime.Now);
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
                    LoadSprzedaz(_stoisko, DateTime.Now);
                }
            }
        }
        private void OnDateSelected(object sender, DateChangedEventArgs e)
        {
            LoadSprzedaz(_stoisko, e.NewDate);
            LoadTotalSprzedaz(_stoisko, e.NewDate);
        }


    }
}
