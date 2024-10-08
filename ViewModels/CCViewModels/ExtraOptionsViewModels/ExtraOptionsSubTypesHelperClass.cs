﻿using Kalendarzyk.ViewModels;
using Kalendarzyk.Views.CustomControls.ExtraOptionsCC.ExtraOptionsViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Kalendarzyk.Services;
using Kalendarzyk.Models.EventModels;
using Kalendarzyk.ViewModels.CustomControls.Buttons;
using CommunityToolkit.Maui.Core.Extensions;

namespace Kalendarzyk.Views.CustomControls.CCViewModels
{
    public class ExtraOptionsEventTypesHelperClass : ExtraOptionsBaseClass
    {
        private bool _isColorBtnSelected;
        public bool IsColorBtnSelected
        {
            get => _isColorBtnSelected;
            set => SetProperty(ref _isColorBtnSelected, value);
        }

        private Command _onIsColorBtnSelectedCommand;
        public Command OnIsColorBtnSelectedCommand
        {
            get => _onIsColorBtnSelectedCommand;
            set => SetProperty(ref _onIsColorBtnSelectedCommand, value);
        }

        // Constructor for create mode
        public ExtraOptionsEventTypesHelperClass()
        {
            InitializeCommon();
        }

        // Constructor for edit mode
        public ExtraOptionsEventTypesHelperClass(EventTypeModel eventTypeModel)
        {
            EventType = eventTypeModel;
            InitializeCommon();
            ConfigureForEventType();
            SetPropperValueType();
        }

        private void InitializeCommon()
        {
            if (ExtraOptionsButtonsSelectors.Count == 0)
            {
                ExtraOptionsButtonsSelectors.Add(new SelectableButtonViewModel("Micro Tasks(D)", false, new RelayCommand<SelectableButtonViewModel>(OnIsMicroTasksSelected), isEnabled: true));
                ExtraOptionsButtonsSelectors.Add(new SelectableButtonViewModel("Value(D)", false, new RelayCommand<SelectableButtonViewModel>(OnIsEventValueTypeSelected), isEnabled: true));
                ExtraOptionsButtonsSelectors.Add(new SelectableButtonViewModel("Default timespan", false, new RelayCommand<SelectableButtonViewModel>(OnDefaultTimespanSelected), isEnabled: true));
                ExtraOptionsButtonsSelectors.Add(new SelectableButtonViewModel("BG color", false, new RelayCommand<SelectableButtonViewModel>(OnColorBtnSelected), isEnabled: true));
            }
            else
            {
                throw new Exception("ExtraOptionsButtonsSelectors.Count != 0");
            }
        }

        private void ConfigureForEventType()
        {
            if (EventType.IsValueType)
            {
                ConfigureForValueType();
            }

            if (EventType.IsMicroTaskType)
            {
                ConfigureForMicroTaskType();
            }
        }

        private void ConfigureForValueType()
        {
            IsValueTypeSelected = true;
            ExtraOptionsButtonsSelectors[1].IsEnabled = false;
            ExtraOptionsButtonsSelectors[1].ButtonCommand = null;
            DefaultMeasurementSelectorCCHelper = Factory.CreateNewMeasurementSelectorCCHelperClass();
            DefaultMeasurementSelectorCCHelper.QuantityAmount = new QuantityModel(DefaultMeasurementSelectorCCHelper.SelectedMeasurementUnit.TypeOfMeasurementUnit, DefaultMeasurementSelectorCCHelper.QuantityValue);

            if (EventType.DefaultQuantity != null)
            {
                OnIsEventValueTypeSelected(ExtraOptionsButtonsSelectors[1]);
                var unitToSelect = DefaultMeasurementSelectorCCHelper.MeasurementUnitsOC.First(x => x.TypeOfMeasurementUnit == EventType.DefaultQuantity.Unit);
                DefaultMeasurementSelectorCCHelper.SelectedMeasurementUnit = unitToSelect;
                DefaultMeasurementSelectorCCHelper.MeasurementUnitsOC = new ObservableCollection<MeasurementUnitItem> { unitToSelect };
                DefaultMeasurementSelectorCCHelper.QuantityValue = EventType.DefaultQuantity.Value;
            }

            DefaultMeasurementSelectorCCHelper.SelectPropperMeasurementData(EventType);
        }

        private void ConfigureForMicroTaskType()
        {
            IsMicroTasksType = true;
            MicroTasksCCAdapter = Factory.CreateNewMicroTasksCCAdapter(new List<MicroTaskModel>());

            if (EventType.DefaultMicroTasks != null && EventType.DefaultMicroTasks.Any())
            {
                OnIsMicroTasksSelected(ExtraOptionsButtonsSelectors[0]);
                MicroTasksCCAdapter.MicroTasksOC = EventType.DefaultMicroTasks;
            }
        }

        private void OnColorBtnSelected(SelectableButtonViewModel clickedButton)
        {
            IsColorBtnSelected = UpdateButtonState(clickedButton);
        }

        protected override void OnIsMicroTasksSelected(SelectableButtonViewModel clickedButton)
        {
            if (MicroTasksCCAdapter == null)
            {
                MicroTasksCCAdapter = Factory.CreateNewMicroTasksCCAdapter(new List<MicroTaskModel>());
            }
            IsMicroTasksType = UpdateButtonState(clickedButton);
        }

        protected override void OnIsEventValueTypeSelected(SelectableButtonViewModel clickedButton)
        {
            IsValueTypeSelected = UpdateButtonState(clickedButton);
        }
        void OnDefaultTimespanSelected(SelectableButtonViewModel clickedButton)
        {
            IsDefaultTimespanSelected = UpdateButtonState(clickedButton);
        }

        
    }
}
