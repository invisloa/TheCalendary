using Kalendarzyk.ViewModels.ModelsViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Kalendarzyk.ViewModels.CCViewModels
{
    public interface IEventGroupsSelectorCCViewModel
    {
        ObservableCollection<EventGroupViewModel> AllEventGroupsOC { get; set; }
        EventGroupViewModel SelectedEventGroup { get; set; }
        ICommand SelectEventGroupCommand { get; set; }
    }
}