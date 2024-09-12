using CommunityToolkit.Mvvm.ComponentModel;
using Kalendarzyk.Models.EventModels;
using Kalendarzyk.Services;
using Kalendarzyk.ViewModels;
using Kalendarzyk.ViewModels.CustomControls.Buttons;
using Kalendarzyk.Views.CustomControls.ExtraOptionsCC.ExtraOptionsViewModels;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Core.Extensions;

namespace Kalendarzyk.Views.CustomControls.CCViewModels
{

	// TODO HERE 27.03.24 WHEN SWITCHING SUB TYPE IN EVENTS ADDING THE CONTROLS cONTROLS ARE NOT REFRESHED Accordingly
	public partial class ExtraOptionsEventsHelperClass :  ExtraOptionsBaseClass
	{
		private bool _isDateBtnSelected;

		public bool IsDateBtnSelected
        {
            get => _isDateBtnSelected;
            set => SetProperty(ref _isDateBtnSelected, value);
        }
		//ctor create mode
		public ExtraOptionsEventsHelperClass()
		{
			InitializeExtraOptionsButtons();
		}
		//ctor edit mode
		public ExtraOptionsEventsHelperClass(EventTypeModel eventTypeModel)
		{
			EventType = eventTypeModel;

            InitializeExtraOptionsButtons();

			if (EventType.IsValueType)
			{
				IsValueType = true;
				DefaultMeasurementSelectorCCHelper = Factory.CreateNewMeasurementSelectorCCHelperClass();
				DefaultMeasurementSelectorCCHelper.QuantityAmount = new QuantityModel(DefaultMeasurementSelectorCCHelper.SelectedMeasurementUnit.TypeOfMeasurementUnit, DefaultMeasurementSelectorCCHelper.QuantityValue);
				if (EventType.DefaultQuantity != null && EventType.DefaultQuantity.Value != 0)
				{
					OnIsEventValueType(ExtraOptionsButtonsSelectors[1]); // TODO refactor this
					DefaultMeasurementSelectorCCHelper.SelectedMeasurementUnit = DefaultMeasurementSelectorCCHelper.MeasurementUnitsOC.Where(x => x.TypeOfMeasurementUnit == EventType.DefaultQuantity.Unit).First();
					DefaultMeasurementSelectorCCHelper.QuantityValue = EventType.DefaultQuantity.Value;
				}
				DefaultMeasurementSelectorCCHelper.SelectPropperMeasurementData(EventType);
				DefaultMeasurementSelectorCCHelper.MeasurementUnitsOC = DefaultMeasurementSelectorCCHelper.MeasurementUnitsOC.Where(x => x.TypeOfMeasurementUnit == EventType.DefaultQuantity.Unit).ToObservableCollection();

			}

			if (EventType.IsMicroTaskType)
			{
				IsMicroTasksType = true;
				MicroTasksCCAdapter = Factory.CreateNewMicroTasksCCAdapter(new List<MicroTaskModel>());

				if (EventType.DefaultMicroTasks != null && EventType.DefaultMicroTasks.Count() > 0)
				{
					OnIsMicroTasksSelected(ExtraOptionsButtonsSelectors[0]); // TODO refactor this
					MicroTasksCCAdapter.MicroTasksOC = EventType.DefaultMicroTasks;

				}
			}
			SetPropperValueType();

		}
		private void InitializeExtraOptionsButtons() // TODO JO XXX REFACTOR THIS to be more modular
		{
			ExtraOptionsButtonsSelectors.Add(new SelectableButtonViewModel("Micro Tasks", false, new RelayCommand<SelectableButtonViewModel>(OnIsMicroTasksSelected), isEnabled: EventType?.IsMicroTaskType == true));
			ExtraOptionsButtonsSelectors.Add(new SelectableButtonViewModel("Value", false, new RelayCommand<SelectableButtonViewModel>(OnIsEventValueType), isEnabled: EventType?.IsValueType == true));
			ExtraOptionsButtonsSelectors.Add(new SelectableButtonViewModel("DATE", false, new RelayCommand<SelectableButtonViewModel>(OnIsDateControlsSelectedCommand), isEnabled: EventType != null));
		}
		private void ReloadExtraOptionsButtons() // TODO JO XXX REFACTOR THIS to be more modular
		{
			ExtraOptionsButtonsSelectors[1].IsEnabled = EventType?.IsValueType ?? true;
			ExtraOptionsButtonsSelectors[1].IsSelected = IsValueType;
			ExtraOptionsButtonsSelectors[0].IsEnabled = EventType?.IsMicroTaskType ?? true;
			ExtraOptionsButtonsSelectors[0].IsSelected = IsMicroTasksType;
			ExtraOptionsButtonsSelectors[2].IsEnabled = true;
			ExtraOptionsButtonsSelectors[2].IsSelected = IsDateBtnSelected;
		}


		private void OnIsDateControlsSelectedCommand(SelectableButtonViewModel clickedButton)
		{
			IsDateBtnSelected = !clickedButton.IsSelected;
			SelectableButtonViewModel.MultiButtonSelection(clickedButton);
		}

		internal void OnEventTypeChanged(EventTypeModel selectedEventType)	
		{
			if(selectedEventType == EventType)
			{ 			
				return;
			}
			EventType = selectedEventType;
			ReloadExtraOptionsButtons();
			IsMicroTasksType = selectedEventType.IsMicroTaskType ? true : false;
			if (IsMicroTasksType)
			{
				if (MicroTasksCCAdapter == null)
				{
					MicroTasksCCAdapter = Factory.CreateNewMicroTasksCCAdapter(selectedEventType.DefaultMicroTasks);
				}
				else
				{
					MicroTasksCCAdapter.MicroTasksOC = selectedEventType.DefaultMicroTasks;
				}
			}


			IsValueType = selectedEventType.IsValueType ? true : false;
			if (IsValueType)
			{
				DefaultMeasurementSelectorCCHelper = Factory.CreateNewMeasurementSelectorCCHelperClass();
				// TODO chcange this so it will look for types in similair families (kg, g, mg, etc...)
				var measurementUnitsForSelectedType = DefaultMeasurementSelectorCCHelper.MeasurementUnitsOC.Where(unit => unit.TypeOfMeasurementUnit == selectedEventType.DefaultQuantity.Unit); // TO CHECK!
				DefaultMeasurementSelectorCCHelper.QuantityAmount = selectedEventType.DefaultQuantity;

				DefaultMeasurementSelectorCCHelper.MeasurementUnitsOC = new ObservableCollection<MeasurementUnitItem>(measurementUnitsForSelectedType);
				DefaultMeasurementSelectorCCHelper.SelectPropperMeasurementData(selectedEventType);
				OnPropertyChanged(nameof(DefaultMeasurementSelectorCCHelper.MeasurementUnitsOC));
			}
			else
			{
				DefaultMeasurementSelectorCCHelper.QuantityAmount = null;
			}
		}
	}
}
