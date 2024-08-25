using Kalendarzyk.Models;
using Kalendarzyk.Models.EventModels;
using Kalendarzyk.Models.EventTypesModels;
using Kalendarzyk.Services.Data;
using Kalendarzyk.ViewModels.CustomControls;
using Kalendarzyk.ViewModels.CustomControls.Buttons;
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
    {        // Event Repository Singleton Pattern
        private static IEventRepository _eventRepository;

        public static IEventRepository GetEventRepository => _eventRepository;

        public static async Task<IEventRepository> InitializeEventRepository()  // TODO xxx USE IT SOMEWHERE AT STARTUP
        {
            Console.WriteLine("start initialize");
            if (_eventRepository == null)
                _eventRepository = await SQLiteRepository.CreateAsync();
            Console.WriteLine("end initialize");
            return _eventRepository;
        }
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
        internal static IEventsService CreateNewEventService()
        {
            return new EventsService(_eventRepository);
        }

        internal static IconModel CreateGroupVisualElement(string selectedIconString, Color backgroundColor, Color textColor)
        {
            return new IconModel(selectedIconString, backgroundColor, textColor);
        }
        internal static EventGroupModel CreateNewEventGroup(string mainTypeName, IconModel iconForEventGroup)
        {
            return new EventGroupModel(mainTypeName, iconForEventGroup);
        }
    }
}
