namespace StudentHelper.MAUI;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

	private async void OnNotesClicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("//notes");
	}

	private async void OnTasksClicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("//tasks");
	}

	private async void OnExamsClicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("//exams");
	}

	private async void OnCalendarClicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("//calendar");
	}

	private async void OnSettingsClicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("//settings");
	}
}