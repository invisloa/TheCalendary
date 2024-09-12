using SQLite;

namespace Kalendarzyk.Models.EventModels
{
    [Table("IconModel")]
    public class IconModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; } // Primary Key
        [MaxLength(55)]
        public string ElementName { get; set; }
        [MaxLength(55)]
        public string BackgroundColorString { get; set; }
        [MaxLength(55)]
        public string TextColorString { get; set; }
        [Ignore]
        public Color BackgroundColor => Color.FromRgba(BackgroundColorString);
        [Ignore]
        public Color TextColor => Color.FromRgba(TextColorString);
        public IconModel() { }

        public IconModel(string icon, Color backgroundColor, Color textColor)
        {
            ElementName = icon;
            BackgroundColorString = backgroundColor.ToArgbHex();
            TextColorString = textColor.ToArgbHex();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            IconModel other = (IconModel)obj;
            return ElementName == other.ElementName &&
                   BackgroundColorString == other.BackgroundColorString &&
                   TextColorString == other.TextColorString;
        }

        public override int GetHashCode()
        {
            return (ElementName, BackgroundColorString, TextColorString).GetHashCode();
        }
    }
}