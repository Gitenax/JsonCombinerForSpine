using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Сombine.Components;

namespace Сombine.Units
{
    public class VertexCollection : IEnumerable
    {
        private List<VertexNode> _nodes;

        public VertexCollection(int vetexCount)
        {
            _nodes = new List<VertexNode>();
        }

        public VertexNode this[int index]
        {
            get
            {
                if (index >= 0 && index < _nodes.Count)
                    return _nodes[index];
                return null;
            }
            set
            {
                if (index >= 0 && index < _nodes.Count)
                    _nodes[index] = value;
            }
        }


        public int Count => _nodes.Count;
        
        public void Add(VertexNode node)
        {
            _nodes.Add(node);
        }

        public void AddRange(VertexNode[] containers)
        {
            _nodes.AddRange(containers);
        }

        public float[] ToArray()
        {
            bool isWeighted = _nodes[0].Vertices[0].Connected;
            List<float> array = new List<float>();
            
            foreach (var container in _nodes)
            {
                if(isWeighted) array.Add(container.Count);
                
                foreach (var vtx in container.Vertices)
                    array.AddRange(vtx.ToArray());
            }
            return array.ToArray();
        }

        public Bone[] GetRelativeBones()
        {
            HashSet<Bone> bones = new HashSet<Bone>();
            foreach (var node in _nodes)
            {
                foreach (Vertex vertex in node)
                {
                    if (vertex.Connected)
                        bones.Add(vertex.Parent);
                }
            }
            return bones.ToArray();
        }

        public IEnumerator GetEnumerator()
        {
            return _nodes.GetEnumerator();
        }
    }
}