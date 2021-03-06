using System.Text.Json.Serialization;
using Сombine.Units;

namespace Сombine.Components.Attachments
{
    public class BoundingBox : Attachment, IVertexIncludingAttachment
    {
        private float[] _vertices;
        private int _vertexCount;
        
        
        [JsonConstructor]
        public BoundingBox(string type, int vertexCount, object color)
        {
            Type = type;
            _vertexCount = vertexCount;
            Color = color;
        }

        public BoundingBox(string name)
        {
            Name = name;
            Type = "boundingbox";
        }

        
        public int VertexCount
        {
            get => VertexCollection?.Count ?? _vertexCount;
            set => _vertexCount = value;
        }

        public float[] Vertices
        {
            get => VertexCollection?.ToArray() ?? _vertices;
            set => _vertices = value;
        }
        
        [JsonIgnore]
        public VertexCollection VertexCollection { get; set; }
        
        public object Color { get; set; }
    }
}