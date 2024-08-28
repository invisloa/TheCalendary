using AsyncAwaitBestPractices;
using Kalendarzyk.Models.EventModels;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Kalendarzyk.Services.Data
{
    public class EventsService : IEventsService
    {
        public ObservableCollection<EventModel> AllEventsOC { get; private set; }
        public ObservableCollection<EventTypeModel> AllEventTypesOC { get; private set; }
        public ObservableCollection<EventGroupModel> AllEventGroupsOC { get; private set; }

        private readonly IEventRepository _eventRepository;

        public EventsService(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public async Task InitializeDataAsync()
        {
            var events = await _eventRepository.GetEventsListAsync().ConfigureAwait(false);
            AllEventsOC = new ObservableCollection<EventModel>(events.ToList());

            var eventTypes = await _eventRepository.GetEventTypesListAsync().ConfigureAwait(false);
            AllEventTypesOC = new ObservableCollection<EventTypeModel>(eventTypes.ToList());

            var eventGroups = await _eventRepository.GetEventGroupsListAsync().ConfigureAwait(false);
            AllEventGroupsOC = new ObservableCollection<EventGroupModel>(eventGroups.ToList());
        }

        public async Task<OperationResult> AddEventAsync(EventModel eventToAdd)
        {
            var result = await _eventRepository.AddEventAsync(eventToAdd);
            if (result.IsSuccess)
            {
                AllEventsOC.Add(eventToAdd);
            }
            return result;
        }

        public async Task<OperationResult> UpdateEventAsync(EventModel eventToUpdate)
        {
            return await _eventRepository.UpdateEventAsync(eventToUpdate);
        }

        public async Task<OperationResult> DeleteEventAsync(EventModel eventToDelete)
        {
            var result = await _eventRepository.DeleteEventAsync(eventToDelete);
            if (result.IsSuccess)
            {
                AllEventsOC.Remove(eventToDelete);
            }
            return result;
        }

        public async Task<OperationResult> AddEventTypeAsync(EventTypeModel eventTypeToAdd)
        {
            var result = await _eventRepository.AddEventTypeAsync(eventTypeToAdd);
            if (result.IsSuccess)
            {
                AllEventTypesOC.Add(eventTypeToAdd);
            }
            return result;
        }

        public async Task<OperationResult> UpdateEventTypeAsync(EventTypeModel eventTypeToUpdate)
        {
            return await _eventRepository.UpdateEventTypeAsync(eventTypeToUpdate);
        }

        public async Task<OperationResult> DeleteEventTypeAsync(EventTypeModel eventTypeToDelete)
        {
            var eventsToDelete = AllEventsOC
                .Where(e => e.EventType.Equals(eventTypeToDelete))
                .ToList();

            foreach (var eventModel in eventsToDelete)
            {
                var deleteEventResult = await _eventRepository.DeleteEventAsync(eventModel);
                if (!deleteEventResult.IsSuccess)
                {
                    return OperationResult.Failure($"Failed to delete event: {deleteEventResult.ErrorMessage}");
                }
                AllEventsOC.Remove(eventModel);
            }

            var deleteEventTypeResult = await _eventRepository.DeleteEventTypeAsync(eventTypeToDelete);
            if (deleteEventTypeResult.IsSuccess)
            {
                AllEventTypesOC.Remove(eventTypeToDelete);
            }

            return deleteEventTypeResult;
        }

        public async Task<OperationResult> AddEventGroupAsync(EventGroupModel eventGroupToAdd)
        {
            var result = await _eventRepository.AddEventGroupAsync(eventGroupToAdd);
            if (result.IsSuccess)
            {
                AllEventGroupsOC.Add(eventGroupToAdd);
            }
            return result;
        }

        public async Task<OperationResult> UpdateEventGroupAsync(EventGroupModel eventGroupToUpdate)
        {
            return await _eventRepository.UpdateEventGroupAsync(eventGroupToUpdate);
        }

        public async Task<OperationResult> DeleteEventGroupAsync(EventGroupModel groupToDelete)
        {
            var eventsToDelete = AllEventsOC
                .Where(e => e.EventType.EventGroup.Equals(groupToDelete))
                .ToList();

            foreach (var eventModel in eventsToDelete)
            {
                var deleteEventResult = await _eventRepository.DeleteEventAsync(eventModel);
                if (!deleteEventResult.IsSuccess)
                {
                    return OperationResult.Failure($"Failed to delete event: {deleteEventResult.ErrorMessage}");
                }
                AllEventsOC.Remove(eventModel);
            }

            var eventTypesToDelete = AllEventTypesOC
                .Where(et => et.EventGroup.Equals(groupToDelete))
                .ToList();

            foreach (var eventType in eventTypesToDelete)
            {
                var deleteEventTypeResult = await _eventRepository.DeleteEventTypeAsync(eventType);
                if (!deleteEventTypeResult.IsSuccess)
                {
                    return OperationResult.Failure($"Failed to delete event type: {deleteEventTypeResult.ErrorMessage}");
                }
                AllEventTypesOC.Remove(eventType);
            }

            var deleteGroupResult = await _eventRepository.DeleteEventGroupAsync(groupToDelete);
            if (deleteGroupResult.IsSuccess)
            {
                AllEventGroupsOC.Remove(groupToDelete);
            }

            return deleteGroupResult;
        }
    }
}
