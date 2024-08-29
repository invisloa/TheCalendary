using SQLite;

namespace Kalendarzyk.Models.EventModels
{
    [Table("MicroTaskModel")]

    public partial class MicroTaskModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int AssociatedId { get; set; } // Can be an EventModel Id or EventTypeModel Id

        public TaskType Type { get; set; } // Enum to specify the type (Event or EventType)

        [MaxLength(255)]
        public string Name { get; set; }

        public bool IsCompleted { get; set; } = false;

        public MicroTaskModel() { }

        public MicroTaskModel(string name, int associatedId, TaskType type, bool isCompleted = false)
        {
            Name = name;
            AssociatedId = associatedId;
            Type = type;
            IsCompleted = isCompleted;
        }
        public MicroTaskModel(string name)
        {
            Name = name;
            IsCompleted = false;
        }
    }
    // Enum to represent Task Type
    public enum TaskType
    {
        Event,       // Task associated with an Event
        EventType    // Default Task associated with an EventType
    }
}

