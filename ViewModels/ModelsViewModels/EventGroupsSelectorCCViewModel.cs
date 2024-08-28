using CommunityToolkit.Mvvm.Input;
using Kalendarzyk.Models.EventModels;
using Kalendarzyk.Views.CustomControls.CCInterfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Kalendarzyk.ViewModels.ModelsViewModels
{
    public class EventGroupsSelectorCCViewModel : BaseViewModel, IEventGroupsCCViewModel
    {
        // Constants
        private const int FullOpacity = 1;
        private const float FadedOpacity = 0.3f;
        private const int NoBorderSize = 0;
        private const int BorderSize = 10;

        // Fields
        private readonly ObservableCollection<EventGroupModel> _eventGroupsList;
        private readonly Dictionary<EventGroupModel, EventGroupViewModel> _eventVisualDetails;

        // Properties
        public ObservableCollection<EventGroupViewModel> EventGroupsVisualsOC { get; set; }

        private EventGroupModel _selectedEventGroup;
        public EventGroupModel SelectedEventGroup
        {
            get => _selectedEventGroup;
            set
            {
                if (SetProperty(ref _selectedEventGroup, value))
                {
                    VisuallySelectEventGroupElement();
                    EventGroupChanged?.Invoke(_selectedEventGroup);
                }
            }
        }

        private IconModel _selectedVisualElement;
        public IconModel SelectedVisualElement
        {
            get => _selectedVisualElement;
            set => SetProperty(ref _selectedVisualElement, value);
        }

        private Color _selectedColor = Color.FromRgb(255, 0, 0); // Default to red
        public Color SelectedColor
        {
            get => _selectedColor;
            set => SetProperty(ref _selectedColor, value);
        }

        // Commands
        public RelayCommand<EventGroupViewModel> EventGroupSelectedCommand { get;  set; }

        // Events
        public event Action<EventGroupModel> EventGroupChanged;

        // Constructor
        public EventGroupsSelectorCCViewModel(ObservableCollection<EventGroupModel> eventGroupsList)
        {
            _eventGroupsList = eventGroupsList ?? throw new ArgumentNullException(nameof(eventGroupsList));
            _eventVisualDetails = new Dictionary<EventGroupModel, EventGroupViewModel>();
            EventGroupSelectedCommand = new RelayCommand<EventGroupViewModel>(SetEventGroupFromViewModel);
            RefreshGroups();
        }

        // Private Methods
        private void SetEventGroupFromViewModel(EventGroupViewModel viewModel)  // collection contains objects of type EventGroupViewModel thats why we need to convert it to EventGroupModel
        {
            var selectedEventGroup = _eventGroupsList.FirstOrDefault(o => o.Equals(viewModel.EventGroup));
            if (selectedEventGroup == null)
            {
                throw new ArgumentException($"Invalid EventGroup value: {viewModel.EventGroup}");
            }
            SelectedEventGroup = selectedEventGroup;
        }

        private void VisuallySelectEventGroupElement()
        {
            foreach (var eventType in _eventVisualDetails.Values)
            {
                eventType.IsSelected = false;
            }

            if (SelectedEventGroup != null && _eventVisualDetails.ContainsKey(SelectedEventGroup))
            {
                var eventGroupToSelect = _eventVisualDetails[SelectedEventGroup];
                eventGroupToSelect.IsSelected = true;
            }
        }

        public void RefreshGroups()
        {
            EventGroupsVisualsOC.Clear();

            foreach (EventGroupModel eventType in _eventGroupsList)
            {
                var viewModel = new EventGroupViewModel(eventType);
                _eventVisualDetails[eventType] = viewModel;
                EventGroupsVisualsOC.Add(viewModel);
            }
        }
    }
}
