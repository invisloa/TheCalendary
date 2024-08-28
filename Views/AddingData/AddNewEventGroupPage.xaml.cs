using Kalendarzyk.ViewModels.Event;

namespace Kalendarzyk.Views;

public partial class AddNewEventGroupPage : ContentPage
{
	public AddNewEventGroupPage()
	{
		AddNewEventGroupPageViewModel viewModel = new AddNewEventGroupPageViewModel();
		InitializeComponent();
		BindingContext = viewModel;
	}
}