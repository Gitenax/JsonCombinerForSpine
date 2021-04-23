using System.Text.Json.Serialization;
using Сombine.Units;

namespace Сombine.Components.Attachments
{
    public class Clipping : Attachment, IVertexIncludingAttachment
    {
        private float[] _vertices;
        
        [JsonConstructor]
        public Clipping(string type, int vertexCount)
        {
            Type = type;
            VertexCount = vertexCount;
        }
        
        
        /// <summary>
        /// Имя слота
        /// </summary>
        public string End { get; set; }

        public float[] Vertices
        {
            get => VertexCollection?.ToArray() ?? _vertices;
            set => _vertices = value;
        }
        
        [JsonIgnore]
        public VertexCollection VertexCollection { get; set; }
        
        public int VertexCount { get; }

        public object Color { get; set; }
    }
}