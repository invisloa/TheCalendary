using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kalendarzyk;
using Kalendarzyk.Helpers;
using Kalendarzyk.Models.EventModels;
using Kalendarzyk.Services;
using Kalendarzyk.Services.Data;
using Kalendarzyk.ViewModels.CCViewModels;
using Kalendarzyk.ViewModels.CustomControls;
using Kalendarzyk.ViewModels.CustomControls.Buttons;
using Kalendarzyk.ViewModels.ModelsViewModels;
using Kalendarzyk.Views;
using Kalendarzyk.Views.CustomControls.CCInterfaces;
using Kalendarzyk.Views.CustomControls.CCViewModels;
using System.Collections.ObjectModel;

namespace Kalendarzyk.ViewModels
{
    public class AddNewEventTypePageViewModel : BaseViewModel
    {
        #region Fields

        private IEventsService _eventService = Factory.GetEventService;
        private ChangableFontsIconCCViewModel _eventTypesInfoButton;
        private IEventGroupsCCViewModel _eventGroupsCCHelper;
        private EventTypeModel _currentType; // if null => add new type, else => edit type
        private string _typeName;
        private IEventRepository _eventRepository;
        private IColorButtonsSelectorHelperClass _colorButtonsHelperClass = Factory.CreateNewIColorButtonsHelperClass(startingColor: Colors.Red);
        private bool _canSubmitTypeCommand;
        private bool _isEventGroupSelected;
        #endregion

