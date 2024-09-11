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
        public string DefaultEventTimeSpanString { get; set; }
        [Ignore]
        public TimeSpan DefaultEventTimeSpan { get; set; }

        public bool IsValueType { get; set; }

        public bool IsMicroTaskType { get; set; }

        public MeasurementUnit? MeasurementUnit 
        {
            get => DefaultQuantity?.Unit; // Return null if DefaultQuantity is null
            set
            {
                if (DefaultQuantity != null)
                {
                    if (value != null)
                    {
                        DefaultQuantity.Unit = (MeasurementUnit)value;
                    }
                }
            }
        }
        public decimal? DefaultValue { 
            get => DefaultQuantity?.Value; 
            set
            {
                if (DefaultQuantity != null)
                {
                    if (value != null)
                    {
                        DefaultQuantity.Value = (decimal)value;
                    }
                }
            }
        }
        [Ignore]
        public List<MicroTaskModel>? DefaultMicroTasks { get; set; }

        // Ignored properties (not stored directly in the database)
        [Ignore]
        public EventGroupModel EventGroup { get; set; }

        [Ignore]
        public QuantityModel? DefaultQuantity { get; set; }

        public EventTypeModel() { }

        public EventTypeModel(EventGroupModel eventGroup, string eventTypeName, string eventTypeColorString, TimeSpan defaultEventTime, QuantityModel? quantity, List<MicroTaskModel>? defaultMicroTasks = null)
        {
            EventGroup = eventGroup;
            EventGroupId = eventGroup.Id;
            EventTypeName = eventTypeName;
            EventTypeColorString = eventTypeColorString;
            DefaultEventTimeSpan = defaultEventTime;
            DefaultEventTimeSpanString = defaultEventTime.ToString();
            DefaultMicroTasks = defaultMicroTasks;
            if(quantity != null)
            {
                DefaultQuantity = quantity;
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
