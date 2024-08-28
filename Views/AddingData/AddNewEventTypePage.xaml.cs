using Kalendarzyk.Helpers;
using Kalendarzyk.Models.EventModels;
using Kalendarzyk.ViewModels;
using Kalendarzyk.ViewModels.ModelsViewModels;
using System.Collections.ObjectModel;
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
    protected override void OnAppearing()
    {
        base.OnAppearing();
        vm.EventGroupsCCHelper.RefreshGroups();
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        vm.EventGroupsVisualsOC.Remove(vm.EventGroupsVisualsOC.Last());
    }
}