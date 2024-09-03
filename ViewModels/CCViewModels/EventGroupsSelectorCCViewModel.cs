using Kalendarzyk.Models.EventModels;
using Kalendarzyk.ViewModels.ModelsViewModels;
using Kalendarzyk.Mediator;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Kalendarzyk.ViewModels.CCViewModels
{
    internal class EventGroupsSelectorCCViewModel : BaseViewModel, IEventGroupsSelectorCCViewModel
    {
        private readonly IMediator _mediator;
        private EventGroupViewModel _selectedEventGroup;

        public ObservableCollection<EventGroupViewModel> AllEventGroupsOC { get; set; }
        public EventGroupViewModel SelectedEventGroup
        {
            get => _selectedEventGroup;
            set
            {
                _selectedEventGroup = value;
                OnPropertyChanged();
            }
        }
        public ICommand SelectEventGroupCommand { get; set; }
        
        //ctor
        public EventGroupsSelectorCCViewModel(ObservableCollection<EventGroupModel> eventGroupModels, IMediator mediator)
        {
            _mediator = mediator;
            AllEventGroupsOC = new ObservableCollection<EventGroupViewModel>(eventGroupModels.Select(x => new EventGroupViewModel(x)));
            SelectEventGroupCommand = new RelayCommand<EventGroupViewModel>(OnSelectEventGroupCommand);

            // Subscribe to mediator notifications
            _mediator.Subscribe("EventGroupAdded", OnEventGroupAdded);
            _mediator.Subscribe("EventGroupUpdated", OnEventGroupUpdated);
            _mediator.Subscribe("EventGroupRemoved", OnEventGroupRemoved);
        }

        private void OnSelectEventGroupCommand(EventGroupViewModel eventGroup)
        {
            AllEventGroupsOC.ToList().ForEach(x => x.IsSelected = false); // Deselect all
            eventGroup.IsSelected = true;
            SelectedEventGroup = eventGroup;
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

        // Unsubscribe from mediator notifications when disposing!!!!!!!!!!!!!!!!!!!
        public void Dispose()
        {
            _mediator.Unsubscribe("EventGroupAdded", OnEventGroupAdded);
            _mediator.Unsubscribe("EventGroupUpdated", OnEventGroupUpdated);
            _mediator.Unsubscribe("EventGroupRemoved", OnEventGroupRemoved);
        }
    }
}
