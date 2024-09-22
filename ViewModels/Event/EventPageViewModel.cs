using Kalendarzyk.Mediator;
using Kalendarzyk.Models.EventModels;
using Kalendarzyk.Services;
using Kalendarzyk.Services.Data;
using Kalendarzyk.ViewModels.CCViewModels;
using Kalendarzyk.ViewModels.ModelsViewModels;
using Kalendarzyk.Views.CustomControls;
using Kalendarzyk.Views.CustomControls.CCViewModels;
using Kalendarzyk.Views.CustomControls.ExtraOptionsCC;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kalendarzyk.ViewModels.Event
{
    public class EventPageViewModel : BaseViewModel,  IDisposable
    {
        // Fields
        private bool _isEditMode;
        private bool _canSubmitEvent;
        private EventModel _eventToEdit;
        private  IMediator _mediator;
        private IEventGroupsCCViewModel _eventGroupsCCHelper;
        private IEventRepository _eventRepository = Factory.GetEventRepository;
        private IEventsService _eventService = Factory.GetEventService;
        private ObservableCollection<EventTypeViewModel> _eventTypesOC;
        private ChangableFontsIconCCViewModel _eventTypesInfoButton;
        private ChangableFontsIconCCViewModel _isCompletedButton;
        private EventTypeViewModel _selectedEventType;

        // ctor for adding events
        public EventPageViewModel(DateTime selectedDate)
        {
            EventToEdit = new EventModel();
            _isEditMode = false;
            ExtraOptionsHelperToChangeName = Factory.CreateNewExtraOptionsEventHelperClass();
            EventGroupsCCHelper = Factory.CreateNewIEventGroupViewModelClass(_eventService.AllEventGroupsOC);
            AllEventTypesOC = new ObservableCollection<EventTypeViewModel>(_eventService.AllEventTypesOC.Select(x => new EventTypeViewModel(x)));
            EventStartDate = selectedDate;
            EventEndDate = selectedDate;
            StartExactTime = selectedDate.TimeOfDay;
            InitializeCommon();
        }

        // ctor for editing events
        public EventPageViewModel(EventModel eventToEdit)
        {
            _eventToEdit = eventToEdit;
            _isEditMode = true;
            ExtraOptionsHelperToChangeName = Factory.CreateNewExtraOptionsEventHelperClass(eventToEdit.EventType);
            EventGroupsCCHelper = Factory.CreateNewIEventGroupViewModelClass(_eventService.AllEventGroupsOC);
            EventGroupsCCHelper.EventGroupSelectedCommand.Execute(EventGroupsCCHelper.EventGroupsVisualsOC.Where(x => x.Id == eventToEdit.EventType.EventGroupId).First()); // to check
            AllEventTypesOC = new ObservableCollection<EventTypeViewModel>(_eventService.AllEventTypesOC.Select(x => new EventTypeViewModel(x)));
            InitializeCommon();
        }

        // Initialize common properties
        private void InitializeCommon()
        {
            _mediator = Factory.GetMediator;

            EventTypesInfoButton = Factory.CreateNewChangableFontsIconAdapter(true, "info", "info_outline");
            IsCompletedButton = Factory.CreateNewChangableFontsIconAdapter(false, "check_box", "check_box_outline_blank");
            SelectEventTypeCommand = new RelayCommand<EventTypeViewModel>(OnEventTypeSelectedCommand);

            // Subscribe to EventType mediator notifications
            _mediator.Subscribe("EventTypeAdded", OnEventTypeAdded);
            _mediator.Subscribe("EventTypeUpdated", OnEventTypeUpdated);
            _mediator.Subscribe("EventTypeRemoved", OnEventTypeRemoved);

            // Subscribe to EventGroup mediator notifications
            _mediator.Subscribe("EventGroupAdded", OnEventGroupAdded);
            _mediator.Subscribe("EventGroupUpdated", OnEventGroupUpdated);
            _mediator.Subscribe("EventGroupRemoved", OnEventGroupRemoved);
        }

        // EventType Methods
        private void OnEventTypeAdded(object sender, object args)
        {
            if (args is EventTypeModel newType)
            {
                var newViewModel = new EventTypeViewModel(newType);
                AllEventTypesOC.Add(newViewModel);
            }
        }

        private void OnEventTypeUpdated(object sender, object args)
        {
            if (args is EventTypeModel updatedType)
            {
                var existingViewModel = AllEventTypesOC.FirstOrDefault(vm => vm.Id == updatedType.Id);
                if (existingViewModel != null)
                {
                    existingViewModel.UpdateFromModel(updatedType);
                    OnPropertyChanged(nameof(AllEventTypesOC));
                }
            }
        }

        private void OnEventTypeRemoved(object sender, object args)
        {
            if (args is EventTypeModel removedType)
            {
                var existingViewModel = AllEventTypesOC.FirstOrDefault(vm => vm.Id == removedType.Id);
                if (existingViewModel != null)
                {
                    AllEventTypesOC.Remove(existingViewModel);
                    OnPropertyChanged(nameof(AllEventTypesOC));
                }
            }
        }

        // EventGroup Methods
        private void OnEventGroupAdded(object sender, object args)
        {
            if (args is EventGroupModel newGroup)
            {
                var newViewModel = new EventGroupViewModel(newGroup);
                EventGroupsCCHelper.EventGroupsVisualsOC.Add(newViewModel);
                OnPropertyChanged(nameof(EventGroupsCCHelper.EventGroupsVisualsOC));

            }
        }

        private void OnEventGroupUpdated(object sender, object args)
        {
            if (args is EventGroupModel updatedGroup)
            {
                var existingViewModel = EventGroupsCCHelper.EventGroupsVisualsOC
                    .FirstOrDefault(vm => vm.EventGroup.Id == updatedGroup.Id);

                if (existingViewModel != null)
                {
                    existingViewModel.UpdateFromModel(updatedGroup);
                    OnPropertyChanged(nameof(EventGroupsCCHelper.EventGroupsVisualsOC));
                }
            }
        }

        private void OnEventGroupRemoved(object sender, object args)
        {
            if (args is EventGroupModel removedGroup)
            {
                var existingViewModel = EventGroupsCCHelper.EventGroupsVisualsOC
                    .FirstOrDefault(vm => vm.EventGroup.Id == removedGroup.Id);

                if (existingViewModel != null)
                {
                    EventGroupsCCHelper.EventGroupsVisualsOC.Remove(existingViewModel);
                    OnPropertyChanged(nameof(EventGroupsCCHelper.EventGroupsVisualsOC));
                }
            }
        }

        // Dispose Method
        public void Dispose()
        {
            _mediator.Unsubscribe("EventGroupAdded", OnEventTypeAdded);
            _mediator.Unsubscribe("EventGroupUpdated", OnEventTypeUpdated);
            _mediator.Unsubscribe("EventGroupRemoved", OnEventTypeRemoved);
        }
        // Properties
        public EventTypeViewModel SelectedEventType
        {
            get { return _selectedEventType; }
            set
            {
                _selectedEventType = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<EventTypeViewModel> AllEventTypesOC
        {
            get => _eventTypesOC;
            set
            {
                _eventTypesOC = value;
                OnPropertyChanged();
            }
        }

        public IEventGroupsCCViewModel EventGroupsCCHelper
        {
            get => _eventGroupsCCHelper;
            set
            {
                _eventGroupsCCHelper = value;
                OnPropertyChanged();
            }
        }

        public bool CanSubmitEvent
        {
            get { return _canSubmitEvent; }
            set
            {
                _canSubmitEvent = value;
                OnPropertyChanged();
            }
        }

        public ChangableFontsIconCCViewModel EventTypesInfoButton
        {
            get => _eventTypesInfoButton;
            set
            {
                _eventTypesInfoButton = value;
                OnPropertyChanged();
            }
        }
        public ChangableFontsIconCCViewModel IsCompletedButton
        {
            get => _isCompletedButton;
            set
            {
                _isCompletedButton = value;
                OnPropertyChanged();
            }
        }

        public string PageTitle => _isEditMode ? "EDIT EVENT" : "ADD NEW EVENT";
        public string SubmitButtonText => _isEditMode ? "SUBMIT CHANGES" : "ADD NEW EVENT";
        public bool IsEditMode => _isEditMode;

        public EventModel EventToEdit
        {
            get { return _eventToEdit; }
            set
            {
                _eventToEdit = value;
                OnPropertyChanged();
            }
        }

        public string EventName
        {
            get { return _eventToEdit.Name; }
            set
            {
                _eventToEdit.Name = value;
                OnPropertyChanged();
            }
        }

        public string EventDescription
        {
            get { return _eventToEdit.Description; }
            set
            {
                _eventToEdit.Description = value;
                OnPropertyChanged();
            }
        }

        public DateTime EventStartDate
        {
            get { return _eventToEdit.StartDateTime; }
            set
            {
                _eventToEdit.StartDateTime = value;
                OnPropertyChanged();
            }
        }

        public DateTime EventEndDate
        {
            get { return _eventToEdit.EndDateTime; }
            set
            {
                _eventToEdit.EndDateTime = value;
                OnPropertyChanged();
            }
        }

        private TimeSpan _startExactTime;
        public TimeSpan StartExactTime
        {
            get => _startExactTime;
            set
            {
                if (_startExactTime != value)
                {
                    _startExactTime = value;
                    OnPropertyChanged();
                }
            }
        }
        private TimeSpan _endExactTime;
        public TimeSpan EndExactTime
        {
            get => _endExactTime;
            set
            {
                if (_endExactTime != value)
                {
                    _endExactTime = value;
                    OnPropertyChanged();
                }
            }
        }

        // Other Properties
        public ExtraOptionsEventsHelperClass ExtraOptionsHelperToChangeName { get; set; }
        public MicroTasksCCAdapterVM MicroTasksCCAdapterVM { get; set; }
        public MeasurementSelectorCCViewModel MeasurementSelectorCCViewModel { get; set; }
        // Commands
        public AsyncRelayCommand AsyncSubmitEventCommand { get; set; } 
        public AsyncRelayCommand AsyncDeleteEventCommand { get; set; }
        public AsyncRelayCommand AsyncShareEventCommand { get; set; }
        public RelayCommand<EventTypeViewModel> SelectEventTypeCommand { get; set; }


        protected void OnEventTypeSelectedCommand(EventTypeViewModel selectedEventType)
        {
            //xxx
            SelectedEventType = selectedEventType;
            if (!IsEditMode)
            {
                ExtraOptionsHelperToChangeName.OnEventTypeChanged(selectedEventType.EventTypeModel);

                SetEndExactTimeAccordingToEventType();
            }
            ExtraOptionsHelperToChangeName.OnEventTypeChanged(selectedEventType.EventTypeModel);

            SetVisualsForSelectedEventType();
        }
        private void SetEndExactTimeAccordingToEventType()
        {
            try
            {
                var timeSpanAdded = StartExactTime.Add(SelectedEventType.DefaultEventTimeSpan);
                int days = (int)timeSpanAdded.TotalDays;
                TimeSpan remainingTime = TimeSpan.FromHours(timeSpanAdded.Hours).Add(TimeSpan.FromMinutes(timeSpanAdded.Minutes)).Add(TimeSpan.FromSeconds(timeSpanAdded.Seconds));
                EventToEdit.EndDateTime = EventToEdit.StartDateTime.AddDays(days);
                OnPropertyChanged(nameof(EventEndDate));    
                EndExactTime = remainingTime;
            }
            catch
            {
                EndExactTime = StartExactTime;
            }
        }
        protected void SetVisualsForSelectedEventType()
        {
            foreach (var eventType in AllEventTypesOC)
            {
                eventType.IsSelectedToFilter = false;

                if (eventType.Equals(_selectedEventType))
                {
                    SelectedEventType = eventType; // Assign it when found
                }
            }

            SelectedEventType.IsSelectedToFilter = true;
            var selectedEventGroup = EventGroupsCCHelper.EventGroupsVisualsOC.Where(x => x.EventGroup.Id == SelectedEventType.EventGroup.Id).FirstOrDefault();      
            _eventGroupsCCHelper.EventGroupSelectedCommand.Execute(selectedEventGroup);

        }
    }
}
