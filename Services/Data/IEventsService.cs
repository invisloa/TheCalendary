using Kalendarzyk.Models.EventModels;
using Kalendarzyk.Models.EventTypesModels;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Kalendarzyk.Services.Data
{
    public interface IEventsService
    {
        ObservableCollection<EventModel> AllEventsOC { get; }
        ObservableCollection<EventTypeModel> AllEventTypesOC { get; }
        ObservableCollection<EventGroupModel> AllEventGroupsOC { get; }
        Task InitializeDataAsync();
        Task DeleteEventGroupAsync(EventGroupModel groupToDelete);
        Task DeleteEvenTypeAsync(EventTypeModel groupToDelete);
        Task DeleteEvenAsync(EventModel groupToDelete);

    }
}
