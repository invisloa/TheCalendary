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
        private EventTypeViewModel _selectedEventType;

        // ctor for adding events
        public EventPageViewModel(DateTime selectedDate)
        {
            EventToEdit = new EventModel();
            _isEditMode = false;
            ExtraOptionsHelperToChangeName = Factory.CreateNewExtraOptionsEventHelperClass();
            EventGroupsCCHelper = Factory.CreateNewIEventGroupViewModelClass(_eventService.AllEventGroupsOC);
            AllEventTypesOC = new ObservableCollection<EventTypeViewModel>(_eventService.AllEventTypesOC.Select(x => new EventTypeViewModel(x)));
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
            // to change
            InitializeCommon();
        }

        // Initialize common properties
        private void InitializeCommon()
        {
            _mediator = Factory.GetMediator;

            EventTypesInfoButton = Factory.CreateNewChangableFontsIconAdapter(true, "info", "info_outline");
            SelectEventTypeCommand = new RelayCommand<EventTypeViewModel>(OnEventTypeSelectedCommand);
            // Subscribe to mediator notifications
            _mediator.Subscribe("EventGroupAdded", OnEventTypeAdded);
            _mediator.Subscribe("EventGroupUpdated", OnEventTypeUpdated);
            _mediator.Subscribe("EventGroupRemoved", OnEventTypeRemoved);

        }
        private void OnEventTypeAdded(object sender, object args)
        {
            if (args is EventTypeModel newGroup)
            {
                var newViewModel = new EventTypeViewModel(newGroup);
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
            if (args is EventGroupModel removedGroup)
            {
                var existingViewModel = AllEventTypesOC.FirstOrDefault(vm => vm.Id == removedGroup.Id);
                if (existingViewModel != null)
                {
                    AllEventTypesOC.Remove(existingViewModel);
                    OnPropertyChanged(nameof(AllEventTypesOC));
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

        // Other Properties
        public ExtraOptionsEventsHelperClass ExtraOptionsHelperToChangeName { get; set; }
        public MicroTasksCCAdapterVM MicroTasksCCAdapterVM { get; set; }
        public MeasurementSelectorCCViewModel MeasurementSelectorCCViewModel { get; set; }
        public DateStartEndCC DateStartEndCC { get; set; }

        // Commands
        public AsyncRelayCommand AsyncSubmitEventCommand { get; set; }
        public AsyncRelayCommand AsyncDeleteEventCommand { get; set; }
        public AsyncRelayCommand AsyncShareEventCommand { get; set; }
        public RelayCommand<EventTypeViewModel> SelectEventTypeCommand { get; set; }


        protected void OnEventTypeSelectedCommand(EventTypeViewModel selectedEventType)
        {
            //xxx
            SelectedEventType = selectedEventType;
            //if (!IsEditMode)
            //{
            //    ExtraOptionsHelperToChangeName.OnEventTypeChanged(selectedEventType);

            //    SetEndExactTimeAccordingToEventType();
            //}
            SetVisualsForSelectedEventType();
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
            //  deleted to check xxx // AllEventTypesOC = new ObservableCollection<EventTypeModel>(AllEventTypesOC); // ?????? this is done to trigger the property changed event
            //var selectedEventGroup = EventGroupsCCHelper.EventGroupsVisualsOC.Where(x => x.EventGroup.Equals(SelectedEventType.EventGroup)).FirstOrDefault();
            var selectedEventGroup = EventGroupsCCHelper.EventGroupsVisualsOC.Where(x => x.EventGroup.Id == SelectedEventType.EventGroup.Id).FirstOrDefault();      
            _eventGroupsCCHelper.EventGroupSelectedCommand.Execute(selectedEventGroup);

        }
    }
}
