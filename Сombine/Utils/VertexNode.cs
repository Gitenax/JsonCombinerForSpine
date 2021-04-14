using System.Collections;

namespace Сombine.Utils
{
    /// <summary>
    /// Представляет контейнер связанных между собой костями вершин
    /// </summary>
    public class VertexNode : IEnumerable
    {
        public VertexNode(Vertex vertex)
        {
            Vertices = new []{ vertex };
        }
        
        public VertexNode(Vertex[] vertices)
        {
            Vertices = vertices;
        }
        
        
        public Vertex this[int index]
        {
            get
            {
                if (index >= 0 && index < Vertices.Length)
                    return Vertices[index];
                return default;
            }
            set
            {
                if (index >= 0 && index < Vertices.Length)
                    Vertices[index] = value;
            }
        }
        
        
        public int Count => Vertices.Length;
        public Vertex[] Vertices { get; set; }
        
        public IEnumerator GetEnumerator()
        {
            return Vertices.GetEnumerator();
        }
    }
}