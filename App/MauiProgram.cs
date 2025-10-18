using App.Services;
using App.Views;
using Microsoft.Extensions.Logging;

namespace App;

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

#if DEBUG
		builder.Logging.AddDebug();
#endif

        // Services
        builder.Services.AddSingleton<UserService>();
        builder.Services.AddSingleton<SettingsService>();

        // Pages
        builder.Services.AddTransient<UserPage>();
        builder.Services.AddTransient<CreateUser>();
        builder.Services.AddTransient<SettingsPage>();
        builder.Services.AddTransient<LoginPage>();

        return builder.Build();
	}
}
