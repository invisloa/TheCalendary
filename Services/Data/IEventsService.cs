using Kalendarzyk.Models.EventModels;
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

        Task<OperationResult> AddEventAsync(EventModel eventToAdd);
        Task<OperationResult> UpdateEventAsync(EventModel eventToUpdate);
        Task<OperationResult> DeleteEventAsync(EventModel eventToDelete);

        Task<OperationResult> AddEventTypeAsync(EventTypeModel eventTypeToAdd);
        Task<OperationResult> UpdateEventTypeAsync(EventTypeModel eventTypeToUpdate);
        Task<OperationResult> DeleteEventTypeAsync(EventTypeModel eventTypeToDelete);

        Task<OperationResult> AddEventGroupAsync(EventGroupModel eventGroupToAdd);
        Task<OperationResult> UpdateEventGroupAsync(EventGroupModel eventGroupToUpdate);
        Task<OperationResult> DeleteEventGroupAsync(EventGroupModel groupToDelete);
    }
}