        #region Properties
        public bool CanSubmitTypeCommand
            {
            get => _canSubmitTypeCommand;
            set
            {
                _canSubmitTypeCommand = value;
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
        public ExtraOptionsEventTypesHelperClass ExtraOptionsHelperToChangeName { get; set; } = Factory.CreateNewExtraOptionsEventTypesHelperClass();
        public DefaultTimespanCCViewModel DefaultEventTimespanCCHelper { get; set; } = Factory.CreateNewDefaultEventTimespanCCHelperClass();
        public IColorButtonsSelectorHelperClass ColorButtonsHelperClass => _colorButtonsHelperClass;
        public string QuantityValueText => IsEdit ? "DEFAULT VALUE:" : "Value:";
        public string PageTitle => IsEdit ? "EDIT TYPE" : "ADD NEW TYPE";
        public string PlaceholderText => IsEdit ? $"TYPE NEW NAME FOR: {TypeName}" : "...NEW TYPE NAME...";
        public string SubmitButtonText => IsEdit ? "SUBMIT CHANGES" : "ADD NEW TYPE";
        public bool IsEdit => _currentType != null;
        public bool IsNotEdit => !IsEdit;
        public IEventGroupsCCViewModel EventGroupsCCHelper
        {
            get => _eventGroupsCCHelper;
            set
            {
                _eventGroupsCCHelper = value;
                OnPropertyChanged();
            }
        }
        public EventTypeModel CurrentType
        {
            get => _currentType;
            set
            {
                if (value == _currentType) return;
                _currentType = value;
                OnPropertyChanged();
            }
        }
        public string TypeName
        {
            get => _typeName;
            set
            {
                if (value == _typeName) return;
                _typeName = value;
                AsyncSubmitTypeCommand.RaiseCanExecuteChanged();   // not fully working, no idea why...
                CanExecuteAsyncSubmitTypeCommand();

                OnPropertyChanged();
            }
        }
        public ObservableCollection<SelectableButtonViewModel> ButtonsColorsOC { get; set; }
        #endregion

        #region Commands
        public RelayCommand<SelectableButtonViewModel> SelectColorCommand { get; private set; }
        public AsyncRelayCommand AsyncSubmitTypeCommand { get; private set; }
        public AsyncRelayCommand AsyncDeleteSelectedEventTypeCommand { get; private set; }

        #endregion

        #region Constructors

        // Constructor for create mode
        public AddNewEventTypePageViewModel()
        {
            try
            {
                InitializeCommon();
                DefaultEventTimespanCCHelper.SelectedUnitIndex = 0; // minutes
                DefaultEventTimespanCCHelper.DurationValue = 30;
            }
            catch (Exception ex)
            {
                App.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        // Constructor for edit mode
        public AddNewEventTypePageViewModel(EventTypeModel currentType)
        {
            CurrentType = currentType;
            InitializeCommon();
            ColorButtonsHelperClass.SelectedColor = Color.FromArgb(currentType.EventTypeColorString);
            TypeName = currentType.EventTypeName;
            DefaultEventTimespanCCHelper.SetControlsValues(currentType.DefaultEventTimeSpan);
            AsyncDeleteSelectedEventTypeCommand = new AsyncRelayCommand(AsyncDeleteSelectedEventType);
        }

        #endregion

        #region Methods

        private void InitializeCommon()
        {
            _eventRepository = Factory.GetEventRepository;

            EventTypesInfoButton = Factory.CreateNewChangableFontsIconAdapter(true, "info", "info_outline");
            EventGroupsCCHelper = Factory.CreateNewIEventGroupViewModelClass(_eventService.AllEventGroupsOC);
            bool isEditMode = CurrentType != null;
            AsyncSubmitTypeCommand = new AsyncRelayCommand(AsyncSubmitType, CanExecuteAsyncSubmitTypeCommand);

            InitializeColorButtons();
        }

        private void OnEventGroupChanged(EventGroupModel newEventGroup)
        {
            AsyncSubmitTypeCommand.RaiseCanExecuteChanged();
            CanExecuteAsyncSubmitTypeCommand();
        }

        private async Task AsyncDeleteSelectedEventType()
        {
            var eventTypesInDb = _eventService.AllEventsOC.Where(x => x.EventType.Equals(_currentType));
            if (eventTypesInDb.Any())
            {
                var action = await App.Current.MainPage.DisplayActionSheet("This type is used in some events.", "Cancel", null, "Delete all associated events", "Go to All Events Page");
                switch (action)
                {
                    case "Delete all associated events":
                        await _eventService.DeleteEventTypeAsync(_currentType);
                        break;
                    case "Go to All Events Page":
                        await Shell.Current.GoToAsync("AllEventsPage");
                        break;
                    default:
                        break;
                }
                return;
            }
            await Shell.Current.GoToAsync(".."); // TODO CHANGE NOT WORKING!!!
        }

        private async Task AsyncSubmitType()
        {
            if (IsEdit)
            {
                    _currentType.EventGroup = EventGroupsCCHelper.SelectedEventGroup;
                    _currentType.EventTypeName = TypeName;
                    _currentType.EventTypeColorString = ColorButtonsHelperClass.SelectedColor.ToArgbHex();
                    if (ExtraOptionsHelperToChangeName.DefaultMeasurementSelectorCCHelper.IsValueTypeSelected)
                        _currentType.DefaultQuantity = ExtraOptionsHelperToChangeName.DefaultMeasurementSelectorCCHelper.QuantityAmount;
                    _currentType.DefaultMicroTasks = ExtraOptionsHelperToChangeName.MicroTasksCCAdapter.MicroTasksOC;

                    await _eventRepository.UpdateEventTypeAsync(_currentType);
                    await Shell.Current.GoToAsync("..");    // TODO CHANGE NOT WORKING!!!
            }
            else
            {
                    //var timespan = EventTypeExtraOptionsHelper.IsDefaultEventTimespanSelected ? DefaultEventTimespanCCHelper.GetDuration() : TimeSpan.Zero;
                    var quantityAmount = ExtraOptionsHelperToChangeName.DefaultMeasurementSelectorCCHelper.IsValueTypeSelected ? ExtraOptionsHelperToChangeName.DefaultMeasurementSelectorCCHelper.QuantityAmount : null;
                    var microTasks = ExtraOptionsHelperToChangeName.IsMicroTasksType ? new List<MicroTaskModel>(ExtraOptionsHelperToChangeName.MicroTasksCCAdapter.MicroTasksOC) : null;
                    var newEventType = Factory.CreateNewEventType(EventGroupsCCHelper.SelectedEventGroup, TypeName, ColorButtonsHelperClass.SelectedColor.ToArgbHex(), TimeSpan.FromHours(1), quantityAmount, microTasks);
                    await _eventRepository.AddEventTypeAsync(newEventType);
                    TypeName = string.Empty;
            }
        }

        private void InitializeColorButtons() //TODO ! also to extract as a separate custom control
        {
            ButtonsColorsOC = ButtonsColorsInitializerHelper.InitializeColorButtons();
        }

        public void SetExtraUserControlsValues(EventTypeModel _currentType)
        {
            if (_currentType == null)
            {
                return;
            }
        }

        #endregion
        #region Command CanExecute Methods
        public bool CanExecuteAsyncSubmitTypeCommand()
        { 
         if(string.IsNullOrEmpty(TypeName) || EventGroupsCCHelper.SelectedEventGroup == null)
            {
                CanSubmitTypeCommand = false;
                return false;
            }
            CanSubmitTypeCommand = true;
            return true;
        }
        #endregion
    }
}
