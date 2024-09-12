using Kalendarzyk.Models.EventModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kalendarzyk.ViewModels.ModelsViewModels
{
    public class EventTypeViewModel : BaseViewModel
    {
        
        private EventTypeModel _eventTypeModel;

        public EventTypeViewModel(EventTypeModel model)
        {
            _eventTypeModel = model ?? throw new ArgumentNullException(nameof(model));
        }
        public int Id => _eventTypeModel.Id;
        public EventGroupModel EventGroup
        {
            get => _eventTypeModel.EventGroup;
            set
            {
                if (_eventTypeModel.EventGroup != value)
                {
                    _eventTypeModel.EventGroup = value;
                    OnPropertyChanged();
                }
            }
        }

        public string EventTypeName
        {
            get => _eventTypeModel.EventTypeName;
            set
            {
                if (_eventTypeModel.EventTypeName != value)
                {
                    _eventTypeModel.EventTypeName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string EventTypeColorString
        {
            get => _eventTypeModel.EventTypeColorString;
            set
            {
                if (_eventTypeModel.EventTypeColorString != value)
                {
                    _eventTypeModel.EventTypeColorString = value;
                    OnPropertyChanged();
                }
            }
        }

        public TimeSpan DefaultEventTimeSpan
        {
            get => _eventTypeModel.DefaultEventTimeSpan;
            set
            {
                if (_eventTypeModel.DefaultEventTimeSpan != value)
                {
                    _eventTypeModel.DefaultEventTimeSpan = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(FormattedTimeSpan)); // Notify that FormattedTimeSpan changed as well
                }
            }
        }

        public bool IsValueType
        {
            get => _eventTypeModel.IsValueType;
            set
            {
                if (_eventTypeModel.IsValueType != value)
                {
                    _eventTypeModel.IsValueType = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsMicroTaskType
        {
            get => _eventTypeModel.IsMicroTaskType;
            set
            {
                if (_eventTypeModel.IsMicroTaskType != value)
                {
                    _eventTypeModel.IsMicroTaskType = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isSelectedToFilter;
        public bool IsSelectedToFilter
        {
            get => _isSelectedToFilter;
            set
            {
                if (_isSelectedToFilter != value)
                {
                    _isSelectedToFilter = value;
                    OnPropertyChanged();
                }
            }
        }


        public MeasurementUnit? MeasurementUnit
        {
            get => _eventTypeModel.MeasurementUnit;
            set
            {
                if (_eventTypeModel.MeasurementUnit != value)
                {
                    _eventTypeModel.MeasurementUnit = value;
                    OnPropertyChanged();
                }
            }
        }

        public string FormattedTimeSpan => DefaultEventTimeSpan.ToString(@"hh\:mm");
        public override string ToString()
        {
            return $"{EventTypeName} - {FormattedTimeSpan}";
        }

        public bool Equals(EventTypeModel obj)
        {
            return _eventTypeModel.Equals(obj);
        }
        public void UpdateFromModel(EventTypeModel updatedModel)
        {
            if (updatedModel == null) throw new ArgumentNullException(nameof(updatedModel));

            // Update the underlying model
            _eventTypeModel = updatedModel;

            // Notify the UI about changes to the properties
            OnPropertyChanged(nameof(Id));
            OnPropertyChanged(nameof(EventGroup));
            OnPropertyChanged(nameof(EventTypeName));
            OnPropertyChanged(nameof(EventTypeColorString));
            OnPropertyChanged(nameof(DefaultEventTimeSpan));
            OnPropertyChanged(nameof(IsValueType));
            OnPropertyChanged(nameof(IsMicroTaskType));
            OnPropertyChanged(nameof(MeasurementUnit));
            OnPropertyChanged(nameof(FormattedTimeSpan)); // Dependent property, must notify if DefaultEventTimeSpan changes
        }

        public override int GetHashCode()
        {
            return _eventTypeModel.GetHashCode();
        }
    }
}
