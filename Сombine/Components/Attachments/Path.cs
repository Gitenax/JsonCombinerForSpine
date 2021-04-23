using System.Text.Json.Serialization;
using Сombine.Units;

namespace Сombine.Components.Attachments
{
    public class Path: Attachment, IVertexIncludingAttachment
    {
        private float[] _vertices;
        private int _vertexCount;

        [JsonConstructor]
        public Path(bool closed, bool constantSpeed, float[] lengths, int vertexCount)
        {
            Type = "path";
            Closed = closed;
            ConstantSpeed = constantSpeed;
            Lengths = lengths;
            VertexCount = vertexCount;
        }


        public bool Closed { get; set; }

        public bool ConstantSpeed { get; set; }

        public float[] Lengths { get; set; }
        
        /// <summary>
        ///     Для каждой вершины либо пара x, y, либо, для взвешенной сетки(имеющей вес для вершины),
        ///     сначала количество костей, которые влияют на вершину, затем для этого количества костей:
        ///     индекс кости, позиция привязки X, позиция привязки Y, вес.
        ///     <remarks>Сетка считается взвешенной, если количество вершин больше количества UV.</remarks>
        /// </summary>
        public float[] Vertices
        {
            get => VertexCollection?.ToArray() ?? _vertices;
            set => _vertices = value;
        }
        
        [JsonIgnore]
        public VertexCollection VertexCollection { get; set; }

        public int VertexCount { get; }
        
        /// <summary>
        ///     Цвет тонирования.
        /// </summary>
        public object Color { get; set; }
    }
}