using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Сombine.Units
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

        public IEnumerable<float> GetX()
        {
            return Vertices.Select(x => x.X);
        }

        public IEnumerable<float> GetY()
        {
            return Vertices.Select(x => x.Y);
        }
        
        public IEnumerator GetEnumerator()
        {
            return Vertices.GetEnumerator();
        }
    }
}