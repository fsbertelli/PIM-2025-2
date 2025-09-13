using Microsoft.Extensions.Logging;
using ClientApiClientes.Services;
using ClientApiClientes.Views;

namespace ClientApiClientes;

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
        builder.Services.AddSingleton<ClienteService>();
        builder.Services.AddSingleton<SettingsService>();

        // Pages
        builder.Services.AddTransient<ClientesPage>();
        builder.Services.AddTransient<CreateCliente>();
        builder.Services.AddTransient<SettingsPage>();

        return builder.Build();
	}
}
