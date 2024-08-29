using CommunityToolkit.Mvvm.ComponentModel;
using Kalendarzyk.ViewModels;
using Kalendarzyk.Views.CustomControls.ExtraOptionsCC.ExtraOptionsViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kalendarzyk.Services;
using Kalendarzyk.Models.EventModels;
using Kalendarzyk.ViewModels.CustomControls.Buttons;
using CommunityToolkit.Maui.Core.Extensions;


namespace Kalendarzyk.Views.CustomControls.CCViewModels
{
	 public partial class ExtraOptionsEventTypesHelperClass : ExtraOptionsBaseClass
	{
		[ObservableProperty]
		private bool _isColorBtnSelected;

		[ObservableProperty]
		private Command _onIsColorBtnSelectedCommand;


		//ctor create mode
		public ExtraOptionsEventTypesHelperClass()
		{
			//EventType = Factory.CreateNewEventTypeModel();
			InitializeCommon();
		}
		//ctor edit mode
		public ExtraOptionsEventTypesHelperClass(EventTypeModel eventTypeModel)
		{
			EventType = eventTypeModel;

			InitializeCommon();
			ExtraOptionsButtonsSelectors[1].IsEnabled = false;

			if (EventType.IsValueType)
			{
				IsValueType = true;
				ExtraOptionsButtonsSelectors[1].ButtonCommand = null;
				DefaultMeasurementSelectorCCHelper = Factory.CreateNewMeasurementSelectorCCHelperClass();
				DefaultMeasurementSelectorCCHelper.QuantityAmount = new QuantityModel(DefaultMeasurementSelectorCCHelper.SelectedMeasurementUnit.TypeOfMeasurementUnit, DefaultMeasurementSelectorCCHelper.QuantityValue);
				if (EventType.DefaultQuantity != null )
				{
					OnIsEventValueType(ExtraOptionsButtonsSelectors[1]); // TODO refactor this
					var unitToSelect = DefaultMeasurementSelectorCCHelper.MeasurementUnitsOC.Where(x => x.TypeOfMeasurementUnit == EventType.DefaultQuantity.Unit).First();
					DefaultMeasurementSelectorCCHelper.SelectedMeasurementUnit = unitToSelect;
					DefaultMeasurementSelectorCCHelper.MeasurementUnitsOC = new ObservableCollection<MeasurementUnitItem> { unitToSelect };		// TODO JO REFACTOR THIS, ONLY TEMPORARY - NO TIME :(
					DefaultMeasurementSelectorCCHelper.QuantityValue = EventType.DefaultQuantity.Value;
				}
				DefaultMeasurementSelectorCCHelper.SelectPropperMeasurementData(EventType);
			}

			if (EventType.IsMicroTaskType)
			{
				IsMicroTasksType = true;
				MicroTasksCCAdapter = Factory.CreateNewMicroTasksCCAdapter(new List<MicroTaskModel>());

				if (EventType.DefaultMicroTasks != null && EventType.DefaultMicroTasks.Count() > 0)
				{
					OnIsMicroTasksSelected(ExtraOptionsButtonsSelectors[0]); // TODO refactor this
					MicroTasksCCAdapter.MicroTasksOC = EventType.DefaultMicroTasks.ToObservableCollection();
				}
			}
			SetPropperValueType();

		}
		private void InitializeCommon() // TODO JO XXX REFACTOR THIS to be more modular
		{
			if (ExtraOptionsButtonsSelectors.Count == 0)
			{
				ExtraOptionsButtonsSelectors.Add(new SelectableButtonViewModel("Micro Tasks(D)", false, new RelayCommand<SelectableButtonViewModel>(OnIsMicroTasksSelected), isEnabled: true));
				ExtraOptionsButtonsSelectors.Add(new SelectableButtonViewModel("Value(D)", false, new RelayCommand<SelectableButtonViewModel>(OnIsEventValueType), isEnabled: true));
				ExtraOptionsButtonsSelectors.Add(new SelectableButtonViewModel("BG color", false, new RelayCommand<SelectableButtonViewModel>(OnColorBtnSelected), isEnabled: true));
			}
			else
			{
				// TODO JO XXX  just for testing this might never be executed if so remove it
				throw new Exception("ExtraOptionsButtonsSelectors.Count != 0");

				ExtraOptionsButtonsSelectors[0].IsEnabled = EventType?.IsMicroTaskType ?? true;
				ExtraOptionsButtonsSelectors[0].IsSelected = ExtraOptionsButtonsSelectors[0].IsEnabled ? IsMicroTasksBtnSelected : false;
				ExtraOptionsButtonsSelectors[1].IsEnabled = EventType?.IsValueType ?? true;
				ExtraOptionsButtonsSelectors[1].IsSelected = ExtraOptionsButtonsSelectors[1].IsEnabled ? IsValueBtnSelected : false;
			}
		}
		private void OnColorBtnSelected(SelectableButtonViewModel clickedButton)
		{
			IsColorBtnSelected = UpdateButtonState(clickedButton);
		}
		protected override void OnIsMicroTasksSelected(SelectableButtonViewModel clickedButton)
		{
			if(MicroTasksCCAdapter == null)
			{
				MicroTasksCCAdapter = Factory.CreateNewMicroTasksCCAdapter(new List<MicroTaskModel>());
			}
			IsMicroTasksBtnSelected = IsMicroTasksType = UpdateButtonState(clickedButton);
		}

		protected override void OnIsEventValueType(SelectableButtonViewModel clickedButton)
		{
			IsValueBtnSelected = IsValueType = UpdateButtonState(clickedButton);
		}
	}
}
