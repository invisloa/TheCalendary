using Kalendarzyk.Models.EventTypesModels;
using System.Collections.Generic;

namespace Kalendarzyk.ViewModels.ModelsViewModels
{
    public class EventGroupViewModel : BaseViewModel
    {
        private readonly EventGroupModel _eventGroup;

        public EventGroupViewModel(EventGroupModel eventGroup)
        {
            _eventGroup = eventGroup;
            MainEventTitle = _eventGroup.GroupName;
        }

        public EventGroupModel EventGroup => _eventGroup;

        private string _eventGroupName;
        public string EventGroupName
        {
            get => _eventGroupName;
            set => SetProperty(ref _eventGroupName, value);
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        private string _mainEventTitle;
        public string MainEventTitle
        {
            get => _mainEventTitle;
            set => SetProperty(ref _mainEventTitle, value);
        }
    }
}
