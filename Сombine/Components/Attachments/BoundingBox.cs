using System.Text.Json.Serialization;
using Сombine.Utils;


namespace Сombine.Components.Attachments
{
    public class BoundingBox : Attachment
    {
        private float[] _vertices;

        public BoundingBox(string type, int vertexCount, object color)
        {
            Type = type;
            VertexCount = vertexCount;
            Color = color;
        }

        public BoundingBox(string name)
        {
            Name = name;
            Type = "boundingbox";
        }
        
            
        public string Type { get; set; }
        
        public int VertexCount { get; set; }
        
        public float[] Vertices
        {
            get => VertexCollection?.ToArray() ?? _vertices;
            set => _vertices = value;
        }
        
        [JsonIgnore]
        public VertexCollection VertexCollection { get; set; }
        
        public object Color { get; set; }
    }
    
    
    
    
    // vertexCount: The number of bounding box vertices.
    // vertices: For each vertex either an x,y pair or, for a weighted bounding box, first the number of bones which influence the vertex, then for that many bones: bone index, bind position X, bind position Y, weight. A bounding box is weighted if the number of vertices > vertex count.
    // color: The color of the bounding box in Spine. Assume 60F000FF RGBA if omitted. Nonessential.

}