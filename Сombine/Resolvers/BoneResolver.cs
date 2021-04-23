using System;
using System.Collections.Generic;
using System.Linq;
using Сombine.Components;
using Сombine.Components.Attachments;
using Сombine.Units;

namespace Сombine.Resolvers
{
    internal static class BoneResolver
    {
        public static void AssignParentToBones(Bone[] bones)
        {
            foreach (var bone in bones)
            {
                if(string.IsNullOrEmpty(bone.ParentName)) continue;
                
                var parent = FindByName(bones, bone.ParentName);
                bone.SetParent(parent);
            }

            AssignBoneIndex(bones);
        }
        

        /// <exception cref="NullReferenceException" />
        public static Bone FindByName(Bone[] bones, string name)
        {
            foreach (var bone in bones)
                if (bone.Name.Equals(name)) return bone;
            
            return default;
        }
        
        public static Bone FindByIndex(Bone[] bones, int index)
        {
            foreach (var bone in bones)
                if (bone.Index.Equals(index)) return bone;
            
            return default;
        }

        public static void AdjustBonePositions(Bone[] bones, float scaleX, float scaleY, bool scaleLenght = false)
        {
            foreach (var bone in bones)
            {
                if (scaleLenght)
                    bone.AdjustLength(scaleX, scaleY);
                
                bone.Adjust(scaleX, scaleY);
            }
        }

        public static Bone[] GetAttachmentBones(IVertexIncludingAttachment attachment, Bone[] allBones, bool includeParent = false)
        {
            HashSet<Bone> bones = new HashSet<Bone>();
            
            foreach (VertexNode node in attachment.VertexCollection)
            {
                foreach (Vertex vertex in node)
                {
                    if(vertex.Connected)
                    {
                        if (vertex.Parent == null)
                            bones.Add(FindByIndex(allBones, vertex.BoneIndex));

                        bones.Add(vertex.Parent);
                        
                        if(includeParent)
                        {
                            bool endBone = false;
                            Bone boneParent = vertex.Parent.Parent;
                            while (true)
                            {
                                if (boneParent == null) break;

                                bones.Add(boneParent);
                                boneParent = boneParent.Parent;
                            }
                        }
                    }
                }
            }
            return bones.ToArray();
        }
        
        private static void AssignBoneIndex(Bone[] bones)
        {
            for (int i = 0; i < bones.Length; i++)
            {
                bones[i].Index = i;
            }
        }
    }
}