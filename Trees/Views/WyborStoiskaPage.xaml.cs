using Trees.Models;
using Trees.Services;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Trees.Views
{
    public partial class WyborStoiskaPage : ContentPage
    {
        private readonly DatabaseService _databaseService;

        public event EventHandler<Stoisko> StoiskoSelected;

        public WyborStoiskaPage()
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
            LoadStoiska();
        }

        private async void LoadStoiska()
        {
            var stoiska = await _databaseService.GetStoiskaAsync();
            StoiskaCollectionView.ItemsSource = stoiska;
        }

        private async void OnStoiskoSelected(object sender, SelectionChangedEventArgs e)
        {
            Stoisko? selectedStoisko = e.CurrentSelection.FirstOrDefault() as Stoisko;
            if (selectedStoisko != null)
            {
                StoiskoSelected?.Invoke(this, selectedStoisko);
                await Navigation.PopAsync();
            }
        }
    }
}
