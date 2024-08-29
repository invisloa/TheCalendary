using SQLite;
using System;
using System.Collections.Generic;

namespace Kalendarzyk.Models.EventModels
{
    [Table("EventTypeModel")]
    public class EventTypeModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int EventGroupId { get; set; } // Foreign Key to EventGroupModel
        [MaxLength(55)]
        [Unique]
        public string EventTypeName { get; set; }

        public string EventTypeColorString { get; set; }

        public TimeSpan DefaultEventTimeSpan { get; set; }

        public bool IsValueType { get; set; }

        public bool IsMicroTaskType { get; set; }

        public MeasurementUnit? MeasurementUnit { get; set; }
        public int? DefaultValue { get; set; }
        [Ignore]
        public List<MicroTaskModel>? DefaultMicroTasks { get; set; }

        // Ignored properties (not stored directly in the database)
        [Ignore]
        public EventGroupModel EventGroup { get; set; }

        [Ignore]
        public QuantityModel? DefaultQuantity { get; set; }

        public EventTypeModel() { }

        public EventTypeModel(int eventGroupId, string eventTypeName, string eventTypeColorString, TimeSpan defaultEventTime, MeasurementUnit? measurementUnit = null, int? defaultValue = null, List<MicroTaskModel>? defaultMicroTasks = null)
        {
            EventGroupId = eventGroupId;
            EventTypeName = eventTypeName;
            EventTypeColorString = eventTypeColorString;
            DefaultEventTimeSpan = defaultEventTime;
            DefaultMicroTasks = defaultMicroTasks;
            if(measurementUnit != null && defaultValue != null)
            {
                DefaultQuantity = new QuantityModel((MeasurementUnit)measurementUnit, (int)defaultValue);
                DefaultValue = defaultValue;
                MeasurementUnit = measurementUnit;
                IsValueType = true;
            }
        }

        public override string ToString()
        {
            return EventTypeName;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            EventTypeModel other = (EventTypeModel)obj;
            return Id == other.Id;
        }

    }
}
