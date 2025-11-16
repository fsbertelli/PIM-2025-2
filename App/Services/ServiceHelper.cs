using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace App;

public static class ServiceHelper
{
    public static T GetService<T>() where T : class
        => Current.GetService<T>() 
           ?? throw new InvalidOperationException($"Serviço não registrado: {typeof(T)}");

    public static IServiceProvider Current =>
#if WINDOWS
        MauiWinUIApplication.Current.Services;
#else
        Application.Current?.Handler?.MauiContext?.Services 
            ?? throw new InvalidOperationException("ServiceProvider indisponível");
#endif
}