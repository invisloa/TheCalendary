using Kalendarzyk.ViewModels;

namespace Kalendarzyk.Views;

public partial class AddNewEventGroupPage : ContentPage
{
	public AddNewEventGroupPage()
	{
		AddNewEventGroupViewModel viewModel = new AddNewEventGroupViewModel();
		InitializeComponent();
		BindingContext = viewModel;
	}
}