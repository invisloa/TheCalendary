using Kalendarzyk.Models.EventModels;
using System.Collections.ObjectModel;

namespace Kalendarzyk.Helpers
{
	public class EventTimeConflictChecker : IEventTimeConflictChecker
	{

		// foreach for debugging
		public bool IsEventConflict(bool isSubEventTimeDifferent, bool isEventGroupTimeDifferent, EventModel eventToCheck)
		{
			// Method for sub event type conflict check
			bool hasEventTypeConflict(EventTypeModel eventType)
			{
				foreach (var x in allEvents)
				{
					bool isSameEventType = x.EventType.Equals(eventType);
					bool isStartDateTimeConflict = x.StartDateTime < eventToCheck.EndDateTime;
					bool isEndDateTimeConflict = x.EndDateTime > eventToCheck.StartDateTime;

					if (isSameEventType && isStartDateTimeConflict && isEndDateTimeConflict)
					{
						return true;
					}
				}
				return false;
			}

			// Method for main event type conflict check
			bool hasEventGroupConflict(EventGroupModel eventGroupType)
			{
				foreach (var x in allEvents)
				{
					bool isSameEventGroup = x.EventType.EventGroup.Equals(eventGroupType);
					bool isStartDateTimeConflict = x.StartDateTime < eventToCheck.EndDateTime;
					bool isEndDateTimeConflict = x.EndDateTime > eventToCheck.StartDateTime;

					if (isSameEventGroup && isStartDateTimeConflict && isEndDateTimeConflict)
					{
						return true;
					}
				}
				return false;
			}


			if (isSubEventTimeDifferent && isEventGroupTimeDifferent)
			{
				return hasEventGroupConflict(eventToCheck.EventType.EventGroup);
			}
			else if (isEventGroupTimeDifferent)
			{
				return hasEventGroupConflict(eventToCheck.EventType.EventGroup);
			}
			else if (isSubEventTimeDifferent)
			{
				return hasEventTypeConflict(eventToCheck.EventType);
			}

			return false;
		}
		// todo changed for testing
		public ObservableCollection<EventModel> allEvents { get; set; }

		public EventTimeConflictChecker(ObservableCollection<EventModel> allEventsList)
		{
			this.allEvents = allEventsList;
		}
	}
}

