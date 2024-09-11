using Kalendarzyk.Helpers;
using Kalendarzyk.Mediator;
using Kalendarzyk.Models.EventModels;
using Kalendarzyk.Services.Data;
using Kalendarzyk.ViewModels.CCViewModels;
using Kalendarzyk.ViewModels.CustomControls;
using Kalendarzyk.ViewModels.CustomControls.Buttons;
using Kalendarzyk.ViewModels.ModelsViewModels;
using Kalendarzyk.Views.CustomControls.CCInterfaces;
using Kalendarzyk.Views.CustomControls.CCViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Kalendarzyk.Services
{
    internal class Factory
    {
        public static object ButtonOpacityValue { get; set; } = 0.65;



        // Event Repository Singleton Pattern
        private static IEventRepository _eventRepository;
        private static IMediator _mediator;

        public static IEventRepository GetEventRepository => _eventRepository;
        public static IMediator GetMediator
        {
            get
            {
                if (_mediator == null)
                {
                    _mediator = new EventMediator();
                }
                return _mediator;
            }
        }

        public static async Task<IEventRepository> InitializeEventRepository()
        {
            if (_eventRepository == null)
            {
                _eventRepository = await SQLiteRepository.CreateAsync();
            }
            //await _eventRepository.ResetDatabaseAsync();
            return _eventRepository;
        }

        private static IEventsService _eventsService;

        public static async Task<IEventsService> InitializeEventService()
        {
            if (_eventsService == null)
            {
                var repository = await InitializeEventRepository();
                _eventsService = new EventsService(repository, GetMediator);
                await _eventsService.InitializeDataAsync();
            }
            return _eventsService;
        }
        public static IEventsService GetEventService => _eventsService;


        public static ObservableCollection<MeasurementUnitItem> PopulateMeasurementCollection()
        {
            var descriptions = new Dictionary<MeasurementUnit, string>();
            var currencySymbol = CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol;

            foreach (MeasurementUnit unit in Enum.GetValues(typeof(MeasurementUnit)))
            {
                descriptions[unit] = unit == MeasurementUnit.Money ? currencySymbol : unit.GetDescription();
            }

            var measurementUnitItems = new List<MeasurementUnitItem>(descriptions.Count);

            foreach (var unit in descriptions)
            {
                measurementUnitItems.Add(new MeasurementUnitItem(unit.Key)
                {
                    DisplayName = unit.Value
                });
            }

            return new ObservableCollection<MeasurementUnitItem>(measurementUnitItems);
        }

        public static IColorButtonsSelectorHelperClass CreateNewIColorButtonsHelperClass(
                ObservableCollection<SelectableButtonViewModel> colorButtons = null,
                ICommand selectedButtonCommand = null,
                Color? startingColor = null)
        {
            return new ColorButtonsSelectorViewModel(colorButtons, selectedButtonCommand, startingColor);
        }

        internal static IconModel CreateGroupVisualElement(string selectedIconString, Color backgroundColor, Color textColor)
        {
            return new IconModel(selectedIconString, backgroundColor, textColor);
        }
        internal static EventGroupModel CreateNewEventGroup(string mainTypeName, IconModel iconForEventGroup)
        {
            return new EventGroupModel(mainTypeName, iconForEventGroup);
        }
        internal static DefaultTimespanCCViewModel CreateNewDefaultEventTimespanCCHelperClass()
        {
            return new DefaultTimespanCCViewModel();
        }
        internal static ChangableFontsIconCCViewModel CreateNewChangableFontsIconAdapter(bool isSelected, string selectedIconText, string notSelectedIconText)
        {
            return new ChangableFontsIconCCViewModel(isSelected, selectedIconText, notSelectedIconText);
        }
        public static IEventGroupsCCViewModel CreateNewIEventGroupViewModelClass(ObservableCollection<EventGroupModel> eventGroups)
        {
            return new EventGroupsSelectorCCViewModel(eventGroups, GetMediator);
        }
        public static MeasurementSelectorCCViewModel CreateNewMeasurementSelectorCCHelperClass()
        {
            return new MeasurementSelectorCCViewModel();
        }
        internal static MicroTasksCCAdapterVM CreateNewMicroTasksCCAdapter(IEnumerable<MicroTaskModel> listToAddMiroTasks)
        {
            return new MicroTasksCCAdapterVM(listToAddMiroTasks);
        }
        internal static ExtraOptionsEventTypesHelperClass CreateNewExtraOptionsEventTypesHelperClass() => new ExtraOptionsEventTypesHelperClass();
        public static EventTypeModel CreateNewEventType(EventGroupModel EventGroup, string eventTypeName, string eventTypeColorString, TimeSpan defaultEventTime, QuantityModel quantity = null, List<MicroTaskModel> microTasksList = null)
        {
            var eventTypeModel = new EventTypeModel(EventGroup, eventTypeName, eventTypeColorString, defaultEventTime, quantity, microTasksList);

            return eventTypeModel;
        }
        internal static IEventTimeConflictChecker CreateNewEventTimeConflictChecker(ObservableCollection<EventModel> allEventsList)
        {
            return new EventTimeConflictChecker(allEventsList);
        }
        internal static ExtraOptionsEventsHelperClass CreateNewExtraOptionsEventHelperClass() => new ExtraOptionsEventsHelperClass();
        internal static ExtraOptionsEventsHelperClass CreateNewExtraOptionsEventHelperClass(EventTypeModel eventTypeModel) => new ExtraOptionsEventsHelperClass(eventTypeModel);
    }
}
