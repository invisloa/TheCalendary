using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kalendarzyk.Models.EventModels
{
    [SQLite.Table("EventModel")]

    public class EventModel
    {
        private TimeSpan _defaulteventRemindertime = TimeSpan.FromHours(24);

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }  // Use int for autoincrement

        [MaxLength(55)]
        public string Name { get; set; }
        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }
        [MaxLength(255)]
        public string Description { get; set; }

        public bool WasShown { get; set; }

        public bool IsCompleted { get; set; }

        public int EventTypeId { get; set; }
        public TimeSpan ReminderTime { get; set; }
        // Ignored properties (not stored directly in the database)
        [Ignore]
        public EventTypeModel EventType { get; set; }
        // Ignored properties (not stored directly in the database)
        public decimal? Value { get; set; }
        [Ignore]
        public List<MicroTaskModel> MicroTasks { get; set; }

        public EventModel() { }

        public EventModel(string name, string description, DateTime startTime, DateTime endTime, EventTypeModel eventType, decimal? value = null,
                                  bool isCompleted = false, TimeSpan? remindTime = null, bool wasShown = false, Guid? id = null, int? notificationID = null, bool usesNotification = false)
        {
            ReminderTime = remindTime ?? _defaulteventRemindertime;
            Name = name;
            Description = description;
            StartDateTime = startTime;
            EndDateTime = endTime;
            EventType = eventType; // Use concrete type
            IsCompleted = isCompleted;
            WasShown = wasShown;
            Value = value;
        }
    }
}
