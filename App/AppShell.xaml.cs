using App.Views;

namespace App;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(CreateUser), typeof(CreateUser));
		Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
	}
}
