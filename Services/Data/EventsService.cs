using AsyncAwaitBestPractices;
using Kalendarzyk.Mediator;
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
        private readonly IMediator _mediator;

        public EventsService(IEventRepository eventRepository, IMediator mediator)
        {
            _eventRepository = eventRepository;
            _mediator = mediator;
        }

        public async Task InitializeDataAsync()
        {
            var events = await _eventRepository.GetEventsListAsync().ConfigureAwait(false);
            AllEventsOC = new ObservableCollection<EventModel>(events.ToList());
            _mediator.Notify("EventsLoaded", this, AllEventsOC);

            var eventTypes = await _eventRepository.GetEventTypesListAsync().ConfigureAwait(false);
            AllEventTypesOC = new ObservableCollection<EventTypeModel>(eventTypes.ToList());
            _mediator.Notify("EventTypesLoaded", this, AllEventTypesOC);

            var eventGroups = await _eventRepository.GetEventGroupsListAsync().ConfigureAwait(false);
            AllEventGroupsOC = new ObservableCollection<EventGroupModel>(eventGroups.ToList());
            _mediator.Notify("EventGroupsLoaded", this, AllEventGroupsOC);
        }

        public async Task<OperationResult> AddEventAsync(EventModel eventToAdd)
        {
            var result = await _eventRepository.AddEventAsync(eventToAdd);
            if (result.IsSuccess)
            {
                AllEventsOC.Add(eventToAdd);
                _mediator.Notify("EventAdded", this, eventToAdd);
            }
            return result;
        }

        public async Task<OperationResult> UpdateEventAsync(EventModel eventToUpdate)
        {
            var result = await _eventRepository.UpdateEventAsync(eventToUpdate);
            if (result.IsSuccess)
            {
                _mediator.Notify("EventUpdated", this, eventToUpdate);
            }
            return result;
        }

        public async Task<OperationResult> DeleteEventAsync(EventModel eventToDelete)
        {
            var result = await _eventRepository.DeleteEventAsync(eventToDelete);
            if (result.IsSuccess)
            {
                AllEventsOC.Remove(eventToDelete);
                _mediator.Notify("EventRemoved", this, eventToDelete);
            }
            return result;
        }

        public async Task<OperationResult> AddEventTypeAsync(EventTypeModel eventTypeToAdd)
        {
            var result = await _eventRepository.AddEventTypeAsync(eventTypeToAdd);
            if (result.IsSuccess)
            {
                AllEventTypesOC.Add(eventTypeToAdd);
                _mediator.Notify("EventTypeAdded", this, eventTypeToAdd);
            }
            return result;
        }

        public async Task<OperationResult> UpdateEventTypeAsync(EventTypeModel eventTypeToUpdate)
        {
            var result = await _eventRepository.UpdateEventTypeAsync(eventTypeToUpdate);
            if (result.IsSuccess)
            {
                _mediator.Notify("EventTypeUpdated", this, eventTypeToUpdate);
            }
            return result;
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
                _mediator.Notify("EventRemoved", this, eventModel);
            }

            var deleteEventTypeResult = await _eventRepository.DeleteEventTypeAsync(eventTypeToDelete);
            if (deleteEventTypeResult.IsSuccess)
            {
                AllEventTypesOC.Remove(eventTypeToDelete);
                _mediator.Notify("EventTypeRemoved", this, eventTypeToDelete);
            }

            return deleteEventTypeResult;
        }

        public async Task<OperationResult> AddEventGroupAsync(EventGroupModel eventGroupToAdd)
        {
            var result = await _eventRepository.AddEventGroupAsync(eventGroupToAdd);
            if (result.IsSuccess)
            {
                AllEventGroupsOC.Add(eventGroupToAdd);
                _mediator.Notify("EventGroupAdded", this, eventGroupToAdd);
            }
            return result;
        }

        public async Task<OperationResult> UpdateEventGroupAsync(EventGroupModel eventGroupToUpdate)
        {
            var result = await _eventRepository.UpdateEventGroupAsync(eventGroupToUpdate);
            if (result.IsSuccess)
            {
                _mediator.Notify("EventGroupUpdated", this, eventGroupToUpdate);
            }
            return result;
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
                _mediator.Notify("EventRemoved", this, eventModel);
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
                _mediator.Notify("EventTypeRemoved", this, eventType);
            }

            var deleteGroupResult = await _eventRepository.DeleteEventGroupAsync(groupToDelete);
            if (deleteGroupResult.IsSuccess)
            {
                AllEventGroupsOC.Remove(groupToDelete);
                _mediator.Notify("EventGroupRemoved", this, groupToDelete);
            }

            return deleteGroupResult;
        }
    }
}
