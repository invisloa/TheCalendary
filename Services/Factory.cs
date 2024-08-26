using Kalendarzyk.Models;
using Kalendarzyk.Models.EventModels;
using Kalendarzyk.Models.EventTypesModels;
using Kalendarzyk.Services.Data;
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
        // Event Repository Singleton Pattern
        private static IEventRepository _eventRepository;

        public static IEventRepository GetEventRepository => _eventRepository;

        public static async Task<IEventRepository> InitializeEventRepository()
        {
            if (_eventRepository == null)
            {
                _eventRepository = await SQLiteRepository.CreateAsync();
            }
            return _eventRepository;
        }

        private static IEventsService _eventsService;

        public static async Task<IEventsService> InitializeEventService()
        {
            if (_eventsService == null)
            {
                var repository = await InitializeEventRepository();
                _eventsService = new EventsService(repository);
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
            return new EventGroupsSelectorCCViewModel(eventGroups);
        }

    }
}
