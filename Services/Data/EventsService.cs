using AsyncAwaitBestPractices;
using Kalendarzyk.Models.EventModels;
using Kalendarzyk.Models.EventTypesModels;
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

        public async Task DeleteEventGroupAsync(EventGroupModel groupToDelete)
        {
            // Delete all events that belong to the event group
            var eventsToDelete = AllEventsOC
                .Where(e => e.EventType.EventGroup.Equals(groupToDelete))
                .ToList();

            foreach (var eventModel in eventsToDelete)
            {
                AllEventsOC.Remove(eventModel);
                await _eventRepository.DeleteEventAsync(eventModel);
            }

            // Delete all event types that belong to the event group
            var eventTypesToDelete = AllEventTypesOC
                .Where(et => et.EventGroup.Equals(groupToDelete))
                .ToList();

            foreach (var eventType in eventTypesToDelete)
            {
                AllEventTypesOC.Remove(eventType);
                await _eventRepository.DeleteEventTypeAsync(eventType);
            }

            // Finally, delete the event group itself
            AllEventGroupsOC.Remove(groupToDelete);
            await _eventRepository.DeleteEventGroupAsync(groupToDelete);
        }

        public async Task DeleteEvenTypeAsync(EventTypeModel eventTypeToDelete)
        {
            // Delete all events that belong to the event type
            var eventsToDelete = AllEventsOC
                .Where(e => e.EventType.Equals(eventTypeToDelete))
                .ToList();

            foreach (var eventModel in eventsToDelete)
            {
                AllEventsOC.Remove(eventModel);
                await _eventRepository.DeleteEventAsync(eventModel);
            }

            // Delete the event type
            AllEventTypesOC.Remove(eventTypeToDelete);
            await _eventRepository.DeleteEventTypeAsync(eventTypeToDelete);
        }

        public async Task DeleteEvenAsync(EventModel eventToDelete)
        {
            // Delete the event
            AllEventsOC.Remove(eventToDelete);
            await _eventRepository.DeleteEventAsync(eventToDelete);
        }
    }
}
