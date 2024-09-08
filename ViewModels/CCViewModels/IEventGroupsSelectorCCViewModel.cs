using Kalendarzyk.Models.EventModels;
using Kalendarzyk.ViewModels.ModelsViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Kalendarzyk.ViewModels.CCViewModels
{
    public interface IEventGroupsCCViewModel
    {
        public EventGroupModel SelectedEventGroup { get; set; }
        ObservableCollection<EventGroupViewModel> EventGroupsVisualsOC { get; set; }
        RelayCommand<EventGroupViewModel> EventGroupSelectedCommand { get; set; }
        public event Action<EventGroupModel> EventGroupChanged;
    }
}