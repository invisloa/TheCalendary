using Kalendarzyk.ViewModels;
using Kalendarzyk.Views.CustomControls.CCViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Kalendarzyk.Services;
using Kalendarzyk.Models.EventModels;
using Kalendarzyk.ViewModels.CustomControls.Buttons;

namespace Kalendarzyk.Views.CustomControls.ExtraOptionsCC.ExtraOptionsViewModels
{
    public class ExtraOptionsBaseClass : BaseViewModel
    {
        private bool _isMicroTasksType;
        public bool IsMicroTasksType
        {
            get => _isMicroTasksType;
            set
            {
                SetProperty(ref _isMicroTasksType, value);
                
            }
        }

        private bool _isValueTypeSelected;
        public bool IsValueTypeSelected
        {
            get => _isValueTypeSelected;
            set
            {
                SetProperty(ref _isValueTypeSelected, value);

            }
        }


        private MicroTasksCCAdapterVM _microTasksCCAdapter;
        public MicroTasksCCAdapterVM MicroTasksCCAdapter
        {
            get => _microTasksCCAdapter;
            set => SetProperty(ref _microTasksCCAdapter, value);
        }

        private MeasurementSelectorCCViewModel _defaultMeasurementSelectorCCHelper = Factory.CreateNewMeasurementSelectorCCHelperClass();
        public MeasurementSelectorCCViewModel DefaultMeasurementSelectorCCHelper
        {
            get => _defaultMeasurementSelectorCCHelper;
            set => SetProperty(ref _defaultMeasurementSelectorCCHelper, value);
        }

        private ChangableFontsIconCCViewModel _changableFontsIconCC;
        public ChangableFontsIconCCViewModel ChangableFontsIconCC
        {
            get => _changableFontsIconCC;
            set => SetProperty(ref _changableFontsIconCC, value);
        }

        private ObservableCollection<SelectableButtonViewModel> _extraOptionsButtonsSelectors = new ObservableCollection<SelectableButtonViewModel>();
        public ObservableCollection<SelectableButtonViewModel> ExtraOptionsButtonsSelectors
        {
            get => _extraOptionsButtonsSelectors;
            set => SetProperty(ref _extraOptionsButtonsSelectors, value);
        }

        private EventTypeModel _eventType;
        public EventTypeModel EventType
        {
            get => _eventType;
            set => SetProperty(ref _eventType, value);
        }

        protected virtual void OnIsMicroTasksSelected(SelectableButtonViewModel clickedButton)
        {
            IsMicroTasksType = UpdateButtonState(clickedButton);
        }

        protected virtual void OnIsEventValueType(SelectableButtonViewModel clickedButton)
        {
            IsValueTypeSelected = UpdateButtonState(clickedButton);
        }

        protected bool UpdateButtonState(SelectableButtonViewModel clickedButton)
        {
            bool newState = !clickedButton.IsSelected;
            SelectableButtonViewModel.MultiButtonSelection(clickedButton);
            return newState;
        }

        protected void SetPropperValueType()
        {
            DefaultMeasurementSelectorCCHelper.SelectPropperMeasurementData(EventType);
        }
    }
}
