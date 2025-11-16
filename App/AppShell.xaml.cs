using App.Views;

namespace App;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(CreateUser), typeof(CreateUser));
		Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
		Routing.RegisterRoute("relatorio-usuarios", typeof(RelatorioUsuariosPage));
		Routing.RegisterRoute("relatorio-atividades", typeof(RelatorioAtividadesPage));


		Items.Add(new MenuItem
		{
			Text = "Relatórios",
			Command = new Command(async () => await Shell.Current.GoToAsync($"//{nameof(RelatoriosPage)}"))
		});
	}
}
