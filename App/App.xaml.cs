namespace App;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
		// Force the application to use the light theme regardless of the OS theme.
		// This makes AppThemeBinding evaluate to the Light values and avoids white text on white backgrounds.
		this.UserAppTheme = AppTheme.Light;
	}
	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new AppShell());
	}
}