using System.Text.Json.Serialization;
using Сombine.Units;

namespace Сombine.Components.Attachments
{
    public class Mesh : Attachment, IVertexIncludingAttachment
    {
        private float[] _vertices;
        
        
        [JsonConstructor]
        public Mesh(string type, string path, float[] uvs, int[] triangles)
        {
            Type = type;
            Path = path;
            UVs = uvs;
            Triangles = triangles;
        }
        
        public Mesh(string name)
        {
            Name = name;
            Type = "mesh";
        }

        
        /// <summary>
        ///     Если указан, то это значение используется вместо имени вложения для поиска области текстуры.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        ///     Список пар чисел, являющихся координатами текстуры для каждой вершины.
        /// </summary>
        [JsonPropertyName("uvs")]
        public float[] UVs { get; set; }

        /// <summary>
        ///     Список индексов вершин, определяющих каждый треугольник сетки.
        /// </summary>
        public int[] Triangles { get; set; }

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

        [JsonIgnore] 
        public int VertexCount => UVs.Length / 2;
        
        /// <summary>
        ///     Количество вершин, составляющих корпус многоугольника. Вершины корпуса всегда первыми в списке вершин.
        /// </summary>
        public int Hull { get; set; }

        /// <summary>
        ///     Список пар индексов вершин, определяющих ребра между соединенными вершинами.
        ///     <remarks>Несущественное</remarks>
        /// </summary>
        public int[] Edges { get; set; }

        /// <summary>
        ///     Ширина изображения, используемого сеткой.
        ///     <remarks>Несущественное</remarks>
        /// </summary>
        public float Width { get; set; }

        /// <summary>
        ///     Высота изображения, используемого сеткой.
        ///     <remarks>Несущественное</remarks>
        /// </summary>
        public float Height { get; set; }
    }
}