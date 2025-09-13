using ClientApiClientes.Models;
using ClientApiClientes.Services;
using System.Collections.ObjectModel;

namespace ClientApiClientes.Views;

public partial class ClientesPage : ContentPage
{
    private readonly ClienteService _clienteService;

    public ClientesPage(ClienteService clienteService)
    {
        InitializeComponent();
        _clienteService = clienteService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadClientes();
    }

    private async Task LoadClientes()
    {
        var clientes = await _clienteService.GetClientesAsync();
        // Usando o BindableLayout
        BindableLayout.SetItemsSource(ClientesList, new ObservableCollection<Cliente>(clientes));
    }

    // Evento do Botão Flutuante
    private async void AdicionarButton_Clicked(object sender, System.EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(CreateCliente));
    }

    // Obtém o cliente do CommandParameter do botão
    private Cliente? GetClienteFromSender(object sender)
    {
        if (sender is Button button && button.CommandParameter is Cliente cliente)
        { 
            return cliente;
        }
        return null;
    }

    private async void EditButton_Clicked_Item(object sender, System.EventArgs e)
    {
        var cliente = GetClienteFromSender(sender);
        if (cliente != null)
        {
            var navigationParameters = new Dictionary<string, object>
            {
                { "ClienteId", cliente.Id }
            };
            await Shell.Current.GoToAsync(nameof(CreateCliente), navigationParameters);
        }
    }

    private async void DeleteButton_Clicked_Item(object sender, System.EventArgs e)
    {
        var cliente = GetClienteFromSender(sender);
        if (cliente != null)
        {
            bool confirm = await DisplayAlert("Confirmar Exclusão", $"Você tem certeza que deseja excluir o cliente '{cliente.Nome}'?", "Sim", "Não");
            if (confirm)
            {
                await _clienteService.DeleteClienteAsync(cliente.Id);
                await LoadClientes();
            }
        }
    }

    private async void SettingsButton_Clicked(object sender, System.EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(SettingsPage));
    }
}