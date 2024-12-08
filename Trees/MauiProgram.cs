using Microsoft.Extensions.Logging;
using Trees.Models;  // Upewnij się, że masz odpowiednią przestrzeń nazw

namespace Trees
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Rejestracja klasy ConnectionString jako Singleton w DI
            builder.Services.AddSingleton<ConnectionString>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
