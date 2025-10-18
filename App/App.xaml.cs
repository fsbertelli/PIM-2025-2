namespace App;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
		// Force the application to use the dark theme regardless of the OS theme.
		// This makes AppThemeBinding evaluate to the Dark values and ensures a consistent dark UI.
		this.UserAppTheme = AppTheme.Dark;
	}
	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new AppShell());
	}
}