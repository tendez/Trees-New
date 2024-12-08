using Microsoft.Maui.Controls;
using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;
using Trees.Models;


namespace Trees.Views
{
    public partial class LoginPage : ContentPage
    {
        private readonly string _connectionString = "Data Source=christmastreessofijowka.database.windows.net;Initial Catalog=Trees;User ID=mikolaj;Password=Qwerty123!;Connect Timeout=30;Encrypt=True;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
        private int _loggedInUser;

        public LoginPage()
        {
            InitializeComponent();


            if (Preferences.ContainsKey("IsLoggedIn") && Preferences.Get("IsLoggedIn", false))
            {

                Navigation.PushAsync(new MainPage());
            }
        }

        private async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            string username = UsernameEntry.Text;
            string password = PasswordEntry.Text;

            if (await ValidateUserAsync(username, password))
            {
                Preferences.Set("IsLoggedIn", true);
                await Navigation.PushAsync(new MainPage());
            }
            else
            {
                LoginStatusLabel.Text = "Nieprawidlowa nazwa uzytkownika lub haslo.";
            }
        }

        private async Task<bool> ValidateUserAsync(string username, string password)
        {
            bool isValid = false;
            string hashedPassword = HashPassword(password);

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT COUNT(1) FROM Uzytkownicy WHERE Login=@username AND Password=@password";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", hashedPassword);

                try
                {
                    await connection.OpenAsync();

                    int count = (int)await command.ExecuteScalarAsync();
                    isValid = count > 0;

                    if (isValid)
                    {
                        string query2 = "SELECT UserID FROM Uzytkownicy WHERE Login=@username";
                        SqlCommand command2 = new SqlCommand(query2, connection);
                        command2.Parameters.AddWithValue("@username", username);

                        _loggedInUser = (int)await command2.ExecuteScalarAsync();
                        Preferences.Set("UserID", _loggedInUser);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Blad podczas walidacji uzytkownika: {ex.Message}");
                }
            }

            return isValid;
        }

        private string HashPassword(string password)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            byte[] bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            var builder = new System.Text.StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }


        private void OnShowPasswordCheckBoxChanged(object sender, CheckedChangedEventArgs e)
        {
            PasswordEntry.IsPassword = !e.Value;
        }


        public async Task LogoutAsync()
        {
            Preferences.Remove("IsLoggedIn");
            Preferences.Remove("UserID");

            await Navigation.PushAsync(new LoginPage());
        }
    }
}
