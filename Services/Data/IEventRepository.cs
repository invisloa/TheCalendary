using Kalendarzyk.Models.EventModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kalendarzyk.Services.Data
{
    public interface IEventRepository
    {
        // Event methods
        Task<OperationResult> AddEventAsync(EventModel eventToAdd);
        Task<IEnumerable<EventModel>> GetEventsListAsync();
        Task<EventModel> GetEventByIdAsync(int eventId);
        Task<OperationResult> UpdateEventAsync(EventModel eventToUpdate);
        Task<OperationResult> DeleteEventAsync(EventModel eventToDelete);
        Task<OperationResult> ClearAllEventsAsync();

        // Event group methods
        Task<OperationResult> AddEventGroupAsync(EventGroupModel eventGroupToAdd);
        Task<IEnumerable<EventGroupModel>> GetEventGroupsListAsync();
        Task<OperationResult> UpdateEventGroupAsync(EventGroupModel eventGroupToUpdate);
        Task<OperationResult> DeleteEventGroupAsync(EventGroupModel eventGroupToDelete);
        Task<OperationResult> ClearAllEventGroupsAsync();

        // Event type methods
        Task<OperationResult> AddEventTypeAsync(EventTypeModel eventTypeToAdd);
        Task<IEnumerable<EventTypeModel>> GetEventTypesListAsync();
        Task<OperationResult> UpdateEventTypeAsync(EventTypeModel eventTypeToUpdate);
        Task<OperationResult> DeleteEventTypeAsync(EventTypeModel eventTypeToDelete);
        Task<OperationResult> ClearAllEventTypesAsync();


        // File Operations
        Task<OperationResult> SaveEventsAndTypesToFile(IEnumerable<EventModel> eventsToSave = null);
        Task<OperationResult> LoadEventsAndTypesFromFile();
        Task<OperationResult> ResetDatabaseAsync();

    }
}
