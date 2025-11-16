namespace App.Views;

public partial class HomePage : ContentPage
{
    public HomePage()
    {
        InitializeComponent();
    }

    private async void OnViewUsersClicked(object sender, EventArgs e)
    {
        try
        {
            await Shell.Current.GoToAsync(nameof(UserPage));
        }
        catch
        {
            try { await DisplayAlert("Navegação", "Não foi possível navegar para UserPage.", "OK"); } catch { }
        }
    }
    private async void OnRelatoriosClicked(object sender, EventArgs e)
    {
        // Exemplo: navegar para a página de relatórios
        await Navigation.PushAsync(new RelatoriosPage());
    }

}
