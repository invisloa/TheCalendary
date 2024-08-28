using Kalendarzyk.Models.EventModels;
using System.Collections.Generic;

namespace Kalendarzyk.ViewModels.ModelsViewModels
{
    public class EventGroupViewModel : BaseViewModel
    {
        private EventGroupModel _eventGroupModel;
        private bool _isSelected;



        public EventGroupModel EventGroup
        { get => _eventGroupModel; }

        public EventGroupViewModel(EventGroupModel eventGroupModel)
        {
            _eventGroupModel = eventGroupModel;
        }

        public int Id
        {
            get => _eventGroupModel.Id;
            set
            {
                if (_eventGroupModel.Id != value)
                {
                    _eventGroupModel.Id = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }
        public string EventGroupName
        {
            get => _eventGroupModel.GroupName;
            set
            {
                if (_eventGroupModel.GroupName != value)
                {
                    _eventGroupModel.GroupName = value;
                    OnPropertyChanged();
                }
            }
        }

        public int IconModelId
        {
            get => _eventGroupModel.SelectedVisualElementId;
            set
            {
                if (_eventGroupModel.SelectedVisualElementId != value)
                {
                    _eventGroupModel.SelectedVisualElementId = value;
                    OnPropertyChanged();
                }
            }
        }

        public IconModel SelectedVisualElement
        {
            get => _eventGroupModel.SelectedVisualElement;
            set
            {
                if (_eventGroupModel.SelectedVisualElement != value)
                {
                    _eventGroupModel.SelectedVisualElement = value;
                    OnPropertyChanged();
                }
            }
        }
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var otherViewModel = (EventGroupViewModel)obj;

            return Id == otherViewModel.Id;
        }
    }
}
