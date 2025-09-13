using ClientApiClientes.Models;
using ClientApiClientes.Services;

namespace ClientApiClientes.Views;

[QueryProperty(nameof(ClienteId), "ClienteId")]
public partial class CreateCliente : ContentPage
{
    private readonly ClienteService _clienteService;
    private int _clienteId;
    private Cliente? _cliente;

    public int ClienteId
    {
        get => _clienteId;
        set
        {
            _clienteId = value;
            if (_clienteId > 0)
            {
                LoadCliente();
            }
        }
    }

    public CreateCliente(ClienteService clienteService)
    {
        InitializeComponent();
        _clienteService = clienteService;
        _cliente = new Cliente();
    }

    private async void LoadCliente()
    {
        _cliente = await _clienteService.GetClienteAsync(_clienteId);
        if (_cliente != null)
        {
            NomeEntry.Text = _cliente.Nome;
            CpfEntry.Text = _cliente.CPF;
            DataNascimentoPicker.Date = _cliente.DataDeNascimento;
            SexoPicker.SelectedItem = _cliente.Sexo;
            EnderecoEntry.Text = _cliente.Endereco;
            CepEntry.Text = _cliente.CEP;
            CidadeEntry.Text = _cliente.Cidade;
            FotoEntry.Text = _cliente.Foto;
        }
    }

    private async void SalvarButton_Clicked(object sender, System.EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NomeEntry.Text) || string.IsNullOrWhiteSpace(CpfEntry.Text))
        {
            await DisplayAlert("Campos Obrigatórios", "Por favor, preencha Nome e CPF.", "OK");
            return;
        }

        if (_cliente == null)
        {
            _cliente = new Cliente();
        }

        _cliente.Nome = NomeEntry.Text;
        _cliente.CPF = CpfEntry.Text;
        _cliente.DataDeNascimento = DataNascimentoPicker.Date;
        _cliente.Sexo = SexoPicker.SelectedItem as string;
        _cliente.Endereco = EnderecoEntry.Text;
        _cliente.CEP = CepEntry.Text;
        _cliente.Cidade = CidadeEntry.Text;
        _cliente.Foto = FotoEntry.Text;

        await _clienteService.SaveClienteAsync(_cliente);
        await Shell.Current.GoToAsync("..");
    }

    private async void CancelarButton_Clicked(object sender, System.EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}