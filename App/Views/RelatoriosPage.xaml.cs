using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace App.Views
{
    public partial class RelatoriosPage : ContentPage
    {
        public RelatoriosPage()
        {
            InitializeComponent();
        }

        private async void OnRelatorioUsuariosClicked(object sender, EventArgs e)
            => await Shell.Current.GoToAsync("relatorio-usuarios");

        private async void OnRelatorioAtividadesClicked(object sender, EventArgs e)
            => await Shell.Current.GoToAsync("relatorio-atividades");

        private async void OnRelatorioPersonalizadoClicked(object sender, EventArgs e)
        {
            // Exemplo de fluxo com período
            var escolha = await DisplayActionSheet(
                "Selecionar período",
                "Cancelar", null,
                "Últimos 7 dias", "Últimos 30 dias", "Customizar…");

            if (escolha == "Customizar…")
            {
                // Aqui você pode abrir uma página de filtros
                // await Navigation.PushAsync(new FiltrosRelatorioPage());
                await DisplayAlert("Relatórios", "Abrir filtros personalizados…", "OK");
            }
        }
    }
}
