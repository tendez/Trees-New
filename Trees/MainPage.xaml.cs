using Microsoft.Maui.Controls;
using Trees.Models;
using Trees.Services;
using Trees.Views;

namespace Trees
{
    public partial class MainPage : ContentPage
    {
        public Stoisko _selectedStoisko;
        private int _loggedInUserId;
        private readonly DatabaseService _databaseService;

        public MainPage()
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
            if (Preferences.ContainsKey("IsLoggedIn") && Preferences.Get("IsLoggedIn", false))
            {
                _loggedInUserId = Preferences.Get("UserID", -1);

                if (_loggedInUserId == -1)
                {
                    DisplayAlert("Błąd", "Nie można załadować danych użytkownika. Proszę spróbować ponownie.", "OK");
                    return;
                }

                CheckUserRoleAndRedirect(); // Sprawdzenie roli użytkownika

                if (Preferences.ContainsKey("SelectedStoiskoID"))
                {
                    int stoiskoId = Preferences.Get("SelectedStoiskoID", 0);
                    LoadSelectedStoisko(stoiskoId);
                }
                else
                {
                    ShowLoggedInUI();
                }
            }
            else
            {
                Navigation.PushAsync(new Views.LoginPage());
            }
        }

        private async void CheckUserRoleAndRedirect()
        {
            try
            {
                var users = await _databaseService.GetUzytkownicyAsync();
                var loggedInUser = users.FirstOrDefault(u => u.UserID == _loggedInUserId);

                if (loggedInUser != null && loggedInUser.Role == "admin")
                {
                    await Navigation.PushAsync(new Views.AdminView());
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd", $"Wystąpił problem podczas sprawdzania roli użytkownika: {ex.Message}", "OK");
            }
        }

        private async void LoadSelectedStoisko(int stoiskoId)
        {
            _selectedStoisko = await _databaseService.GetStoiskoByIdAsync(stoiskoId);
            if (_selectedStoisko != null)
            {
                NazwaStoiskaLabel.Text = _selectedStoisko.StoiskoNazwa;
                NazwaStoiskaLabel.IsVisible = true;
                ShowActionsUI();
            }
            else
            {
                ShowLoggedInUI();
            }
        }

        private void ShowLoggedInUI()
        {
            WybierzStoiskoButton.IsVisible = true;
            DodajSprzedazButton.IsVisible = false;
            ZobaczMojaSprzedazButton.IsVisible = false;
            ZobaczSprzedazStoiskaButton.IsVisible = false;

            LogoutButton.IsVisible = false;
        }

        private void ShowActionsUI()
        {
            DodajSprzedazButton.IsVisible = true;
            ZobaczMojaSprzedazButton.IsVisible = true;
            ZobaczSprzedazStoiskaButton.IsVisible = true;
            LogoutButton.IsVisible = true;
            WybierzStoiskoButton.IsVisible = true;
        }

        private async void OnWybierzStoiskoClicked(object sender, EventArgs e)
        {
            var stoiskoPage = new Views.WyborStoiskaPage();
            stoiskoPage.StoiskoSelected += OnStoiskoSelected;
            await Navigation.PushAsync(stoiskoPage);
        }

        private void OnStoiskoSelected(object sender, Stoisko selectedStoisko)
        {
            _selectedStoisko = selectedStoisko;
            NazwaStoiskaLabel.Text = _selectedStoisko.StoiskoNazwa;
            NazwaStoiskaLabel.IsVisible = true;


            Preferences.Set("SelectedStoiskoID", _selectedStoisko.StoiskoID);

            ShowActionsUI();
        }

        private async void OnDodajSprzedazClicked(object sender, EventArgs e)
        {
            if (_selectedStoisko == null)
            {
                await DisplayAlert("Błąd", "Proszę wybrać stoisko przed dodaniem sprzedaży.", "OK");
                return;
            }
            await Navigation.PushAsync(new Views.DodajSprzedazPage(_selectedStoisko));
        }

        private async void OnZobaczSprzedazClicked(object sender, EventArgs e)
        {
            if (_selectedStoisko == null)
            {
                await DisplayAlert("Błąd", "Proszę wybrać stoisko przed przeglądaniem sprzedaży.", "OK");
                return;
            }
            await Navigation.PushAsync(new Views.ZobaczSprzedazPage(_selectedStoisko));
        }
        private async void OnZobaczMojaSprzedazClicked(object sender, EventArgs e)
        {
            if (_selectedStoisko == null)
            {
                await DisplayAlert("Błąd", "Proszę wybrać stoisko przed przeglądaniem sprzedaży.", "OK");
                return;
            }
            await Navigation.PushAsync(new Views.MojaSprzedazPage(_selectedStoisko));
        }

        private async void OnZobaczSprzedazStoiskaClicked(object sender, EventArgs e)
        {
            if (_selectedStoisko == null)
            {
                await DisplayAlert("Błąd", "Proszę wybrać stoisko przed przeglądaniem sprzedaży.", "OK");
                return;
            }
            await Navigation.PushAsync(new Views.ZobaczSprzedazPage(_selectedStoisko));
        }


        private async void OnLogoutButtonClicked(object sender, EventArgs e)
        {
            Preferences.Set("IsLoggedIn", false);
            Preferences.Remove("SelectedStoiskoID");
            await Navigation.PushAsync(new Views.LoginPage());
        }
    }
}
