﻿using CommunityToolkit.Mvvm.Input;
using Kalendarzyk.Models.EventModels;
using Kalendarzyk.Models.EventTypesModels;
using System.Collections.ObjectModel;

namespace Kalendarzyk.Views.CustomControls.CCInterfaces.EventTypeExtraOptions
{

	/// <summary>
	/// Its good to use MeasurementOperationsHelperClass
	/// </summary>
	public interface IMeasurementSelectorCC
	{
		// Properties
		ObservableCollection<MeasurementUnitItem> MeasurementUnitsOC { get; set; }
		RelayCommand<MeasurementUnitItem> MeasurementUnitSelectedCommand { get; set; }
		MeasurementUnitItem SelectedMeasurementUnit { get; set; }
        QuantityModel QuantityAmount { get; set; }
		public string QuantityValueText { get; set; }
		public decimal QuantityValue { get; set; }
		void SelectPropperMeasurementData(EventTypeModel userEventTypeModel);
	}
}
