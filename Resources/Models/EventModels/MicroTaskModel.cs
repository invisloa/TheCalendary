using SQLite;

namespace Kalendarzyk.Models.EventModels
{
    [Table("MicroTaskModel")]
    public partial class MicroTaskModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; } // Primary Key

        [Indexed]
        public int EventId { get; set; } // Foreign Key to EventModelId

        [MaxLength(255)]
        public string Name { get; set; }

        public bool IsCompleted { get; set; } = false;

        public MicroTaskModel() { }

        public MicroTaskModel(string name, int eventModelId, bool isCompleted = false)
        {
            Name = name;
            EventId = eventModelId;
            IsCompleted = isCompleted;
        }
        public MicroTaskModel(string name)
        {
            Name = name;
        }
    }
}
