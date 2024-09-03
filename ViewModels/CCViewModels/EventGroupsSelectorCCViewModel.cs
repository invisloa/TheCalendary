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
        private readonly Dictionary<EventGroupModel, EventGroupViewModel> _eventVisualDetails;
        private readonly IMediator _mediator;
        private EventGroupModel _selectedEventGroup;
        private EventGroupViewModel _selectedEventGroupViewModel;
        private IconModel _selectedVisualElement;
        private Color _selectedColor = Color.FromRgb(255, 0, 0); // Default to red

        // Properties
        public ObservableCollection<EventGroupViewModel> EventGroupsVisualsOC { get; set; }
        public ObservableCollection<EventGroupViewModel> AllEventGroupsOC { get; set; }

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

        public EventGroupViewModel SelectedEventGroupViewModel
        {
            get => _selectedEventGroupViewModel;
            set
            {
                if (_selectedEventGroupViewModel != value)
                {
                    _selectedEventGroupViewModel = value;
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
            _eventVisualDetails = new Dictionary<EventGroupModel, EventGroupViewModel>();

            AllEventGroupsOC = new ObservableCollection<EventGroupViewModel>(eventGroupModels.Select(x => new EventGroupViewModel(x)));
            InitializeEventGroupsVisuals();

            EventGroupSelectedCommand = new RelayCommand<EventGroupViewModel>(SetEventGroupFromViewModel);

            // Subscribe to mediator notifications
            _mediator.Subscribe("EventGroupAdded", OnEventGroupAdded);
            _mediator.Subscribe("EventGroupUpdated", OnEventGroupUpdated);
            _mediator.Subscribe("EventGroupRemoved", OnEventGroupRemoved);
        }

        // Private Methods
        private void SetEventGroupFromViewModel(EventGroupViewModel viewModel)
        {
            var selectedEventGroup = _eventGroupsList.FirstOrDefault(o => o.Equals(viewModel.EventGroup));
            if (selectedEventGroup == null)
            {
                throw new ArgumentException($"Invalid EventGroup value: {viewModel.EventGroup}");
            }
            SelectedEventGroup = selectedEventGroup;
            VisuallySelectEventGroupElement();
            EventGroupChanged?.Invoke(SelectedEventGroup);
        }

        private void VisuallySelectEventGroupElement()
        {
            foreach (var eventType in _eventVisualDetails.Values)
            {
                eventType.IsSelected = false;
            }
            if (SelectedEventGroup != null && _eventVisualDetails.ContainsKey(SelectedEventGroup))
            {
                _eventVisualDetails[SelectedEventGroup].IsSelected = true;
            }
        }

        private void InitializeEventGroupsVisuals()
        {
            EventGroupsVisualsOC = new ObservableCollection<EventGroupViewModel>();

            foreach (var eventType in _eventGroupsList)
            {
                var viewModel = new EventGroupViewModel(eventType);
                _eventVisualDetails[eventType] = viewModel;
                EventGroupsVisualsOC.Add(viewModel);
            }
        }

        private void OnSelectEventGroupCommand(EventGroupViewModel eventGroup)
        {
            AllEventGroupsOC.ToList().ForEach(x => x.IsSelected = false); // Deselect all
            eventGroup.IsSelected = true;
            SelectedEventGroupViewModel = eventGroup;
        }

        private void OnEventGroupAdded(object sender, object args)
        {
            if (args is EventGroupModel newGroup)
            {
                var newViewModel = new EventGroupViewModel(newGroup);
                AllEventGroupsOC.Add(newViewModel);
                OnPropertyChanged(nameof(AllEventGroupsOC));
            }
        }

        private void OnEventGroupUpdated(object sender, object args)
        {
            if (args is EventGroupModel updatedGroup)
            {
                var existingViewModel = AllEventGroupsOC.FirstOrDefault(vm => vm.Id == updatedGroup.Id);
                if (existingViewModel != null)
                {
                    existingViewModel.UpdateFromModel(updatedGroup);
                    OnPropertyChanged(nameof(AllEventGroupsOC));
                }
            }
        }

        private void OnEventGroupRemoved(object sender, object args)
        {
            if (args is EventGroupModel removedGroup)
            {
                var existingViewModel = AllEventGroupsOC.FirstOrDefault(vm => vm.Id == removedGroup.Id);
                if (existingViewModel != null)
                {
                    AllEventGroupsOC.Remove(existingViewModel);
                    OnPropertyChanged(nameof(AllEventGroupsOC));
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
