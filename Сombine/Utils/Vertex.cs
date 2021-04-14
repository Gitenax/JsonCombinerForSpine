using System;
using Сombine.Components;

namespace Сombine.Utils
{
    public struct Vertex
    {
        private bool _connectedToBones;

        public Vertex(float x, float y)
        {
            (X, Y) = (x, y);
            _connectedToBones = false;
            BoneIndex = 0;
            Weight = 0;
        }
            
        public Vertex(int boneIndex, float x, float y, float weight)
        {
            (BoneIndex, X, Y, Weight) = (boneIndex, x, y, weight);
            _connectedToBones = true;
        }

            
        public int BoneIndex { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Weight { get; set; }

        public bool Connected => _connectedToBones;


        public void ConvertToWeighted(int boneIndex, Bone relativeBone, float weight)
        {
            BoneIndex = boneIndex;
            Weight = weight;
            X = relativeBone.X - X;
            Y = relativeBone.Y - Y;
        }
        
        public float[] ToArray()
        {
            if (_connectedToBones)
                return new [] { BoneIndex, X, Y,  Weight};
            
            return new [] { X, Y };
        }
    }
}