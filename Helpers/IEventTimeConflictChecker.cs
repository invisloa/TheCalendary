using Kalendarzyk.Models.EventModels;
using System.Collections.ObjectModel;

namespace Kalendarzyk.Helpers
{
	public interface IEventTimeConflictChecker
	{
		public ObservableCollection<EventModel> allEvents { get; set; }

		bool IsEventConflict(bool isSubEventTimeDifferent, bool isEventGroupTimeDifferent, EventModel eventToCheck);
	}
}