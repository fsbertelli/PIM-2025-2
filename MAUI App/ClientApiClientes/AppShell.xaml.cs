using ClientApiClientes.Views;

namespace ClientApiClientes;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(CreateCliente), typeof(CreateCliente));
		Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
	}
}
