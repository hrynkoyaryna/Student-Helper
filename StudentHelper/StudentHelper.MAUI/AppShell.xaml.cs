using StudentHelper.MAUI.Services;

namespace StudentHelper.MAUI;

public partial class AppShell : Shell
{
	private readonly IUserContext _userContext;

	public AppShell(IUserContext userContext)
	{
		_userContext = userContext;
		InitializeComponent();

		UpdateFlyoutBehavior();
	}

	private void UpdateFlyoutBehavior()
	{
		if (_userContext.IsAuthenticated)
		{
			FlyoutBehavior = FlyoutBehavior.Flyout;
			CurrentItem = FindByName("main") as FlyoutItem;
		}
		else
		{
			FlyoutBehavior = FlyoutBehavior.Disabled;
			CurrentItem = FindByName("auth") as TabBar;
		}
	}
}