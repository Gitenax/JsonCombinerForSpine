using System.Text.Json.Serialization;

namespace Сombine.Components
{
    public abstract class Attachment
    {
        public Slot Owner { get; set; }
        [JsonIgnore]
        public string Name { get; set; }
        
        public string Type { get; protected set; }

        public override bool Equals(object obj)
        {
            if (obj is Attachment attachment)
                return attachment.Name == Name;

            return false;
        }
    }
}