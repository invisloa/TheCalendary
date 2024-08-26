using CommunityToolkit.Mvvm.Input;
using Kalendarzyk.Models.EventTypesModels;
using Kalendarzyk.ViewModels.ModelsViewModels;
using System.Collections.ObjectModel;

namespace Kalendarzyk.Views.CustomControls.CCInterfaces
{

    /// <summary>
    /// When using this interface consider using EventGroupsCCHelper class
    /// EventGroupsCCHelper implements this interface and helps to set the logic for control operations 
    /// </summary>
    public interface IEventGroupsCCViewModel
	{
		public EventGroupModel SelectedEventGroup { get; set; }
		ObservableCollection<EventGroupViewModel> EventGroupsVisualsOC { get; set; }
		RelayCommand<EventGroupViewModel> EventGroupSelectedCommand { get; }
		public event Action<EventGroupModel> EventGroupChanged;
	}
}
