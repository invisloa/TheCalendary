using Kalendarzyk.Models.EventModels;
using Kalendarzyk.ViewModels.ModelsViewModels;
using Kalendarzyk.Mediator;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Kalendarzyk.ViewModels.CCViewModels
{
    internal class EventGroupsSelectorCCViewModel : BaseViewModel, IEventGroupsCCViewModel, IDisposable
    {
        // Constants
        private const int FullOpacity = 1;
        private const float FadedOpacity = 0.3f;
        private const int NoBorderSize = 0;
        private const int BorderSize = 10;

        // Fields
        private readonly ObservableCollection<EventGroupModel> _eventGroupsList;
        private readonly IMediator _mediator;
        private EventGroupModel _selectedEventGroup;
        private IconModel _selectedVisualElement;
        private Color _selectedColor = Color.FromRgb(255, 0, 0); // Default to red

        // Properties
        public ObservableCollection<EventGroupViewModel> EventGroupsVisualsOC { get; set; }

        public EventGroupModel SelectedEventGroup
        {
            get => _selectedEventGroup;
            set
            {
                if (_selectedEventGroup != value)
                {
                    _selectedEventGroup = value;
                    OnPropertyChanged();
                }
            }
        }
        public IconModel SelectedVisualElement
        {
            get => _selectedVisualElement;
            set
            {
                if (_selectedVisualElement != value)
                {
                    _selectedVisualElement = value;
                    OnPropertyChanged();
                }
            }
        }

        public Color SelectedColor
        {
            get => _selectedColor;
            set
            {
                if (_selectedColor != value)
                {
                    _selectedColor = value;
                    OnPropertyChanged();
                }
            }
        }

        // Commands
        public RelayCommand<EventGroupViewModel> EventGroupSelectedCommand { get; private set; }

        // Events
        public event Action<EventGroupModel> EventGroupChanged;

        // Constructor
        public EventGroupsSelectorCCViewModel(ObservableCollection<EventGroupModel> eventGroupModels, IMediator mediator)
        {
            _mediator = mediator;
            _eventGroupsList = eventGroupModels;

            InitializeEventGroupsVisuals();

            EventGroupSelectedCommand = new RelayCommand<EventGroupViewModel>(SetEventGroupFromViewModel);

            // Subscribe to mediator notifications
            _mediator.Subscribe("EventGroupAdded", OnEventGroupAdded);
            _mediator.Subscribe("EventGroupUpdated", OnEventGroupUpdated);
            _mediator.Subscribe("EventGroupRemoved", OnEventGroupRemoved);
        }

        // Private Methods
        private void SetEventGroupFromViewModel(EventGroupViewModel selectedEventGroupViewModel)
        {
            foreach (var x in EventGroupsVisualsOC)
            {
                x.IsSelected = false;
            }
            var selectedEventGroup = _eventGroupsList.FirstOrDefault(o => o.Equals(selectedEventGroupViewModel.EventGroup));
            if (selectedEventGroup == null)
            {
                throw new ArgumentException($"Invalid EventGroup value: {selectedEventGroupViewModel.EventGroup}");
            }
            SelectedEventGroup = selectedEventGroup;
            selectedEventGroupViewModel.IsSelected = true;
            EventGroupChanged?.Invoke(SelectedEventGroup);
        }

        private void VisuallySelectEventGroupElement()
        {
            foreach (var eventType in EventGroupsVisualsOC)
            {
                eventType.IsSelected = false;
            }
        }

        private void InitializeEventGroupsVisuals()
        {
            EventGroupsVisualsOC = new ObservableCollection<EventGroupViewModel>();

            foreach (var eventType in _eventGroupsList)
            {
                var viewModel = new EventGroupViewModel(eventType);
                EventGroupsVisualsOC.Add(viewModel);
            }
        }

        private void OnEventGroupAdded(object sender, object args)
        {
            if (args is EventGroupModel newGroup)
            {
                var newViewModel = new EventGroupViewModel(newGroup);
                EventGroupsVisualsOC.Add(newViewModel);
                OnPropertyChanged(nameof(EventGroupsVisualsOC));
            }
        }

        private void OnEventGroupUpdated(object sender, object args)
        {
            if (args is EventGroupModel updatedGroup)
            {
                var existingViewModel = EventGroupsVisualsOC.FirstOrDefault(vm => vm.Id == updatedGroup.Id);
                if (existingViewModel != null)
                {
                    existingViewModel.UpdateFromModel(updatedGroup);
                    OnPropertyChanged(nameof(EventGroupsVisualsOC));
                }
            }
        }

        private void OnEventGroupRemoved(object sender, object args)
        {
            if (args is EventGroupModel removedGroup)
            {
                var existingViewModel = EventGroupsVisualsOC.FirstOrDefault(vm => vm.Id == removedGroup.Id);
                if (existingViewModel != null)
                {
                    EventGroupsVisualsOC.Remove(existingViewModel);
                    OnPropertyChanged(nameof(EventGroupsVisualsOC));
                }
            }
        }

        // Dispose Method
        public void Dispose()
        {
            _mediator.Unsubscribe("EventGroupAdded", OnEventGroupAdded);
            _mediator.Unsubscribe("EventGroupUpdated", OnEventGroupUpdated);
            _mediator.Unsubscribe("EventGroupRemoved", OnEventGroupRemoved);
        }
    }
}
