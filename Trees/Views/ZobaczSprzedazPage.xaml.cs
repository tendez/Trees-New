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
     
        private void OnDateSelected(object sender, DateChangedEventArgs e)
        {
            LoadSprzedaz(_stoisko, e.NewDate);
            LoadTotalSprzedaz(_stoisko, e.NewDate);
        }


    }
}
