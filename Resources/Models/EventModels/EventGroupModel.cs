using SQLite;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kalendarzyk.Models.EventTypesModels
{
    [SQLite.Table("EventGroupModel")]

    public class EventGroupModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; } // Primary Key
        [MaxLength(55)]
        [Unique]
        public string GroupName { get; set; }

        public int SelectedVisualElementId { get; set; } // Foreign Key

        [Ignore]
        public IconModel SelectedVisualElement { get; set; }

        public EventGroupModel() 
        {
            SelectedVisualElement = new IconModel();
        }

        public EventGroupModel(string name, IconModel icon)
        {
            GroupName = name;
            SelectedVisualElement = icon;
        }

        public override string ToString()
        {
            return GroupName;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            EventGroupModel other = (EventGroupModel)obj;
            return Id == other.Id;
        }

    }
}
