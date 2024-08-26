using Kalendarzyk.Helpers;
using Kalendarzyk.Models.EventTypesModels;
using Kalendarzyk.ViewModels;
namespace Kalendarzyk.Views;

public partial class AddNewEventTypePage : ContentPage
{
    AddNewEventTypePageViewModel vm;
    public AddNewEventTypePage()
    {

        vm = new AddNewEventTypePageViewModel();
        BindingContext = vm;
        InitializeComponent();

    }
    public AddNewEventTypePage(EventTypeModel userEventTypeModel)   // edit mode
    {
        vm = new AddNewEventTypePageViewModel(userEventTypeModel);

        BindingContext = vm;
        InitializeComponent();

    }

}