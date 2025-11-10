using App.Models;
using System;
using System.Text.Json;

namespace App.Services
{
    public class UsuarioSessaoService
    {
        private const string ChaveUsuario = "usuario_logado";
        private UsuarioLogado _usuarioAtual;

        public event Action OnChange;

        public UsuarioLogado UsuarioAtual
        {
            get => _usuarioAtual;
            private set
            {
                _usuarioAtual = value;
                OnChange?.Invoke();
            }
        }

        public bool EstaLogado => UsuarioAtual?.EstaAutenticado ?? false;

        // Fazer login
        public void FazerLogin(UsuarioLogado usuario)
        {
            usuario.EstaAutenticado = true;
            usuario.DataLogin = DateTime.Now;
            UsuarioAtual = usuario;

            var json = JsonSerializer.Serialize(usuario);
            Preferences.Set(ChaveUsuario, json);
        }

        // Fazer logout
        public void FazerLogout()
        {
            Preferences.Remove(ChaveUsuario);
            UsuarioAtual = null;
        }

        // Tentar restaurar sessão
        public void CarregarSessao()
        {
            var json = Preferences.Get(ChaveUsuario, null);
            if (json != null)
            {
                UsuarioAtual = JsonSerializer.Deserialize<UsuarioLogado>(json);
            }
        }
    }
}
