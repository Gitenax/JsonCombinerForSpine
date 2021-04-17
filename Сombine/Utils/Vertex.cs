using System;
using System.Text.Json.Serialization;
using Сombine.Components;

namespace Сombine.Utils
{
    public struct Vertex
    {
        private bool _connectedToBones;

        public Vertex(Vertex vertex)
        {
            (X, Y) = (vertex.X, vertex.Y);
            _connectedToBones = vertex.Connected;
            BoneIndex = vertex.BoneIndex;
            Weight = vertex.Weight;
            Parent = vertex.Parent;
        }
        
        public Vertex(float x, float y)
        {
            (X, Y) = (x, y);
            _connectedToBones = false;
            BoneIndex = 0;
            Weight = 0;
            Parent = null;
        }
            
        public Vertex(int boneIndex, float x, float y, float weight, Bone parent = null)
        {
            (BoneIndex, X, Y, Weight) = (boneIndex, x, y, weight);
            _connectedToBones = true;
            Parent = parent;
        }

        
        public static Vertex Zero => new Vertex(0, 0);
        
        public int BoneIndex { get; set; }
        
        [JsonIgnore]
        public Bone Parent { get; private set; }
        
        public float X { get; set; }
        
        public float Y { get; set; }
        
        public float Weight { get; set; }

        public bool Connected => _connectedToBones;



        public static Vertex CalculateRatio(float aX, float aY, float bX, float bY)
        {
            return CalculateRatio(new Vertex(aX, aY), new Vertex(bX, bY));
        }
        
        public static Vertex CalculateRatio(Vertex a, Vertex b)
        {
            float ratioX = Math.Abs(100 / (a.X  / b.X ) / 100);
            float ratioY = Math.Abs(100 / (a.Y  / b.Y)  / 100);
            return new Vertex(ratioX, ratioY);
        }
        
        public void AssignBone(Bone parent)
        {
            Parent = parent;
        }

        public void Adjust(float x, float y)
        {
            X *= x;
            Y *= y;
        }
        
        
        public float[] ToArray()
        {
            if (_connectedToBones)
                return new [] { BoneIndex, X, Y,  Weight};
            
            return new [] { X, Y };
        }
    }
}